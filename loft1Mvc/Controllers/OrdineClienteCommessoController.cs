using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin, Titolare, Commesso")]
    public class OrdineClienteCommessoController : Controller
    {
        private readonly StockV2Context _context;

        public OrdineClienteCommessoController(StockV2Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stockV2Context = _context.OrdineCliente;
            return View(await stockV2Context.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordineCliente = await _context.OrdineCliente.FirstOrDefaultAsync(m => m.Id == id);
            if (ordineCliente == null)
            {
                return NotFound();
            }

            return View(ordineCliente);
        }

        public IActionResult Create()
        {
            ViewData["Id"] = new SelectList(_context.Cliente, "Id", "Email");
            ViewData["IdTipoPagamento"] = new SelectList(_context.TipoPagamento, "Id", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdRappresentante,IdCliente,DataConsegna,IdTipoPagamento,Note,Completato,Pagato,DataInserimento,DataModifica,UtenteInserimento,UtenteModifica,Spedito,SpeditoInParte,Letto,Stampato")] OrdineCliente ordineCliente)
        {
            if (ModelState.IsValid)
            {
                ordineCliente.Id = Guid.NewGuid();
                _context.Add(ordineCliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id"] = new SelectList(_context.Cliente, "Id", "Email", ordineCliente.Id);
            ViewData["IdTipoPagamento"] = new SelectList(_context.TipoPagamento, "Id", "Nome", ordineCliente.IdTipoPagamento);
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
    }
}