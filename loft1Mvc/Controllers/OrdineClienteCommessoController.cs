using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;
using StockManagement.Models.ViewModels;

namespace StockManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin, Titolare, Commesso")]
    public class OrdineClienteCommessoController : Controller
    {
        private readonly StockV2Context _context;
        private readonly IdentityContext _identityContext;

        public OrdineClienteCommessoController(StockV2Context context, IdentityContext identityContext)
        {
            _context = context;
            _identityContext = identityContext;
        }

        public async Task<IActionResult> Index()
        {
            var stockV2Context = _context.ViewOrdineClienteCommesso;
            return View(await stockV2Context.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordineCliente = await _context.ViewRigaOrdineClienteCommesso.Where(m => m.IdOrdine == id).ToListAsync();
            if (ordineCliente == null)
            {
                return NotFound();
            }

            return View(ordineCliente);
        }

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
            }

            ViewBag.ListaOrdini = listaRigheOrdineCliente;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,DataConsegna,NomeCliente,IndirizzoCliente,EmailCliente,CodiceArticolo,ColoreArticolo,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl")] OrdineClienteViewModel ordineCliente)
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
                RigaOrdineCliente rigaOrdineCliente = new RigaOrdineCliente() {
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
                    UtenteInserimento = User.Identity.Name,
                    DataInserimento = DateTime.Now };

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
        public async Task<IActionResult> Edit(Guid id, string DataConsegna, string IdTipoPagamento, string Note, bool Completato,bool Pagato, bool Spedito, bool SpeditoInParte, bool Letto, bool Stampato)
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

        public IActionResult AddRow(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddRow(Guid id, EditRigaOrdineClienteViewModel rigaOrdine)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var rigaOrdineCliente = await _context.RigaOrdineCliente.FindAsync(id);
        //    if (rigaOrdineCliente == null)
        //    {
        //        return NotFound();
        //    }
        //    rigaOrdineCliente.Xxs = rigaOrdine.Xxs;
        //    rigaOrdineCliente.Xs = rigaOrdine.Xs;
        //    rigaOrdineCliente.S = rigaOrdine.S;
        //    rigaOrdineCliente.M = rigaOrdine.M;
        //    rigaOrdineCliente.L = rigaOrdine.L;
        //    rigaOrdineCliente.Xl = rigaOrdine.Xl;
        //    rigaOrdineCliente.Xxl = rigaOrdine.Xxl;
        //    rigaOrdineCliente.Xxxl = rigaOrdine.Xxxl;
        //    rigaOrdineCliente.TagliaUnica = rigaOrdine.TagliaUnica;

        //    try
        //    {
        //        _context.Update(rigaOrdineCliente);
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!OrdineClienteExists(rigaOrdineCliente.Id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //    return RedirectToAction(nameof(Index));
        //}


        public IActionResult ViewAccettazioneCondizioni(Guid id)
        {
            var foto = _context.OrdineClienteFoto.Where(x => x.IdOrdine == id).Select(x => x.Foto).FirstOrDefault();
            if (foto != null && foto.Length > 0)
            {
                return View("AccettazioneCondizioni", foto);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}