using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        public OrdineClienteController(StockV2Context context, IdentityContext identityContext, IConfiguration Configuration)
        {
            _context = context;
            _identityContext = identityContext;
            _configuration = Configuration;
        }

        #region Costanti&Readonly

        private const string OC = "Id,DataConsegna,NomeCliente,IndirizzoCliente,EmailCliente,CodiceArticolo,ColoreArticolo,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,Xxxxl,TagliaUnica";
        private const string OC1 = "Id,IdRappresentante,IdCliente,DataConsegna,IdPagamento,Note,Completato,Pagato,DataInserimento,DataModifica,UtenteInserimento,UtenteModifica";
        private const string NoteOc = "IdTipoPagamento,Note";
        private readonly StockV2Context _context;
        private readonly IdentityContext _identityContext;
        private readonly IConfiguration _configuration;

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

        [Authorize]
        public IActionResult IndexCommesso() {
            return RedirectToAction("Index","OrdineClienteCommesso");
        }
        #endregion

        #region Create
        [Authorize]
        public IActionResult Create()
        {
            try
            {
                HttpContext.Session.Clear();

                ViewData["NomeCliente"] = HttpContext.Session.GetString("NomeCliente");
                ViewData["EmailCliente"] = HttpContext.Session.GetString("EmailCliente");
                ViewData["IndirizzoCliente"] = HttpContext.Session.GetString("IndirizzoCliente");

                string idOrdineSession = HttpContext.Session.GetString("IdOrdine");
                string dataConsegnaSess = HttpContext.Session.GetString("DataConsegna");

                if (!string.IsNullOrEmpty(dataConsegnaSess)) ViewData["DataConsegna"] = DateTime.Parse(dataConsegnaSess);

                Guid idRappresentante = _identityContext.Users.Where(x => x.Email == User.Identity.Name).Select(x => Guid.Parse(x.Id)).FirstOrDefault();
                OrdineCliente ordineCliente = _context.OrdineCliente.Where(x => x.IdRappresentante == idRappresentante).OrderByDescending(x=>x.DataInserimento).FirstOrDefault();

                IEnumerable<ViewRigaOrdineClienteViewModel> listaRigheOrdineCliente = new List<ViewRigaOrdineClienteViewModel>();

                Guid checkGuid = Guid.NewGuid();
                Guid idOrdineAperto = new Guid();

                //Se esiste un ultimo ordine e non è completato, prendo l'id e lo recupero
                if (idOrdineSession == null && ordineCliente != null && ordineCliente.Completato == false)
                {
                    idOrdineAperto = ordineCliente.Id;

                    ViewData["CancellazioneOrdinePrecedenteObbligatoria"] = "E' presente in memoria un ordine iniziato precedentemente e non terminato. Per proseguire occorre cancellarlo, o completarlo.";

                    //Recupero i dati
                    Guid idCliente= ordineCliente.IdCliente;
                    Cliente cliente = _context.Cliente.Where(x => x.Id == idCliente).FirstOrDefault() ;

                    //Metto in sessione i dati
                    HttpContext.Session.SetString("NomeCliente", cliente.Nome);
                    HttpContext.Session.SetString("EmailCliente", cliente.Email);
                    HttpContext.Session.SetString("IndirizzoCliente", cliente.Indirizzo);
                    HttpContext.Session.SetString("DataConsegna", ordineCliente.DataConsegna.ToString());
                    HttpContext.Session.SetString("IdOrdine", ordineCliente.Id.ToString());

                    //Passo i dati al ViewData
                    ViewData["NomeCliente"] = HttpContext.Session.GetString("NomeCliente");
                    ViewData["EmailCliente"] = HttpContext.Session.GetString("EmailCliente");
                    ViewData["IndirizzoCliente"] = HttpContext.Session.GetString("IndirizzoCliente");

                    dataConsegnaSess = HttpContext.Session.GetString("DataConsegna");

                    if (!string.IsNullOrEmpty(dataConsegnaSess)) ViewData["DataConsegna"] = DateTime.Parse(dataConsegnaSess);
                }

                idOrdineSession = idOrdineSession ?? (idOrdineAperto == checkGuid ? null : idOrdineAperto.ToString());

                if (!string.IsNullOrEmpty(idOrdineSession))
                {
                    int totalePezzi = 0;

                    listaRigheOrdineCliente = _context.ViewRigaOrdineCliente.Where(x => x.IdOrdine.ToString().ToUpper() == idOrdineSession.ToUpper()).Select(x => x).ToList();

                    Utility.CheckNull(listaRigheOrdineCliente);

                    if (listaRigheOrdineCliente.Count() == 0)
                    {
                        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Collezione")))
                        {
                            HttpContext.Session.Remove("Collezione");
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Collezione")))
                        {
                            string tempCodiceArticolo = listaRigheOrdineCliente.FirstOrDefault().CodiceArticolo;
                            string collezioneArticolo = _context.Articolo.Where(x => x.Codice == tempCodiceArticolo).Select(x => x.IdCollezioneNavigation.Nome).FirstOrDefault();
                            HttpContext.Session.SetString("Collezione", collezioneArticolo);
                        }
                    }

                    double? sommaPrezzo = _context.ViewOrdineCliente.Where(x => x.Id == new Guid(idOrdineSession)).Select(x => x.SommaPrezzo).FirstOrDefault();

                    if (sommaPrezzo != null)
                    {
                        ViewBag.SommaPrezzo = sommaPrezzo.Value;
                    }

                    foreach (var item in listaRigheOrdineCliente)
                    {
                        totalePezzi += item.Xxs + item.Xs + item.S + item.M + item.L + item.Xl + item.Xxl + item.Xxxl + item.Xxxxl + item.TagliaUnica;
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

                //L'ordine non esiste ancora
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdOrdine")))
                {
                    idCliente = GetOrCreateIdCliente(ordineCliente, NomeCliente, EmailCliente, idCliente);

                    string newRandomNumber = Utility.GetRandomNumber(_context);

                    ordineCliente.RandomNumber = newRandomNumber;

                    //setto le informazioni sull'utente, sul cliente e sul rappresentante.
                    ordineCliente.UtenteInserimento = User.Identity.Name;
                    ordineCliente.DataInserimento = DateTime.Now;
                    ordineCliente.IdCliente = idCliente;
                    ordineCliente.IdRappresentante = _identityContext.Users.Where(x => x.Email == User.Identity.Name).Select(x => new Guid(x.Id)).FirstOrDefault();

                    string collezioneArticolo = _context.Articolo.Where(x => x.Colore.ToUpper() == ordineCliente.ColoreArticolo.ToUpper() && x.Codice.ToUpper() == ordineCliente.CodiceArticolo.ToUpper()).Select(x => x.IdCollezioneNavigation.Nome).FirstOrDefault();
                    HttpContext.Session.SetString("Collezione", collezioneArticolo);

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

                //Controllo la validità della collezione
                string collezione = _context.Articolo.Where(x => x.Colore.ToUpper() == ordineCliente.ColoreArticolo.ToUpper() && x.Codice.ToUpper() == ordineCliente.CodiceArticolo.ToUpper()).Select(x => x.IdCollezioneNavigation.Nome).FirstOrDefault();
                string collezioneSession = HttpContext.Session.GetString("Collezione");

                if (!string.IsNullOrEmpty(collezioneSession))
                {
                    if (collezione != collezioneSession)
                    {
                        TempData["ErroreCollezione"] = "Impossibile inserire articoli di collezioni diverse nello stesso ordine.";
                        return RedirectToAction(nameof(Create)); }
                }
                HttpContext.Session.SetString("Collezione", collezione);

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

                var collezione = HttpContext.Session.GetString("Collezione");
                var regione = _identityContext.Users.Where(x => x.Email == User.Identity.Name).Select(x => x.Regione).FirstOrDefault();
                Utility.Execute(_configuration, _context, ordineClienteCurrent, emailCliente, User.Identity.Name, collezione, regione).Wait();

                //svuoto la sessione.
                HttpContext.Session.Clear();

                return RedirectToAction("VisualizzazioneNumeroOrdine", new { numeroOrdine = ordineClienteCurrent.RandomNumber });
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }

        [Authorize]
        public IActionResult VisualizzazioneNumeroOrdine(string numeroOrdine)
        {
            if (string.IsNullOrEmpty(numeroOrdine)) return NotFound();

            ViewData["numeroOrdine"] = numeroOrdine;

            return View("NumeroOrdine");
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
                    if (ordineFoto != null)
                    {
                        _context.OrdineClienteFoto.Remove(ordineFoto);
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

        #region Dettagli

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var idRappresentante = _identityContext.Users.Where(x => x.Email == User.Identity.Name).Select(x => x.Id).First();

            var listaRigheOrdineCliente = await _context.ViewRigaOrdineClienteCommesso.Where(m => m.IdOrdine == id).ToListAsync();

            var idRappresentanteOrdine = _context.OrdineCliente.Where(x => x.Id == id).Select(x => x.IdRappresentante).FirstOrDefault();

            if (listaRigheOrdineCliente == null || Guid.Parse(idRappresentante) != idRappresentanteOrdine) return NotFound();

            var riepilogoOrdine = _context.ViewOrdineClienteRiepilogoBreve.Where(m => m.Id == id).FirstOrDefault();
            ViewData["TotalePrezzoOrdine"] = listaRigheOrdineCliente.Select(x => x.TotaleRiga).ToList().Sum();
            ViewData["RiepilogoOrdine"] = riepilogoOrdine;
            return View(listaRigheOrdineCliente);
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
                    Xxxxl = ordineCliente.Xxxxl,
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
                rigaOrdineCliente.Xxxxl = rigaOrdine.Xxxxl;
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
                    Xxxxl = rigaOrdineCliente.Xxxxl,
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
                    Xxxxl = rigaOrdineCliente.Xxxxl,
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
                var listaColori = _context.Articolo.Where(x => x.Codice.ToLower() == codice.ToLower() && x.Annullato == false && x.Colore.ToLower().IndexOf("disporre") == -1).Select(x => new { Colore = x.Colore }).ToList();
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
