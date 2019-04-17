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
        private readonly StockV2Context _context;
        private readonly IdentityContext _identityContext;

        public OrdineClienteController(StockV2Context context, IdentityContext identityContext)
        {
            _context = context;
            _identityContext = identityContext;
        }

        #region Index
        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Rappresentante"))
            {
                //ottengo l'id del rappresentante
                Guid idRappresentante = _identityContext.Users.Where(utente => utente.Email == User.Identity.Name).Select(utente => new Guid(utente.Id)).First();
                var lista = _context.ViewOrdineCliente.Where(x => x.IdRappresentante == idRappresentante && x.Completato == true).OrderByDescending(x => x.DataInserimento);

                List<ViewOrdineClienteViewModel> listaOrdini = new List<ViewOrdineClienteViewModel>();
                if (lista != null && lista.Count() > 0)
                {
                    listaOrdini = await lista.ToListAsync();
                }
                //restituisco la vista contenente gli ordini filtrati per idRappresentante
                return View(listaOrdini);
            }
            else
            {
                var lista = _context.ViewOrdineCliente.OrderByDescending(x => x.DataInserimento);
                List<ViewOrdineClienteViewModel> listaOrdini = new List<ViewOrdineClienteViewModel>();
                if (lista != null && lista.Count() > 0)
                {
                    listaOrdini = await lista.ToListAsync();
                }
                return View(listaOrdini);
            }
        }



        #endregion

        #region CreazioneOrdine


        //***********************************************
        //***********************************************
        // STEP 1)
        //***********************************************
        //***********************************************
        [Authorize]
        public IActionResult Create()
        {
            ViewData["NomeCliente"] = HttpContext.Session.GetString("NomeCliente");
            ViewData["EmailCliente"] = HttpContext.Session.GetString("EmailCliente");
            ViewData["IndirizzoCliente"] = HttpContext.Session.GetString("IndirizzoCliente");

            string dataConsegnaSess = HttpContext.Session.GetString("DataConsegna");
            if (!string.IsNullOrEmpty(dataConsegnaSess))
            {
                ViewData["DataConsegna"] = DateTime.Parse(dataConsegnaSess);
            }


            IEnumerable<ViewRigaOrdineClienteViewModel> listaRigheOrdineCliente = new List<ViewRigaOrdineClienteViewModel>();

            string idOrdineSession = HttpContext.Session.GetString("IdOrdine");
            if (idOrdineSession != null && !String.IsNullOrEmpty(idOrdineSession))
            {
                listaRigheOrdineCliente = _context.ViewRigaOrdineCliente.Where(x => x.IdOrdine.ToString().ToUpper() == idOrdineSession.ToUpper()).Select(x => x).ToList();

                double? sommaPrezzo = _context.ViewOrdineCliente.Where(x => x.Id == new Guid(idOrdineSession)).Select(x => x.SommaPrezzo).FirstOrDefault();

                if (sommaPrezzo != null)
                {
                    ViewBag.SommaPrezzo = sommaPrezzo.Value;
                }

                var totalePezzi = 0;
                foreach (var item in listaRigheOrdineCliente)
                {
                    totalePezzi += item.Xxs + item.Xs + item.S + item.M + item.L + item.Xl + item.Xxl + item.Xxxl + item.TagliaUnica;
                }

                HttpContext.Session.SetString("TotalePezzi", totalePezzi.ToString());
            }

            ViewBag.ListaOrdini = listaRigheOrdineCliente;

            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,DataConsegna,NomeCliente,IndirizzoCliente,EmailCliente,CodiceArticolo,ColoreArticolo,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,TagliaUnica")] OrdineClienteViewModel ordineCliente)
        {
            if (ModelState.IsValid)
            {
                //creo i nuovi guid per IdCliente e IdOrdine.
                Guid idCliente = Guid.NewGuid();
                ordineCliente.Id = Guid.NewGuid();

                //Controllo se l'Id dell'ordine esiste già in sessione.
                if (!String.IsNullOrEmpty(HttpContext.Session.GetString("IdOrdine")))
                {
                    //Se esiste significa che il rappresentante ha già creato un ordine.
                    idCliente = new Guid(HttpContext.Session.GetString("IdCliente"));
                    ordineCliente.Id = new Guid(HttpContext.Session.GetString("IdOrdine"));

                    //setto la data di modifica e l'utente di modifica.
                    ordineCliente.DataModifica = DateTime.Now;
                    ordineCliente.UtenteModifica = User.Identity.Name;
                }
                else
                {
                    //verifico che i campi siano valorizzati
                    if (!string.IsNullOrEmpty(ordineCliente.NomeCliente) && !string.IsNullOrEmpty(ordineCliente.IndirizzoCliente) && !string.IsNullOrEmpty(ordineCliente.EmailCliente))
                    {
                        //Ottengo o creo il nuovo cliente
                        List<Cliente> clienti = _context.Cliente.Where(clienteQ => clienteQ.Indirizzo.ToUpper() == ordineCliente.IndirizzoCliente.ToUpper() && clienteQ.Nome.ToUpper() == ordineCliente.NomeCliente.ToUpper() && clienteQ.Email.ToUpper() == ordineCliente.EmailCliente.ToUpper()).ToList();
                        if (clienti != null && clienti.Count > 0)
                        {
                            idCliente = clienti[0].Id;
                        }
                        else
                        {
                            Cliente cliente = new Cliente() { Id = idCliente, Email = ordineCliente.EmailCliente, Indirizzo = ordineCliente.IndirizzoCliente, Nome = ordineCliente.NomeCliente };
                            _context.Cliente.Add(cliente);
                            _context.SaveChanges();
                        }

                        //setto le informazioni sull'utente, sul cliente e sul rappresentante.
                        ordineCliente.UtenteInserimento = User.Identity.Name;
                        ordineCliente.DataInserimento = DateTime.Now;
                        ordineCliente.IdCliente = idCliente;
                        ordineCliente.IdRappresentante = _identityContext.Users.Where(utente => utente.Email == User.Identity.Name).Select(utente => new Guid(utente.Id)).First();

                        _context.Add(ordineCliente);
                        _context.SaveChanges();
                    }
                }

                //ottengo l'articolo.
                Guid idArticolo = _context.Articolo.Where(x => x.Colore.ToUpper() == ordineCliente.ColoreArticolo.ToUpper() && x.Codice.ToUpper() == ordineCliente.CodiceArticolo.ToUpper()).Select(x => x.Id).FirstOrDefault();

                //Se esiste già un record lo modifico altrimenti lo creo.
                List<RigaOrdineCliente> rigaOrdineClienteEsistente = _context.RigaOrdineCliente.Where(x => x.IdOrdine == ordineCliente.Id && x.IdArticolo == idArticolo).ToList();
                RigaOrdineCliente rigaOrdineCliente = new RigaOrdineCliente()
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

                if (rigaOrdineClienteEsistente != null && rigaOrdineClienteEsistente.Count > 0)
                {
                    _context.RigaOrdineCliente.Update(rigaOrdineCliente);
                }
                else
                {
                    _context.RigaOrdineCliente.Add(rigaOrdineCliente);
                }

                _context.SaveChanges();


                //salvo in sessione i valori.
                HttpContext.Session.SetString("IdOrdine", ordineCliente.Id.ToString());
                HttpContext.Session.SetString("IdCliente", ordineCliente.IdCliente.ToString());
                HttpContext.Session.SetString("NomeCliente", ordineCliente.NomeCliente);
                HttpContext.Session.SetString("EmailCliente", ordineCliente.EmailCliente);
                HttpContext.Session.SetString("IndirizzoCliente", ordineCliente.IndirizzoCliente);
                HttpContext.Session.SetString("DataConsegna", ordineCliente.DataConsegna.ToString());

            }

            return RedirectToAction("Create");
        }


        //***********************************************
        //***********************************************
        // STEP 2)
        //***********************************************
        //***********************************************

        [Authorize]
        public IActionResult ImpostaTipoPagamento()
        {
            string idOrdineSession = HttpContext.Session.GetString("IdOrdine");

            double? sommaPrezzo = _context.ViewOrdineCliente.Where(x => x.Id == new Guid(idOrdineSession)).Select(x => x.SommaPrezzo).FirstOrDefault();

            if (sommaPrezzo != null)
            {
                ViewBag.SommaPrezzo = sommaPrezzo.Value;
            }

            IEnumerable<TipoPagamento> listaPagamenti = _context.TipoPagamento.AsEnumerable();

            if (sommaPrezzo != null && sommaPrezzo.Value < 2000)
            {
                listaPagamenti = _context.TipoPagamento.Where(x => (x.Codice == 4 || x.Codice == 3 || x.Codice == 6 || x.Codice == 7)).AsEnumerable();
            }

            ViewData["TipoPagamento"] = new SelectList(listaPagamenti, "Id", "Nome");

            return View(_context.OrdineCliente.Where(x => x.Id == new Guid(idOrdineSession)).FirstOrDefault());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ImpostaTipoPagamento([Bind("IdTipoPagamento,Note")] OrdineCliente ordineCliente)
        {
            if (ModelState.IsValid)
            {
                string idOrdineSession = HttpContext.Session.GetString("IdOrdine");
                OrdineCliente ordineClienteCurrent = _context.OrdineCliente.Where(x => x.Id == Guid.Parse(idOrdineSession)).First();

                ordineClienteCurrent.UtenteModifica = User.Identity.Name;
                ordineClienteCurrent.DataModifica = DateTime.Now;
                ordineClienteCurrent.IdTipoPagamento = ordineCliente.IdTipoPagamento;
                ordineClienteCurrent.Note = ordineCliente.Note;

                _context.OrdineCliente.Update(ordineClienteCurrent);
                _context.SaveChanges();
            }

            return RedirectToAction("Riepilogo");
        }


        //***********************************************
        //***********************************************
        // STEP 3)
        //***********************************************
        //***********************************************

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
            if (condizioniAccettate)
            {
                string idOrdineSession = HttpContext.Session.GetString("IdOrdine");
                OrdineCliente ordineClienteCurrent = _context.OrdineCliente.Where(x => x.Id == Guid.Parse(idOrdineSession)).FirstOrDefault();

                //Setto il campo completato, data modifica e utente modifica.
                ordineClienteCurrent.Completato = true;
                ordineClienteCurrent.UtenteModifica = User.Identity.Name;
                ordineClienteCurrent.DataModifica = DateTime.Now;

                //salvo il nuovo record.
                _context.OrdineCliente.Update(ordineClienteCurrent);
                _context.SaveChanges();

                //Invio la mail ai 3 attori
                var emailCliente = _context.Cliente.Where(x => x.Id == ordineClienteCurrent.IdCliente).Select(x => x.Email).FirstOrDefault();
                Execute(ordineClienteCurrent, emailCliente, User.Identity.Name, false).Wait();
                //TODO DA MODIFICARE il false

                //svuoto la sessione.
                HttpContext.Session.Clear();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        #endregion

        #region Altro
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordineCliente = await _context.OrdineCliente
                .Include(o => o.IdNavigation)
                .Include(o => o.IdPagamentoNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordineCliente == null)
            {
                return NotFound();
            }

            return View(ordineCliente);
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordineCliente = await _context.OrdineCliente.FindAsync(id);
            if (ordineCliente == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_context.Cliente, "Id", "Email", ordineCliente.Id);
            ViewData["IdPagamento"] = new SelectList(_context.TipoPagamento, "Id", "Nome", ordineCliente.IdTipoPagamento);
            return View(ordineCliente);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,IdRappresentante,IdCliente,DataConsegna,IdPagamento,Note,Completato,Pagato,DataInserimento,DataModifica,UtenteInserimento,UtenteModifica")] OrdineCliente ordineCliente)
        {
            if (id != ordineCliente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordineCliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdineClienteExists(ordineCliente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id"] = new SelectList(_context.Cliente, "Id", "Email", ordineCliente.Id);
            ViewData["IdPagamento"] = new SelectList(_context.TipoPagamento, "Id", "Nome", ordineCliente.IdTipoPagamento);
            return View(ordineCliente);
        }

        [Authorize]
        public async Task<IActionResult> EditRow(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rigaOrdineCliente = await _context.RigaOrdineCliente.FindAsync(id);
            if (rigaOrdineCliente == null)
            {
                return NotFound();
            }
            var EditRigaOrdineClienteViewModel = new EditRigaOrdineClienteViewModel()
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
            return View("EditRigaOrdineCliente", EditRigaOrdineClienteViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRow(Guid id, EditRigaOrdineClienteViewModel rigaOrdine)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rigaOrdineCliente = await _context.RigaOrdineCliente.FindAsync(id);
            if (rigaOrdineCliente == null)
            {
                return NotFound();
            }
            rigaOrdineCliente.Xxs = rigaOrdine.Xxs;
            rigaOrdineCliente.Xs = rigaOrdine.Xs;
            rigaOrdineCliente.S = rigaOrdine.S;
            rigaOrdineCliente.M = rigaOrdine.M;
            rigaOrdineCliente.L = rigaOrdine.L;
            rigaOrdineCliente.Xl = rigaOrdine.Xl;
            rigaOrdineCliente.Xxl = rigaOrdine.Xxl;
            rigaOrdineCliente.Xxxl = rigaOrdine.Xxxl;
            rigaOrdineCliente.TagliaUnica = rigaOrdine.TagliaUnica;

            try
            {
                _context.Update(rigaOrdineCliente);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdineClienteExists(rigaOrdineCliente.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Create));
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordineCliente = await _context.OrdineCliente
                .Include(o => o.IdNavigation)
                .Include(o => o.IdPagamentoNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordineCliente == null)
            {
                return NotFound();
            }

            return View(ordineCliente);
        }

        [Authorize]
        public IActionResult CancellaOrdine()
        {
            string idOrdineSession = HttpContext.Session.GetString("IdOrdine");
            if (idOrdineSession != null && !String.IsNullOrEmpty(idOrdineSession))
            {
                //Ottengo tutte le righe dell'ordine e le cancello.
                List<RigaOrdineCliente> listaRigheOrdine = _context.RigaOrdineCliente.Where(x => x.IdOrdine.ToString().ToUpper() == idOrdineSession.ToUpper()).ToList();
                foreach (RigaOrdineCliente riga in listaRigheOrdine)
                {
                    _context.RigaOrdineCliente.Remove(riga);
                    _context.SaveChanges();
                }

                //Cancello l'ordine.
                OrdineCliente ordine = _context.OrdineCliente.Where(x => x.Id.ToString().ToUpper() == idOrdineSession.ToUpper()).FirstOrDefault();
                _context.OrdineCliente.Remove(ordine);
                _context.SaveChanges();

                //Cancello la sessione.
                HttpContext.Session.Clear();
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var ordineCliente = await _context.OrdineCliente.FindAsync(id);
            _context.OrdineCliente.Remove(ordineCliente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdineClienteExists(Guid id)
        {
            return _context.OrdineCliente.Any(e => e.Id == id);
        }

        [Authorize]
        public async Task<IActionResult> DeleteRow(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rigaOrdineCliente = await _context.RigaOrdineCliente.FirstOrDefaultAsync(m => m.Id == id);
            if (rigaOrdineCliente == null)
            {
                return NotFound();
            }

            EditRigaOrdineClienteViewModel tempRigaOrdineCliente = new EditRigaOrdineClienteViewModel()
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

            return View("DeleteRow", tempRigaOrdineCliente);
        }

        [Authorize]
        [HttpPost, ActionName("DeleteRow")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRowConfirmed(Guid id)
        {
            var rigaOrdineCliente = await _context.RigaOrdineCliente.FindAsync(id);
            _context.RigaOrdineCliente.Remove(rigaOrdineCliente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Create));
        }

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
                if (isFotoGiàInserita != null)
                {
                    ViewData["Message"] = "E' già stata caricata una copia dell'accettazione delle condizioni per l'ordine seguente. Se si procede, questa verrà sovrascritta.";
                }
                return View("AccettazioneCondizioni", accettazione);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
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
                if (Image != null)
                {
                    byte[] p1 = null;
                    using (var fs1 = Image.OpenReadStream())
                    using (var ms1 = new MemoryStream())
                    {
                        fs1.CopyTo(ms1);
                        p1 = ms1.ToArray();
                    }
                    var fotoArticolo = new OrdineClienteFoto()
                    {
                        Id = Guid.NewGuid(),
                        Foto = p1,
                        IdOrdine = Id
                    };
                    _context.Add(fotoArticolo);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        [Authorize]
        async Task Execute(OrdineCliente ordineCliente, string emailCliente, string emailRappresentante, bool isLoft1)
        {
            var client = new SendGridClient("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");
            var from = new EmailAddress("zero_meno@outlook.it", "Zero Meno");
            if (isLoft1)
            {
                from = new EmailAddress("info@loft1.it", "Loft1"); ;
            }
            var tos = new List<EmailAddress>
            {
                from,
                new EmailAddress(emailCliente, emailCliente),
                new EmailAddress(emailRappresentante,emailRappresentante)
            };

            var cliente = _context.Cliente.Where(x => x.Id == ordineCliente.IdCliente).FirstOrDefault();
            var pagamento = _context.TipoPagamento.Where(x => x.Id == ordineCliente.IdTipoPagamento).Select(x => x.Nome).FirstOrDefault();

            var subject = $"Riepilogo ordine " + ordineCliente.Id;
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
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""75%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;""><strong>Codice #</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;""><strong></strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;""><strong>Colore</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;""><strong>Descrizione</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;""><strong>Xxs/40</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;""><strong>Xs/42</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;""><strong>S/44</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;""><strong>M/46</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;""><strong>L/48</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;""><strong>Xl/50</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;""><strong>Xxl/52</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;""><strong>Xxxl/54</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;""><strong>VU</strong></td>
                  <td align=""left"" bgcolor=""#D2C7BA"" width=""25%"" style=""padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;""><strong>Prezzo</strong></td>
                </tr>";
            var righeOrdine = _context.RigaOrdineCliente.Where(x => x.IdOrdine == ordineCliente.Id).ToList();
            var totale = 0.0;

            foreach (var item in righeOrdine)
            {
                var articolo = _context.Articolo.Where(x => x.Id == item.IdArticolo).Select(x => x).FirstOrDefault();
                html +=
                   $@"<tr><td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{articolo.Codice}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;""></td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{articolo.Colore}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{articolo.Descrizione}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{item.Xxs}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{item.Xs}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{item.S}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{item.M}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{item.L}</td>
                  <td align=""left"" width=""75%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{item.Xl}</td>
                  <td align=""left"" width=""25%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{item.Xxl}</td>
                  <td align=""left"" width=""25%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{item.Xxxl}</td>
                  <td align=""left"" width=""25%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{item.TagliaUnica}</td>
                  <td align=""left"" width=""25%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{articolo.PrezzoVendita}</td></tr>";
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
                  <td align=""left"" width=""25%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong>€ {totale}</strong></td>
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
                      <p><strong>{(string.IsNullOrEmpty(cliente.Nome) ? "Cliente" : "")}</strong></p>
                      <p>{(string.IsNullOrEmpty(cliente.Nome) ? cliente.Nome : "")}</p>
                    </td>
                  </tr>      
                 <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>{(string.IsNullOrEmpty(cliente.Email) ? "Indirizzo email cliente" : "")}</strong></p>
                      <p>{(string.IsNullOrEmpty(cliente.Email) ? cliente.Email : "")}</p>
                    </td>
                  </tr>  
                    <tr>
                    <td align=""left"" valign=""top"" style=""padding-bottom: 6px; padding-left: 6px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
                      <p><strong>{(string.IsNullOrEmpty(cliente.Email) ? "Indirizzo email cliente" : "")}</strong></p>
                      <p>{(string.IsNullOrEmpty(cliente.Email) ? cliente.Email : "")}</p>
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
                      <p><strong>{(string.IsNullOrEmpty(ordineCliente.Note) ? "Note" : "")}</strong></p>
                      <p>{(string.IsNullOrEmpty(ordineCliente.Note) ? ordineCliente.Note : "")}</p>
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

        #endregion

        #region MetodiLatoClient

        public IActionResult SelectCodiciArticoli(DateTime dataconsegna)
        {
            var listaArticoli = _context.Articolo.Where(x => x.TrancheConsegna <= dataconsegna && x.Annullato == false).Select(x => x.Codice).Distinct().ToArray();
            return Json(listaArticoli);
        }

        public IActionResult SelectColoriFromCodice(string codice)
        {
            var listaColori = _context.Articolo.Where(x => x.Codice == codice && x.Annullato == false).Select(x => new { Colore = x.Colore }).ToList();
            return Json(listaColori);
        }

        public IActionResult SelectDescrizioneFromCodice(string codice)
        {
            string descrizione = _context.Articolo.Where(x => x.Codice == codice).Select(x => x.Descrizione).FirstOrDefault();
            return Json(descrizione);
        }

        public IActionResult GetUnreadOrders()
        {
            int result = _context.ViewOrdineClienteCommesso.Where(x => x.Letto == false).ToList().Count;
            return Json(result);
        }


        #endregion
    }
}
