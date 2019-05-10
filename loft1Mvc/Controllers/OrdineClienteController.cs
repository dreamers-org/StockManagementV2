using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;
using Serilog;
using StockManagement.Models;
using StockManagement.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement
{
    public class OrdineClienteController : Controller
    {
        public OrdineClienteController(StockV2Context context, IdentityContext identityContext)
        {
            _context = context;
            _identityContext = identityContext;
        }
        
        #region Costanti&Readonly

        private const string OC = "Id,DataConsegna,NomeCliente,IndirizzoCliente,EmailCliente,CodiceArticolo,ColoreArticolo,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,TagliaUnica";
        private const string SendGridApyKey = "";
        private const string OC1 = "Id,IdRappresentante,IdCliente,DataConsegna,IdPagamento,Note,Completato,Pagato,DataInserimento,DataModifica,UtenteInserimento,UtenteModifica";
        private const string NoteOc = "IdTipoPagamento,Note";
        private readonly StockV2Context _context;
        private readonly IdentityContext _identityContext;

        #endregion

        #region Index
        [Authorize]
        public IActionResult Index()
        {
            try
            {
                if (IsRappresentante())
                {
                    //Ottengo l'Id del rappresentante
                    Guid idRappresentante = _identityContext.Users.Where(x => x.Email == User.Identity.Name).Select(x => new Guid(x.Id)).FirstOrDefault();

                    Utility.CheckNull(idRappresentante);

                    //Prendo gli ordini del rappresentante corrente
                    var listaOrdiniCliente = _context.ViewOrdineCliente.Where(x => x.IdRappresentante == idRappresentante && x.Completato == true).OrderByDescending(x => x.DataInserimento).ToList();

                    Utility.CheckNull(listaOrdiniCliente);

                    return View(listaOrdiniCliente);
                }
                else
                {
                    //Prendo tutti gli ordini dei clienti e li ordino
                    var listaOrdiniCliente = _context.ViewOrdineCliente.OrderByDescending(x => x.DataInserimento);

                    Utility.CheckNull(listaOrdiniCliente);

                    return View(listaOrdiniCliente);
                }
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        #endregion

        #region Create
        [Authorize]
        public IActionResult Create()
        {
            try
            {
                ViewData["NomeCliente"] = HttpContext.Session.GetString("NomeCliente");
                ViewData["EmailCliente"] = HttpContext.Session.GetString("EmailCliente");
                ViewData["IndirizzoCliente"] = HttpContext.Session.GetString("IndirizzoCliente");
                string idOrdineSession = HttpContext.Session.GetString("IdOrdine");
                string dataConsegnaSess = HttpContext.Session.GetString("DataConsegna");
                if (!string.IsNullOrEmpty(dataConsegnaSess)) ViewData["DataConsegna"] = DateTime.Parse(dataConsegnaSess);

                IEnumerable<ViewRigaOrdineClienteViewModel> listaRigheOrdineCliente = new List<ViewRigaOrdineClienteViewModel>();

                if (!string.IsNullOrEmpty(idOrdineSession))
                {
                    int totalePezzi = 0;

                    listaRigheOrdineCliente = _context.ViewRigaOrdineCliente.Where(x => x.IdOrdine.ToString().ToUpper() == idOrdineSession.ToUpper()).Select(x => x).ToList();

                    Utility.CheckNull(listaRigheOrdineCliente);

                    double? sommaPrezzo = _context.ViewOrdineCliente.Where(x => x.Id == new Guid(idOrdineSession)).Select(x => x.SommaPrezzo).FirstOrDefault();

                    Utility.CheckNull(sommaPrezzo);

                    ViewBag.SommaPrezzo = sommaPrezzo.Value;

                    foreach (var item in listaRigheOrdineCliente)
                    {
                        totalePezzi += item.Xxs + item.Xs + item.S + item.M + item.L + item.Xl + item.Xxl + item.Xxxl + item.TagliaUnica;
                    }

                    HttpContext.Session.SetString("TotalePezzi", totalePezzi.ToString());
                }

                ViewBag.ListaOrdini = listaRigheOrdineCliente;

                return View();
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind(OC)] OrdineClienteViewModel ordineCliente)
        {
            try
            {
                Utility.CheckNull(ordineCliente);

                if (!ModelState.IsValid) return RedirectToAction("Create");

                string NomeCliente = string.Empty, EmailCliente = string.Empty;

                //creo i nuovi guid per IdCliente e IdOrdine.
                Guid idCliente = Guid.NewGuid();
                ordineCliente.Id = Guid.NewGuid();

                //Controllo se l'Id dell'ordine esiste già in sessione.
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdOrdine")))
                {
                    idCliente = GetOrCreateIdCliente(ordineCliente, NomeCliente, EmailCliente, idCliente);

                    //setto le informazioni sull'utente, sul cliente e sul rappresentante.
                    ordineCliente.UtenteInserimento = User.Identity.Name;
                    ordineCliente.DataInserimento = DateTime.Now;
                    ordineCliente.IdCliente = idCliente;
                    ordineCliente.IdRappresentante = _identityContext.Users.Where(x => x.Email == User.Identity.Name).Select(x => new Guid(x.Id)).FirstOrDefault();

                    _context.Add(ordineCliente);
                    _context.SaveChanges();
                }
                else
                {
                    //Se esiste significa che il rappresentante ha già creato un ordine.
                    ordineCliente.Id = new Guid(HttpContext.Session.GetString("IdOrdine"));
                    ordineCliente.DataModifica = DateTime.Now;
                    ordineCliente.UtenteModifica = User.Identity.Name;
                }

                if (!Utility.ArticoloExists(ordineCliente.CodiceArticolo, ordineCliente.ColoreArticolo, _context)) throw new ArgumentNullException("Articolo non esistente");

                //ottengo l'articolo.
                Guid idArticolo = _context.Articolo.Where(x => x.Colore.ToUpper() == ordineCliente.ColoreArticolo.ToUpper() && x.Codice.ToUpper() == ordineCliente.CodiceArticolo.ToUpper()).Select(x => x.Id).FirstOrDefault();

                //Se esiste già un record lo modifico altrimenti lo creo.
                List<RigaOrdineCliente> rigaOrdineClienteEsistente = _context.RigaOrdineCliente.Where(x => x.IdOrdine == ordineCliente.Id && x.IdArticolo == idArticolo).ToList();
                RigaOrdineCliente rigaOrdineCliente = GetRigaOrdineCliente(ordineCliente, idArticolo);

                Utility.CheckNull(rigaOrdineClienteEsistente);
                Utility.CheckNull(rigaOrdineCliente);

                if (rigaOrdineClienteEsistente.Count > 0) _context.RigaOrdineCliente.Update(rigaOrdineCliente);
                else _context.RigaOrdineCliente.Add(rigaOrdineCliente);

                _context.SaveChanges();

                //salvo in sessione i valori.
                SalvaDatiOrdineClienteInSessione(ordineCliente);

                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        #endregion

        #region ImpostazionePagamento
        [Authorize]
        public IActionResult ImpostaTipoPagamento()
        {
            try
            {
                string idOrdineSession = HttpContext.Session.GetString("IdOrdine");

                Utility.CheckNull(idOrdineSession);

                double? sommaPrezzo = _context.ViewOrdineCliente.Where(x => x.Id == new Guid(idOrdineSession)).Select(x => x.SommaPrezzo).FirstOrDefault();

                ViewBag.SommaPrezzo = sommaPrezzo.Value;

                IEnumerable<TipoPagamento> listaPagamenti = _context.TipoPagamento.AsEnumerable();

                return GetMetodiPagamentoIdonei(idOrdineSession, sommaPrezzo, ref listaPagamenti);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ImpostaTipoPagamento([Bind(NoteOc)] OrdineCliente ordineCliente)
        {
            Utility.CheckNull(ordineCliente);

            try
            {
                if (ModelState.IsValid)
                {
                    string idOrdineSession = HttpContext.Session.GetString("IdOrdine");

                    Utility.CheckNull(idOrdineSession);

                    OrdineCliente ordineClienteCurrent = _context.OrdineCliente.Where(x => x.Id == Guid.Parse(idOrdineSession)).First();

                    Utility.CheckNull(ordineClienteCurrent);

                    ordineClienteCurrent.UtenteModifica = User.Identity.Name;
                    ordineClienteCurrent.DataModifica = DateTime.Now;
                    ordineClienteCurrent.IdTipoPagamento = ordineCliente.IdTipoPagamento;
                    ordineClienteCurrent.Note = ordineCliente.Note;

                    _context.OrdineCliente.Update(ordineClienteCurrent);
                    _context.SaveChanges();
                }

                return RedirectToAction("Riepilogo");
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        #endregion

        #region Riepilogo
        [Authorize]
        public IActionResult Riepilogo()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Riepilogo(bool condizioniAccettate)
        {
            try
            {
                if (!condizioniAccettate) return View();

                string idOrdineSession = HttpContext.Session.GetString("IdOrdine");

                Utility.CheckNull(idOrdineSession);

                OrdineCliente ordineClienteCurrent = _context.OrdineCliente.Where(x => x.Id == Guid.Parse(idOrdineSession)).FirstOrDefault();

                Utility.CheckNull(ordineClienteCurrent);

                //Setto il campo completato, data modifica e utente modifica.
                ordineClienteCurrent.Completato = true;
                ordineClienteCurrent.UtenteModifica = User.Identity.Name;
                ordineClienteCurrent.DataModifica = DateTime.Now;

                //salvo il nuovo record.
                _context.OrdineCliente.Update(ordineClienteCurrent);
                _context.SaveChanges();

                //Invio la mail ai 3 attori
                string emailCliente = _context.Cliente.Where(x => x.Id == ordineClienteCurrent.IdCliente).Select(x => x.Email).FirstOrDefault();

                Execute(ordineClienteCurrent, emailCliente, User.Identity.Name, false).Wait();

                //svuoto la sessione.
                HttpContext.Session.Clear();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString()); throw ex;
            }
        }


        #endregion

        #region Edit

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            try
            {
                if (id == null) return NotFound();

                var ordineCliente = await _context.OrdineCliente.FindAsync(id);

                if (ordineCliente == null) return NotFound();

                ViewData["Id"] = new SelectList(_context.Cliente, "Id", "Email", ordineCliente.Id);
                ViewData["IdPagamento"] = new SelectList(_context.TipoPagamento, "Id", "Nome", ordineCliente.IdTipoPagamento);
                return View(ordineCliente);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind(OC1)] OrdineCliente ordineCliente)
        {
            try
            {
                if (id != ordineCliente.Id) return NotFound();

                Utility.CheckNull(ordineCliente);

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(ordineCliente);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!OrdineClienteExists(ordineCliente.Id)) return NotFound();
                        else Log.Error(ex.ToString()); throw ex;
                    }

                    return RedirectToAction(nameof(Index));
                }

                ViewData["Id"] = new SelectList(_context.Cliente, "Id", "Email", ordineCliente.Id);
                ViewData["IdPagamento"] = new SelectList(_context.TipoPagamento, "Id", "Nome", ordineCliente.IdTipoPagamento);
                return View(ordineCliente);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize]
        public async Task<IActionResult> EditRow(Guid id)
        {
            try
            {
                if (id == null) return NotFound();

                var rigaOrdineCliente = await _context.RigaOrdineCliente.FindAsync(id);

                if (rigaOrdineCliente == null) return NotFound();

                EditRigaOrdineClienteViewModel EditRigaOrdineClienteViewModel = GetEditRigaOrdineClienteViewModel(rigaOrdineCliente);

                return View("EditRigaOrdineCliente", EditRigaOrdineClienteViewModel);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString()); throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRow(Guid id, EditRigaOrdineClienteViewModel rigaOrdine)
        {
            try
            {
                if (id == null) return NotFound();

                var rigaOrdineCliente = await _context.RigaOrdineCliente.FindAsync(id);

                if (rigaOrdineCliente == null) return NotFound();

                CopiaRigaOrdineClienteDaViewModel(rigaOrdine, rigaOrdineCliente);

                try
                {
                    _context.Update(rigaOrdineCliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!OrdineClienteExists(rigaOrdineCliente.Id)) return NotFound();
                    else Log.Error(ex.ToString()); throw ex;
                }
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString()); throw ex;
            }
        }

        #endregion

        #region Delete
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            try
            {
                if (id == null) return NotFound();

                var ordineCliente = await _context.OrdineCliente
                    .Include(o => o.IdNavigation)
                    .Include(o => o.IdPagamentoNavigation)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (ordineCliente == null) return NotFound();

                return View(ordineCliente);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize]
        public IActionResult CancellaOrdine()
        {
            try
            {
                string idOrdineSession = HttpContext.Session.GetString("IdOrdine");
                if (idOrdineSession != null && !string.IsNullOrEmpty(idOrdineSession))
                {
                    //Ottengo tutte le righe dell'ordine e le cancello.
                    List<RigaOrdineCliente> listaRigheOrdine = _context.RigaOrdineCliente.Where(x => x.IdOrdine.ToString().ToUpper() == idOrdineSession.ToUpper()).ToList();
                    foreach (RigaOrdineCliente riga in listaRigheOrdine)
                    {
                        _context.RigaOrdineCliente.Remove(riga);
                        _context.SaveChanges();
                    }

                    //Cancello la foto relativa all'accettazione delle condizioni dell'ordine
                    OrdineClienteFoto ordineFoto = _context.OrdineClienteFoto.Where(x => x.IdOrdine == Guid.Parse(idOrdineSession)).FirstOrDefault();
                    if (ordineFoto == null) throw new ArgumentNullException(nameof(ordineFoto));

                    _context.OrdineClienteFoto.Remove(ordineFoto);
                    _context.SaveChanges();

                    //Cancello l'ordine.
                    OrdineCliente ordine = _context.OrdineCliente.Where(x => x.Id.ToString().ToUpper() == idOrdineSession.ToUpper()).FirstOrDefault();
                    _context.OrdineCliente.Remove(ordine);
                    _context.SaveChanges();

                    //Cancello la sessione.
                    HttpContext.Session.Clear();
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var ordineCliente = await _context.OrdineCliente.FindAsync(id);
                _context.OrdineCliente.Remove(ordineCliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString()); throw ex;
            }
        }

        [Authorize]
        public async Task<IActionResult> DeleteRow(Guid? id)
        {
            if (id == null) return NotFound();

            var rigaOrdineCliente = await _context.RigaOrdineCliente.FirstOrDefaultAsync(m => m.Id == id);

            if (rigaOrdineCliente == null) return NotFound();

            EditRigaOrdineClienteViewModel tempRigaOrdineCliente = GetRigaOrdineClienteViewModel(rigaOrdineCliente);

            return View("DeleteRow", tempRigaOrdineCliente);
        }

        [Authorize]
        [HttpPost, ActionName("DeleteRow")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRowConfirmed(Guid id)
        {
            try
            {
                var rigaOrdineCliente = await _context.RigaOrdineCliente.FindAsync(id);
                _context.RigaOrdineCliente.Remove(rigaOrdineCliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString()); throw ex;
            }
        }

        #endregion

        #region UploadFoglioCondizioni
        [Authorize]
        public IActionResult UploadAccettazioneCondizioni(Guid Id)
        {
            try
            {
                var idCliente = _context.OrdineCliente.Where(x => x.Id == Id).Select(x => x.IdCliente).FirstOrDefault();
                var nomeCliente = _context.Cliente.Where(x => x.Id == idCliente).Select(x => x.Nome).FirstOrDefault();
                var dataOrdine = _context.OrdineCliente.Where(x => x.Id == Id).Select(x => x.DataInserimento).FirstOrDefault();
                AccettazioneCondizioni accettazione = new AccettazioneCondizioni()
                {
                    IdOrdine = Id,
                    Cliente = nomeCliente,
                    DataOrdine = dataOrdine.ToString("dd/MM/yyyy")
                };
                var isFotoGiàInserita = _context.OrdineClienteFoto.Where(x => x.IdOrdine == Id).Select(x => x.Foto).FirstOrDefault();
                if (isFotoGiàInserita != null) ViewData["Message"] = "E' già stata caricata una copia dell'accettazione delle condizioni per l'ordine seguente. Se si procede, questa verrà sovrascritta.";

                return View("AccettazioneCondizioni", accettazione);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize]
        [HttpPost, ActionName("UploadAccettazioneCondizioni")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAccettazioneCondizioni(Guid Id, IFormFile Image)
        {
            try
            {
                Utility.CheckNull(Image);
                Utility.CheckNull(Id);

                byte[] p1 = null;
                using (var fs1 = Image.OpenReadStream())
                using (var ms1 = new MemoryStream())
                {
                    fs1.CopyTo(ms1);
                    p1 = ms1.ToArray();
                }

                var fotoArticolo = GetFotoArticolo(Id, p1);

                _context.Add(fotoArticolo);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        #endregion

        #region Altri

        [Authorize]
        async Task Execute(OrdineCliente ordineCliente, string emailCliente, string emailRappresentante, bool isLoft1)
        {
            try
            {
                Utility.CheckNull(ordineCliente);
                Utility.CheckNull(emailCliente);
                Utility.CheckNull(emailRappresentante);

                //var client = new SendGridClient(Environment.GetEnvironmentVariable("SENDGRID_API_KEY", EnvironmentVariableTarget.User));
                var client = new SendGridClient(SendGridApyKey);
                var from = new EmailAddress("zero_meno@outlook.it", "Zero Meno");

                if (isLoft1) from = new EmailAddress("info@loft1.it", "Loft1"); ;

                var tos = new List<EmailAddress>
            {
                from,
                new EmailAddress(emailCliente, emailCliente),
                new EmailAddress(emailRappresentante,emailRappresentante)
            };

                var cliente = _context.Cliente.Where(x => x.Id == ordineCliente.IdCliente).FirstOrDefault();
                var pagamento = _context.TipoPagamento.Where(x => x.Id == ordineCliente.IdTipoPagamento).Select(x => x.Nome).FirstOrDefault();

                var subject = $"Riepilogo ordine: {cliente.Nome}";
                var plainTextContent = $"";

                var html = @"<!DOCTYPE html>
<html>
<head>

  <meta charset=""utf-8"">
  <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
  <title>Ordine ricevuto</title>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <style type=""text/css"">
  /**
   * Google webfonts. Recommended to include the .woff version for cross-client compatibility.
   */
  @media screen {
    @font-face {
      font-family: 'Source Sans Pro';
      font-style: normal;
      font-weight: 400;
      src: local('Source Sans Pro Regular'), local('SourceSansPro-Regular'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff');
    }

    @font-face {
      font-family: 'Source Sans Pro';
      font-style: normal;
      font-weight: 700;
      src: local('Source Sans Pro Bold'), local('SourceSansPro-Bold'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff');
    }
  }

  /**
   * Avoid browser level font resizing.
   * 1. Windows Mobile
   * 2. iOS / OSX
   */
  body,
  table,
  td,
  a {
    -ms-text-size-adjust: 100%; /* 1 */
    -webkit-text-size-adjust: 100%; /* 2 */
  }

  /**
   * Remove extra space added to tables and cells in Outlook.
   */
  table,
  td {
    mso-table-rspace: 0pt;
    mso-table-lspace: 0pt;
  }

  /**
   * Better fluid images in Internet Explorer.
   */
  img {
    -ms-interpolation-mode: bicubic;
  }

  /**
   * Remove blue links for iOS devices.
   */
  a[x-apple-data-detectors] {
    font-family: inherit !important;
    font-size: inherit !important;
    font-weight: inherit !important;
    line-height: inherit !important;
    color: inherit !important;
    text-decoration: none !important;
  }

  /**
   * Fix centering issues in Android 4.4.
   */
  div[style*=""margin: 16px 0;""] {
    margin: 0 !important;
  }

  body {
    width: 100% !important;
    height: 100% !important;
    padding: 0 !important;
    margin: 0 !important;
  }

  /**
   * Collapse table borders to avoid space between cells.
   */
  table {
    border-collapse: collapse !important;
  }

  a {
    color: #1a82e2;
  }

  img {
    height: auto;
    line-height: 100%;
    text-decoration: none;
    border: 0;
    outline: none;
  }
  </style>

</head>
<body style=""background-color: #D2C7BA;"">

  <!-- start preheader -->
  <div class=""preheader"" style=""display: none; max-width: 0; max-height: 0; overflow: hidden; font-size: 1px; line-height: 1px; color: #fff; opacity: 0;"">
    Conferma dell'ordine.
  </div>
  <!-- end preheader -->

  <!-- start body -->
  <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">

    <!-- start logo -->
    <tr>
      <td align=""center"" bgcolor=""#D2C7BA"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end logo -->

    <!-- start hero -->
    <tr>
      <td align=""center"" bgcolor=""#D2C7BA"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 1200px;"">
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 36px 24px 0; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; border-top: 3px solid #d4dadf;"">
              <h1 style=""margin: 0; font-size: 32px; font-weight: 700; letter-spacing: -1px; line-height: 48px;"">Ordine effettuato con successo.</h1>
            </td>
          </tr>
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end hero -->

    <!-- start copy block -->
    <tr>
      <td align=""center"" bgcolor=""#D2C7BA"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 1200px;"">

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
              <p style=""margin: 0;"">Ecco il riepilogo degli articoli ordinati. Restiamo in attesa di ricevere la foto dell'avvenuta accettazione delle condizioni da parte del cliente consumatore.</p>
            </td>
          </tr>
          <!-- end copy -->

          <!-- start receipt table -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
              <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                <tr>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""75%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>Codice</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>Colore</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>Descrizione</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>2XS/40</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>Xs/42</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>S/44</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>M/46</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>L/48</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>Xl/50</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>2XL/52</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>3XL/54</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>VU</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>Tot.</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>€/cad.</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;""><strong>TOT. €</strong></td>
                </tr>";
                var righeOrdine = _context.RigaOrdineCliente.Where(x => x.IdOrdine == ordineCliente.Id).ToList();
                var totale = 0.0;

                foreach (var item in righeOrdine)
                {
                    var articolo = _context.Articolo.Where(x => x.Id == item.IdArticolo).Select(x => x).FirstOrDefault();
                    html +=
                       $@"<tr><td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{articolo.Codice}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{articolo.Colore}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{articolo.Descrizione}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.Xxs}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.Xs}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.S}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.M}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.L}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.Xl}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.Xxl}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.Xxxl}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{item.TagliaUnica}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{(item.Xxs + item.Xs + item.S + item.M + item.L + item.Xl + item.Xxl + item.Xxxl + item.TagliaUnica)}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{articolo.PrezzoVendita}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 24px;"">{ articolo.PrezzoVendita * (item.Xxs + item.Xs + item.S + item.M + item.L + item.Xl + item.Xxl + item.Xxxl + item.TagliaUnica)}</td></tr>";
                    totale += (item.Xxs + item.Xs + item.S + item.M + item.L + item.Xl + item.Xxl + item.Xxxl + item.TagliaUnica) * articolo.PrezzoVendita;
                }

                html += $@"
                <tr>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong>Totale</strong></td>
                  <td align=""left"" width=""100%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong>€</strong></td>
                  <td align=""left"" width=""25%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong>{totale}</strong></td>
                </tr>
              </table>
            </td>
          </tr>
          <!-- end reeipt table -->

        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end copy block -->

    <!-- start receipt address block -->
    <tr>
      <td align=""center"" bgcolor=""#D2C7BA"" valign=""top"" width=""100%"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table align=""center"" bgcolor=""#ffffff"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 1200px;"">
          <tr>
            <td align=""left"" valign=""top"" style=""font-size: 0; border-bottom: 3px solid #d4dadf"">
              <!--[if (gte mso 9)|(IE)]>
              <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
              <tr>
              <td align=""left"" valign=""top"" width=""300"">
              <![endif]-->
              <div style=""display: inline-block; width: 100%; max-width: 50%; min-width: 240px; vertical-align: top;"">
                <table align=""left"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 300px;"">
                    <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>Cliente</strong></p>
                      <p>{cliente.Nome}</p>
                    </td>
                  </tr>      
                 <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>Indirizzo email cliente</strong></p>
                      <p>{cliente.Email}</p>
                    </td>
                  </tr>  
                  <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>Indirizzo di spedizione</strong></p>
                      <p>{cliente.Indirizzo}</p>
                    </td>
                  </tr>
                    <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>Data consegna</strong></p>
                      <p>{ordineCliente.DataConsegna.ToString("dd/MM/yyyy")}</p>
                    </td>
                  </tr>
                    <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>Metodo pagamento</strong></p>
                      <p>{pagamento}</p>
                    </td>
                  </tr>
                    <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>{(!string.IsNullOrEmpty(ordineCliente.Note) ? "Note" : "")}</strong></p>
                      <p>{(!string.IsNullOrEmpty(ordineCliente.Note) ? ordineCliente.Note : "")}</p>
                    </td>
                  </tr>
                </table>
              </div>
            </td>
          </tr>
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end receipt address block -->

    <!-- start footer -->
    <tr>
      <td align=""center"" bgcolor=""#D2C7BA"" style=""padding: 24px;"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 800px;"">
          <!-- start permission -->
          <tr>
            <td align=""center"" bgcolor=""#D2C7BA"" style=""padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">Ai sensi del D. Lgs. 196/03 e dal regolamento UE 2016/679 per la protezione dei dati personali, questo messaggio è destinato unicamente alla persona o al soggetto al quale è indirizzato e può contenere informazioni riservate e/o coperte da segreto professionale, la cui divulgazione è proibita. Qualora non siate i destinatari designati non dovrete leggere, utilizzare, diffondere o copiare le informazioni trasmesse. Nel caso aveste ricevuto questo messaggio per errore, vogliate cortesemente contattare il mittente e cancellare il materiale dai vostri computer.
             <br>
            According to D. Lgs. 196/03 and by the EU regulation 2016/679 for the protection of personal data, this message is intended only for the person or entity to which it is addressed and may contain confidential and/or privileged information, the disclosure of which is prohibited. If you are not the intended recipient you may not read, use, disseminate or copy the information transmitted. If you have received this message in error, please contact the sender and delete the material from any computer.
            Area degli allegati
            </p>
            </td>
          </tr>
          <!-- end permission -->
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end footer -->
  </table>
  <!-- end body -->
</body>
</html>";

                var showAllRecipients = false; // Set to true if you want the recipients to see each others email addresses

                var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, plainTextContent, html, showAllRecipients);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        #endregion

        #region Dettagli

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Details(Guid? id)
        {
            try
            {
                if (id == null) return NotFound();

                var ordineCliente = await _context.OrdineCliente
                    .Include(o => o.IdNavigation)
                    .Include(o => o.IdPagamentoNavigation)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (ordineCliente == null) return NotFound();

                return View(ordineCliente);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        #endregion

        #region MetodiPrivati
        private void SalvaDatiOrdineClienteInSessione(OrdineClienteViewModel ordineCliente)
        {
            try
            {
                HttpContext.Session.SetString("IdOrdine", ordineCliente.Id.ToString());
                HttpContext.Session.SetString("IdCliente", ordineCliente.IdCliente.ToString());
                HttpContext.Session.SetString("NomeCliente", ordineCliente.NomeCliente);
                HttpContext.Session.SetString("EmailCliente", ordineCliente.EmailCliente);
                HttpContext.Session.SetString("IndirizzoCliente", ordineCliente.IndirizzoCliente);
                HttpContext.Session.SetString("DataConsegna", ordineCliente.DataConsegna.ToString());
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }
        private RigaOrdineCliente GetRigaOrdineCliente(OrdineClienteViewModel ordineCliente, Guid idArticolo)
        {
            try
            {
                return new RigaOrdineCliente()
                {
                    Id = new Guid(),
                    IdOrdine = ordineCliente.Id,
                    IdArticolo = idArticolo,
                    Xxs = ordineCliente.Xxs,
                    Xs = ordineCliente.Xs,
                    S = ordineCliente.S,
                    M = ordineCliente.M,
                    L = ordineCliente.L,
                    Xl = ordineCliente.Xl,
                    Xxl = ordineCliente.Xxl,
                    Xxxl = ordineCliente.Xxxl,
                    TagliaUnica = ordineCliente.TagliaUnica,
                    UtenteInserimento = User.Identity.Name,
                    DataInserimento = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }
        private void CopiaRigaOrdineClienteDaViewModel(EditRigaOrdineClienteViewModel rigaOrdine, RigaOrdineCliente rigaOrdineCliente)
        {
            try
            {
                rigaOrdineCliente.Xxs = rigaOrdine.Xxs;
                rigaOrdineCliente.Xs = rigaOrdine.Xs;
                rigaOrdineCliente.S = rigaOrdine.S;
                rigaOrdineCliente.M = rigaOrdine.M;
                rigaOrdineCliente.L = rigaOrdine.L;
                rigaOrdineCliente.Xl = rigaOrdine.Xl;
                rigaOrdineCliente.Xxl = rigaOrdine.Xxl;
                rigaOrdineCliente.Xxxl = rigaOrdine.Xxxl;
                rigaOrdineCliente.TagliaUnica = rigaOrdine.TagliaUnica;
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }
        private EditRigaOrdineClienteViewModel GetEditRigaOrdineClienteViewModel(RigaOrdineCliente rigaOrdineCliente)
        {
            try
            {
                return new EditRigaOrdineClienteViewModel()
                {
                    CodiceArticolo = _context.Articolo.Where(x => x.Id == rigaOrdineCliente.IdArticolo).Select(x => x.Codice).FirstOrDefault(),
                    Colore = _context.Articolo.Where(x => x.Id == rigaOrdineCliente.IdArticolo).Select(x => x.Colore).FirstOrDefault(),
                    IdRiga = rigaOrdineCliente.Id,
                    Xxs = rigaOrdineCliente.Xxs,
                    Xs = rigaOrdineCliente.Xs,
                    S = rigaOrdineCliente.S,
                    M = rigaOrdineCliente.M,
                    L = rigaOrdineCliente.L,
                    Xl = rigaOrdineCliente.Xl,
                    Xxl = rigaOrdineCliente.Xxl,
                    Xxxl = rigaOrdineCliente.Xxxl,
                    TagliaUnica = rigaOrdineCliente.TagliaUnica
                };
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }
        private IActionResult GetMetodiPagamentoIdonei(string idOrdineSession, double? sommaPrezzo, ref IEnumerable<TipoPagamento> listaPagamenti)
        {
            try
            {
                Utility.CheckNull(listaPagamenti);

                if (sommaPrezzo.Value < 2000) listaPagamenti = _context.TipoPagamento.Where(x => (x.Codice == 4 || x.Codice == 6 || x.Codice == 7)).AsEnumerable();
                if (sommaPrezzo.Value >= 2000 && sommaPrezzo.Value < 5000) listaPagamenti = _context.TipoPagamento.Where(x => x.Codice != 1).AsEnumerable();

                ViewData["TipoPagamento"] = new SelectList(listaPagamenti, "Id", "Nome");
                return View(_context.OrdineCliente.Where(x => x.Id == new Guid(idOrdineSession)).FirstOrDefault());
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }
        private bool OrdineClienteExists(Guid id) => _context.OrdineCliente.Any(e => e.Id == id);
        private bool IsRappresentante() => User.IsInRole("Rappresentante");
        private Guid GetOrCreateIdCliente(OrdineClienteViewModel ordineCliente, string NomeCliente, string EmailCliente, Guid idCliente)
        {
            try
            {
                //Ottengo o creo il nuovo cliente
                if (Utility.ClienteExists(EmailCliente, NomeCliente, _context)) idCliente = _context.Cliente.Where(x => x.Nome.ToUpper() == ordineCliente.NomeCliente.ToUpper() && x.Email.ToUpper() == ordineCliente.EmailCliente.ToUpper()).Select(x => x.Id).FirstOrDefault();
                else
                {
                    Cliente cliente = new Cliente() { Id = idCliente, Email = ordineCliente.EmailCliente, Indirizzo = ordineCliente.IndirizzoCliente, Nome = ordineCliente.NomeCliente };
                    _context.Cliente.Add(cliente);
                    _context.SaveChanges();
                }

                return idCliente;
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }
        private EditRigaOrdineClienteViewModel GetRigaOrdineClienteViewModel(RigaOrdineCliente rigaOrdineCliente)
        {
            try
            {
                return new EditRigaOrdineClienteViewModel()
                {
                    IdRiga = rigaOrdineCliente.Id,
                    CodiceArticolo = _context.Articolo.Where(x => x.Id == rigaOrdineCliente.IdArticolo).Select(x => x.Codice).FirstOrDefault(),
                    Colore = _context.Articolo.Where(x => x.Id == rigaOrdineCliente.IdArticolo).Select(x => x.Colore).FirstOrDefault(),
                    Xxs = rigaOrdineCliente.Xxs,
                    Xs = rigaOrdineCliente.Xs,
                    S = rigaOrdineCliente.S,
                    M = rigaOrdineCliente.M,
                    L = rigaOrdineCliente.L,
                    Xl = rigaOrdineCliente.Xl,
                    Xxl = rigaOrdineCliente.Xxl,
                    Xxxl = rigaOrdineCliente.Xxxl,
                    TagliaUnica = rigaOrdineCliente.TagliaUnica
                };
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }
        private OrdineClienteFoto GetFotoArticolo(Guid Id, byte[] p1)
        {
            try
            {
                return new OrdineClienteFoto()
                {
                    Id = Guid.NewGuid(),
                    Foto = p1,
                    IdOrdine = Id
                };
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        #endregion

        #region MetodiLatoClient

        public IActionResult SelectCodiciArticoli(DateTime dataconsegna)
        {
            try
            {
                var listaArticoli = _context.Articolo.Where(x => x.TrancheConsegna <= dataconsegna && x.Annullato == false).Select(x => x.Codice.ToUpper()).Distinct().ToArray();
                return Json(listaArticoli);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }

        public IActionResult SelectColoriFromCodice(string codice)
        {
            try
            {
                var listaColori = _context.Articolo.Where(x => x.Codice.ToLower() == codice.ToLower() && x.Annullato == false).Select(x => new { Colore = x.Colore }).ToList();
                return Json(listaColori);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }

        public IActionResult SelectDescrizioneFromCodice(string codice)
        {
            try
            {
                string descrizione = _context.Articolo.Where(x => x.Codice.ToLower() == codice.ToLower()).Select(x => x.Descrizione).FirstOrDefault();
                return Json(descrizione);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }

        public IActionResult GetUnreadOrders()
        {
            try
            {
                int result = _context.ViewOrdineClienteCommesso.Where(x => x.Letto == false).ToList().Count;
                return Json(result);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }

        #endregion

    }
}
