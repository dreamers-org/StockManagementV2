using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;
using StockManagement.Models.ViewModels;
using System;
using System.Collections.Generic;
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

        // GET: OrdineCliente
        public async Task<IActionResult> Index()
        {
            //ottengo l'id del rappresentante
            Guid idRappresentante = _identityContext.Users.Where(utente => utente.Email == User.Identity.Name).Select(utente => new Guid(utente.Id)).First();

            //List<OrdineCliente> listaOrdini = await _context.OrdineCliente.Where(x => x.IdRappresentante == idRappresentante).Select(x => x).ToListAsync();
            //List<OrdineClienteViewModel> listaOrdiniViewModel = new List<OrdineClienteViewModel>();
            //foreach (OrdineCliente item in listaOrdini)
            //{
            //    OrdineClienteViewModel item2 = (OrdineClienteViewModel)item;

            //    Cliente cliente = _context.Cliente.Where(x => x.Id == item.IdCliente).Select(x => x).FirstOrDefault();
            //    item2.NomeCliente = cliente.Nome;
            //    item2.IndirizzoCliente = cliente.Indirizzo;
            //    item2.EmailCliente = cliente.Email;

            //    listaOrdiniViewModel.Add(item2);
            //}


            _context.
            //restituisco la vista
            return View();
        }

        // GET: OrdineCliente/Details/5
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

        // GET: OrdineCliente/Create
        public IActionResult Create()
        {
            ViewData["CodiceArticolo"] = new SelectList(_context.Articolo, "Codice", "Codice");

            ViewData["NomeCliente"] = HttpContext.Session.GetString("NomeCliente");
            ViewData["EmailCliente"] = HttpContext.Session.GetString("EmailCliente");
            ViewData["IndirizzoCliente"] = HttpContext.Session.GetString("IndirizzoCliente");

            IEnumerable<RigaOrdineCliente> listaRigheOrdineCliente = null;

            string idOrdineSession = HttpContext.Session.GetString("NomeCliente");
            if (idOrdineSession != null && !String.IsNullOrEmpty(idOrdineSession))
            {
                listaRigheOrdineCliente = _context.RigaOrdineCliente.Where(x => x.IdOrdine.ToString().ToUpper() == idOrdineSession.ToUpper()).Select(x => x);
            }


            ViewBag.ListaOrdini = listaRigheOrdineCliente;

            return View();
        }

        // POST: OrdineCliente/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,DataConsegna,NomeCliente,IndirizzoCliente,EmailCliente,CodiceArticolo,ColoreArticolo,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl")] OrdineClienteViewModel ordineCliente)
        {
            if (ModelState.IsValid)
            {
                //creo i nuovi guid per IdCliente e IdOrdine.
                Guid idCliente = new Guid();
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
                RigaOrdineCliente rigaOrdineCliente = new RigaOrdineCliente() { Id = new Guid(), IdOrdine = ordineCliente.Id, IdArticolo = idArticolo, Xxs = ordineCliente.Xxs, Xs = ordineCliente.Xs, S = ordineCliente.S, M = ordineCliente.M, L = ordineCliente.L, Xl = ordineCliente.Xl, Xxl = ordineCliente.Xxl, Xxxl = ordineCliente.Xxxl, UtenteInserimento = User.Identity.Name, DataInserimento = DateTime.Now };

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
            }

            return RedirectToAction("Create");
        }

        // GET: OrdineCliente/Edit/5
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

        // POST: OrdineCliente/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: OrdineCliente/Delete/5
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

        // POST: OrdineCliente/Delete/5
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
    }
}
