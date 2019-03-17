﻿using System;
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

namespace StockManagement
{
    [Authorize]
	public class OrdineClienteController : Controller
    {
        private readonly StockV2Context _context;
        private readonly IdentityContext _identityContext;


        public OrdineClienteController(StockV2Context context, IdentityContext identityContext)
        {
            _context = context;
            _identityContext = identityContext;

        }

        public async Task<IActionResult> Index()
        {
            //ottengo l'id del rappresentante
            Guid idRappresentante = _identityContext.Users.Where(utente => utente.Email == User.Identity.Name).Select(utente => new Guid(utente.Id)).First();

            //restituisco la vista contenente gli ordini filtrati per idRappresentante
            return View(await _context.ViewOrdineCliente.Where(x => x.IdRappresentante == idRappresentante).ToListAsync());
        }

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

        public IActionResult Create()
        {

            IEnumerable<SelectListItem> i = _context.Articolo.Select(x => new SelectListItem
            {
                Value = x.Codice,
                Text = x.Codice
            }).Distinct().AsEnumerable();

            //i.Append(new SelectListItem { Value = "Selezionare un codice", Text = "Selezionare un codice", Selected = true });     

            ViewData["CodiceArticolo"] = new SelectList(i, "Value", "Text");
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
            }

            if (idOrdineSession != null && !String.IsNullOrEmpty(idOrdineSession))    
            {
                double sommaPrezzo = _context.ViewOrdineCliente.Where(x => x.Id == new Guid(idOrdineSession)).Select(x => x.SommaPrezzo).FirstOrDefault();
                ViewBag.SommaPrezzo = sommaPrezzo;
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
                RigaOrdineCliente rigaOrdineCliente = new RigaOrdineCliente() { Id = new Guid(), IdOrdine = ordineCliente.Id, IdArticolo = idArticolo, Xxs = (ordineCliente.Xxs.HasValue ? ordineCliente.Xxs.Value : 0 ) , Xs = (ordineCliente.Xs.HasValue ? ordineCliente.Xs.Value : 0), S = (ordineCliente.S.HasValue ? ordineCliente.S.Value : 0), M = (ordineCliente.M.HasValue ? ordineCliente.M.Value : 0), L = (ordineCliente.L.HasValue ? ordineCliente.L.Value : 0), Xl = (ordineCliente.Xl.HasValue ? ordineCliente.Xl.Value : 0), Xxl = (ordineCliente.Xxl.HasValue ? ordineCliente.Xxl.Value : 0), Xxxl = (ordineCliente.Xxxl.HasValue ? ordineCliente.Xxxl.Value : 0), UtenteInserimento = User.Identity.Name, DataInserimento = DateTime.Now };

                if (rigaOrdineClienteEsistente != null && rigaOrdineClienteEsistente.Count > 0)
                {
                    _context.RigaOrdineCliente.Update(rigaOrdineCliente);
                }
                else
                {
                    _context.RigaOrdineCliente.Add(rigaOrdineCliente);
                }

                _context.SaveChanges();


                //salvo in sessioni i valori.
                HttpContext.Session.SetString("IdOrdine", ordineCliente.Id.ToString());
                HttpContext.Session.SetString("IdCliente", ordineCliente.IdCliente.ToString());
                HttpContext.Session.SetString("NomeCliente", ordineCliente.NomeCliente);
                HttpContext.Session.SetString("EmailCliente", ordineCliente.EmailCliente);
                HttpContext.Session.SetString("IndirizzoCliente", ordineCliente.IndirizzoCliente);
                HttpContext.Session.SetString("DataConsegna", ordineCliente.DataConsegna.ToString());

            }

            return RedirectToAction("Create");
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

        public IActionResult ImpostaTipoPagamento()
        {
            string idOrdineSession = HttpContext.Session.GetString("IdOrdine");

            double sommaPrezzo = _context.ViewOrdineCliente.Where(x => x.Id == new Guid(idOrdineSession)).Select(x => x.SommaPrezzo).FirstOrDefault();
            ViewBag.SommaPrezzo = sommaPrezzo;

            List<TipoPagamento> listaPagamenti = _context.TipoPagamento.ToList();

            if (sommaPrezzo < 2000)
            {
                _context.TipoPagamento.Where(x => (x.Codice == 4 || x.Codice == 3 || x.Codice == 6 || x.Codice == 7)).ToList();
            }

            //TODO: Creare la combo usando la tabella idTipoPagamento
            ViewData["TipoPagamento"] = new SelectList( _context.TipoPagamento, "Id", "Nome");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ImpostaTipoPagamento([Bind("IdTipoPagamento,Note")] OrdineCliente ordineCliente)
        {
            if (ModelState.IsValid)
            {
                string idOrdineSession = HttpContext.Session.GetString("IdOrdine");
                OrdineCliente ordineClienteCurrent = _context.OrdineCliente.Where(x => x.Id == Guid.Parse(idOrdineSession)).First();

                ordineClienteCurrent.IdTipoPagamento = ordineCliente.IdTipoPagamento;
                ordineClienteCurrent.Note = ordineCliente.Note;

                _context.OrdineCliente.Update(ordineClienteCurrent);
                _context.SaveChanges();
            }

            return RedirectToAction("Riepilogo");
        }


        // GET: RiepilogoOrdine
        public IActionResult Riepilogo()
        {
            return View();
        }

        #region MetodiLatoCliente

        public IActionResult SelectColoriFromCodice(string codice)
        {
            var listaColori = _context.Articolo.Where(x => x.Codice == codice)
                                     .Select(x => new
                                     {
                                         Colore = x.Colore
                                     }).ToList();

            return Json(listaColori);
        }

        public IActionResult SelectDescrizioneFromCodice(string codice)
        {
            string descrizione = _context.Articolo.Where(x => x.Codice == codice)
                                     .Select(x => x.Descrizione).FirstOrDefault();
            return Json(descrizione);
        }

        #endregion
    }
}
