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
    public class CollezioneController : Controller
    {
        private readonly StockV2Context _context;

        public CollezioneController(StockV2Context context)
        {
            _context = context;
        }

        // GET: Collezione
        public async Task<IActionResult> Index()
        {
            return View(await _context.Collezione.ToListAsync());
        }

        // GET: Collezione/Details/5
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

        // GET: Collezione/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Collezione/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: Collezione/Edit/5
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

        // POST: Collezione/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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

        // GET: Collezione/Delete/5
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

        // POST: Collezione/Delete/5
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
