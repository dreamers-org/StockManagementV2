using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Controllers
{
    public class OrdineAlFornitoreController : Controller
    {
        private readonly Loft1Context _context;

        public OrdineAlFornitoreController(Loft1Context context)
        {
            _context = context;
        }

        // GET: OrdineAlFornitore
        public async Task<IActionResult> Index()
        {
            return View(await _context.OrdiniAiFornitori.ToListAsync());
        }

        // GET: OrdineAlFornitore/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordineAlFornitore = await _context.OrdiniAiFornitori
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordineAlFornitore == null)
            {
                return NotFound();
            }

            return View(ordineAlFornitore);
        }

        // GET: OrdineAlFornitore/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrdineAlFornitore/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdOrdine,Fornitore,Codice,Descrizione,Colore,PrezzoAcquisto,PrezzoVendita,Xxxs,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,Xxxxl,attr1,attr2")] OrdineAlFornitore ordineAlFornitore)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordineAlFornitore);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ordineAlFornitore);
        }

        // GET: OrdineAlFornitore/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordineAlFornitore = await _context.OrdiniAiFornitori.FindAsync(id);
            if (ordineAlFornitore == null)
            {
                return NotFound();
            }
            return View(ordineAlFornitore);
        }

        // POST: OrdineAlFornitore/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdOrdine,Fornitore,Codice,Descrizione,Colore,PrezzoAcquisto,PrezzoVendita,Xxxs,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,Xxxxl,attr1,attr2")] OrdineAlFornitore ordineAlFornitore)
        {
            if (id != ordineAlFornitore.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordineAlFornitore);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdineAlFornitoreExists(ordineAlFornitore.Id))
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
            return View(ordineAlFornitore);
        }

        // GET: OrdineAlFornitore/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordineAlFornitore = await _context.OrdiniAiFornitori
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordineAlFornitore == null)
            {
                return NotFound();
            }

            return View(ordineAlFornitore);
        }

        // POST: OrdineAlFornitore/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordineAlFornitore = await _context.OrdiniAiFornitori.FindAsync(id);
            _context.OrdiniAiFornitori.Remove(ordineAlFornitore);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdineAlFornitoreExists(int id)
        {
            return _context.OrdiniAiFornitori.Any(e => e.Id == id);
        }
    }
}
