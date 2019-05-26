using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using StockManagement.Models;
using StockManagement.Models.ViewModels;

namespace StockManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin, Titolare, Commesso")]
    public class OrdineClienteCommessoController : Controller
    {
        private readonly StockV2Context _context;
        private readonly IdentityContext _identityContext;
        private readonly IConfiguration _configuration;

        public OrdineClienteCommessoController(StockV2Context context, IdentityContext identityContext, IConfiguration Configuration)
        {
            _context = context;
            _identityContext = identityContext;
            _configuration = Configuration;
        }

        public async Task<IActionResult> Index()
        {
            var stockV2Context = _context.ViewOrdineClienteCommesso.OrderByDescending(x => x.DataInserimento).ToListAsync();

            return View(await stockV2Context);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            HttpContext.Session.Clear();

            var ordineCliente = await _context.ViewRigaOrdineClienteCommesso.Where(m => m.IdOrdine == id).ToListAsync();

            if (ordineCliente == null) return NotFound();

            var riepilogoOrdine = _context.ViewOrdineClienteRiepilogoBreve.Where(m => m.Id == id).FirstOrDefault();

            int totalePezzi = 0;

            foreach (var item in ordineCliente)
            {
                totalePezzi += item.TotalePezzi;
            }

            ViewData["TotalePrezzoOrdine"] = ordineCliente.Select(x => x.TotaleRiga).ToList().Sum();
            ViewData["TotalePezzi"] = totalePezzi;
            ViewData["RiepilogoOrdine"] = riepilogoOrdine;
            ViewData["idOrdine"] = id.ToString();

            return View(ordineCliente);
        }

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
            ViewData["IdTipoPagamento"] = new SelectList(_context.TipoPagamento, "Id", "Nome", ordineCliente.IdTipoPagamento);
            return View(ordineCliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, string DataConsegna, string IdTipoPagamento, string Note, bool Completato, bool Pagato, bool Spedito, bool SpeditoInParte, bool Letto, bool Stampato)
        {
            OrdineCliente ordineCliente = new OrdineCliente();
            if (ModelState.IsValid)
            {
                try
                {
                    ordineCliente = _context.OrdineCliente.Where(x => x.Id == id).Select(x => x).First();
                    ordineCliente.Note = Note;
                    ordineCliente.DataConsegna = DateTime.Parse(DataConsegna);
                    ordineCliente.IdTipoPagamento = Guid.Parse(IdTipoPagamento);
                    ordineCliente.Completato = Completato;
                    ordineCliente.Pagato = Pagato;
                    ordineCliente.Spedito = Spedito;
                    ordineCliente.SpeditoInParte = SpeditoInParte;
                    ordineCliente.Letto = Letto;
                    ordineCliente.Stampato = Stampato;
                    ordineCliente.UtenteModifica = User.Identity.Name;
                    ordineCliente.DataModifica = DateTime.Now;
                    _context.Update(ordineCliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdineClienteExists(id))
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
            ViewData["IdTipoPagamento"] = new SelectList(_context.TipoPagamento, "Id", "Nome", ordineCliente.IdTipoPagamento);
            return View(ordineCliente);
        }

        private bool OrdineClienteExists(Guid id)
        {
            return _context.OrdineCliente.Any(e => e.Id == id);
        }

        [Authorize]
        public async Task<IActionResult> DeleteRow(Guid id)
        {
            var rigaOrdineCliente = await _context.RigaOrdineCliente.FindAsync(id);
            _context.RigaOrdineCliente.Remove(rigaOrdineCliente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EditOrderRows), new { idOrdine = rigaOrdineCliente.IdOrdine });
        }

        public IActionResult EditOrderRows(string idOrdine)
        {
            Guid idOrdineGuid = new Guid();
            if (!string.IsNullOrEmpty(idOrdine))
            {
                HttpContext.Session.SetString("idOrdine", idOrdine);
                idOrdineGuid = Guid.Parse(idOrdine);
            }
            else
            {
                var idOrdineFromSession = HttpContext.Session.GetString("idOrdine");
                if (idOrdineFromSession != null)
                {
                    idOrdineGuid = Guid.Parse(idOrdineFromSession);
                }
            }
            //Recupero i dati che mi servono a partire dall'idRigaOrdine
            var idCliente = _context.OrdineCliente.Where(x => x.Id == idOrdineGuid).Select(x => x.IdCliente).FirstOrDefault();
            var dataConsegna = _context.OrdineCliente.Where(x => x.Id == idOrdineGuid).Select(x => x.DataConsegna).FirstOrDefault();
            var nomeCliente = _context.Cliente.Where(x => x.Id == idCliente).Select(x => x.Nome).FirstOrDefault();
            var emailCliente = _context.Cliente.Where(x => x.Id == idCliente).Select(x => x.Email).FirstOrDefault();
            var indirizzo = _context.Cliente.Where(x => x.Id == idCliente).Select(x => x.Indirizzo).FirstOrDefault();

            //Setto i valori appena recuperati in sessione
            HttpContext.Session.SetString("NomeCliente", nomeCliente);
            HttpContext.Session.SetString("EmailCliente", emailCliente);
            HttpContext.Session.SetString("IndirizzoCliente", indirizzo);
            HttpContext.Session.SetString("DataConsegna", dataConsegna.ToString());

            //Li setto anche nel ViewData
            ViewData["NomeCliente"] = HttpContext.Session.GetString("NomeCliente");
            ViewData["EmailCliente"] = HttpContext.Session.GetString("EmailCliente");
            ViewData["IndirizzoCliente"] = HttpContext.Session.GetString("IndirizzoCliente");

            string dataConsegnaSess = HttpContext.Session.GetString("DataConsegna");

            if (!string.IsNullOrEmpty(dataConsegnaSess))
            {
                ViewData["DataConsegna"] = DateTime.Parse(dataConsegnaSess);
            }

            IEnumerable<ViewRigaOrdineClienteViewModel> listaRigheOrdineCliente = new List<ViewRigaOrdineClienteViewModel>();

            if (idOrdineGuid != null && !string.IsNullOrEmpty(idOrdineGuid.ToString()))
            {
                listaRigheOrdineCliente = _context.ViewRigaOrdineCliente.Where(x => x.IdOrdine.ToString().ToUpper() == idOrdineGuid.ToString().ToUpper()).ToList();

                double? sommaPrezzo = _context.ViewOrdineCliente.Where(x => x.Id == idOrdineGuid).Select(x => x.SommaPrezzo).FirstOrDefault();

                if (sommaPrezzo != null) ViewBag.SommaPrezzo = sommaPrezzo.Value;

                var totalePezzi = 0;
                foreach (var item in listaRigheOrdineCliente)
                {
                    totalePezzi += item.Xxs + item.Xs + item.S + item.M + item.L + item.Xl + item.Xxl + item.Xxxl + item.Xxxxl + item.TagliaUnica;
                }

                HttpContext.Session.SetString("TotalePezzi", totalePezzi.ToString());
                ViewData["TotalePezzi"] = totalePezzi;
            }

            ViewBag.ListaOrdini = listaRigheOrdineCliente;
            HttpContext.Session.Clear();
            
            return View("ModifyRows");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditOrderRows([Bind("Id,DataConsegna,NomeCliente,IndirizzoCliente,EmailCliente,CodiceArticolo,ColoreArticolo,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl, Xxxxl,TagliaUnica")] OrdineClienteViewModel ordineCliente)
        {
            if (ModelState.IsValid)
            {
                //creo i nuovi guid per IdCliente e IdOrdine.
                Guid idCliente = Guid.NewGuid();
                ordineCliente.Id = Guid.NewGuid();

                //Controllo se l'Id dell'ordine esiste già in sessione.
                var idOrdine = HttpContext.Session.GetString("IdOrdine");
                if (!string.IsNullOrEmpty(idOrdine))
                {
                    //Se esiste significa che il rappresentante ha già creato un ordine.
                    ordineCliente.Id = new Guid(HttpContext.Session.GetString("IdOrdine"));
                    //idCliente = _context.OrdineCliente.Where(x => x.Id == Guid.Parse(idOrdine)).Select(x => x.IdCliente).FirstOrDefault();
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
                    Xxxxl = ordineCliente.Xxxxl,
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

            return RedirectToAction("EditOrderRows");
        }

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

                var articoloPerRicavareCollezione = _context.RigaOrdineCliente.Where(x => x.IdOrdine == ordineClienteCurrent.Id).Select(x => x.IdArticolo).FirstOrDefault();
                var idCollezione = _context.Articolo.Where(x => x.Id == articoloPerRicavareCollezione).Select(x => x.IdCollezione).First();
                var collezione = _context.Collezione.Where(x => x.Id == idCollezione).Select(x => x.Nome).FirstOrDefault();
                var regione = _identityContext.Users.Where(x => x.Email == User.Identity.Name).Select(x => x.Regione).FirstOrDefault();
                Utility.Execute(_configuration, _context, ordineClienteCurrent, emailCliente, User.Identity.Name, collezione, regione).GetAwaiter().GetResult();

                //svuoto la sessione.
                HttpContext.Session.Clear();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
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
                Xxxxl = rigaOrdineCliente.Xxxxl,
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
            rigaOrdineCliente.Xxxxl = rigaOrdine.Xxxxl;
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
            return RedirectToAction(nameof(EditOrderRows), new { idOrdine = rigaOrdineCliente.IdOrdine });
        }

        public IActionResult ViewAccettazioneCondizioni(Guid id)
        {
            var foto = _context.OrdineClienteFoto.Where(x => x.IdOrdine == id).Select(x => x.Foto).FirstOrDefault();
            if (foto != null && foto.Length > 0)
            {
                return View("AccettazioneCondizioni", foto);
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Test()
        {
            
            Statistiche.getTotalePerRappresentanteCompletatoLoft(_context, _identityContext);
            Statistiche.getTotalePerRappresentanteCompletatoZeroMeno(_context, _identityContext);
            return View();
        }
    }
}