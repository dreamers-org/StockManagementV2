using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class OrdineFornitoreController : Controller
    {
        private readonly StockV2Context _context;

        public OrdineFornitoreController(StockV2Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stockV2Context = _context.RigaOrdineFornitore.Include(r => r.IdNavigation);
            return View(await stockV2Context.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rigaOrdineFornitore = await _context.RigaOrdineFornitore
                .Include(r => r.IdNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rigaOrdineFornitore == null)
            {
                return NotFound();
            }

            return View(rigaOrdineFornitore);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RigaOrdineFornitore rigaOrdineFornitore)
        {
            if (ModelState.IsValid)
            {
                rigaOrdineFornitore.Id = Guid.NewGuid();
                _context.Add(rigaOrdineFornitore);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rigaOrdineFornitore);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rigaOrdineFornitore = await _context.RigaOrdineFornitore.FindAsync(id);
            if (rigaOrdineFornitore == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_context.Articolo, "Id", "Codice", rigaOrdineFornitore.Id);
            return View(rigaOrdineFornitore);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, RigaOrdineFornitore rigaOrdineFornitore)
        {
            if (id != rigaOrdineFornitore.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rigaOrdineFornitore);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RigaOrdineFornitoreExists(rigaOrdineFornitore.Id))
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
            ViewData["Id"] = new SelectList(_context.Articolo, "Id", "Codice", rigaOrdineFornitore.Id);
            return View(rigaOrdineFornitore);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rigaOrdineFornitore = await _context.RigaOrdineFornitore
                .Include(r => r.IdNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rigaOrdineFornitore == null)
            {
                return NotFound();
            }

            return View(rigaOrdineFornitore);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var rigaOrdineFornitore = await _context.RigaOrdineFornitore.FindAsync(id);
            _context.RigaOrdineFornitore.Remove(rigaOrdineFornitore);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RigaOrdineFornitoreExists(Guid id)
        {
            return _context.RigaOrdineFornitore.Any(e => e.Id == id);
        }
    }
}
