using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Controllers
{
	public class CollezioneController : Controller
    {
        private readonly StockV2Context _context;

        public CollezioneController(StockV2Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Collezione.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collezione = await _context.Collezione
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collezione == null)
            {
                return NotFound();
            }

            return View(collezione);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome")] Collezione collezione)
        {
            if (ModelState.IsValid)
            {
                collezione.Id = Guid.NewGuid();
                _context.Add(collezione);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(collezione);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collezione = await _context.Collezione.FindAsync(id);
            if (collezione == null)
            {
                return NotFound();
            }
            return View(collezione);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nome")] Collezione collezione)
        {
            if (id != collezione.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(collezione);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollezioneExists(collezione.Id))
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
            return View(collezione);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collezione = await _context.Collezione
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collezione == null)
            {
                return NotFound();
            }

            return View(collezione);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var collezione = await _context.Collezione.FindAsync(id);
            _context.Collezione.Remove(collezione);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollezioneExists(Guid id)
        {
            return _context.Collezione.Any(e => e.Id == id);
        }
    }
}
