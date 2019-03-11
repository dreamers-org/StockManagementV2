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
	[Authorize(Roles = "Commesso,Titolare,SuperAdmin")]
	public class FornitoreController : Controller
    {
        private readonly StockV2Context _context;

        public FornitoreController(StockV2Context context)
        {
            _context = context;
        }

        // GET: Fornitore
        public async Task<IActionResult> Index()
        {
            return View(await _context.Fornitore.ToListAsync());
        }

        // GET: Fornitore/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fornitore = await _context.Fornitore
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fornitore == null)
            {
                return NotFound();
            }

            return View(fornitore);
        }

        // GET: Fornitore/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fornitore/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome")] Fornitore fornitore)
        {
            if (ModelState.IsValid)
            {
                fornitore.Id = Guid.NewGuid();
                _context.Add(fornitore);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fornitore);
        }

        // GET: Fornitore/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fornitore = await _context.Fornitore.FindAsync(id);
            if (fornitore == null)
            {
                return NotFound();
            }
            return View(fornitore);
        }

        // POST: Fornitore/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nome")] Fornitore fornitore)
        {
            if (id != fornitore.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fornitore);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FornitoreExists(fornitore.Id))
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
            return View(fornitore);
        }

        // GET: Fornitore/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fornitore = await _context.Fornitore
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fornitore == null)
            {
                return NotFound();
            }

            return View(fornitore);
        }

        // POST: Fornitore/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var fornitore = await _context.Fornitore.FindAsync(id);
            _context.Fornitore.Remove(fornitore);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FornitoreExists(Guid id)
        {
            return _context.Fornitore.Any(e => e.Id == id);
        }
    }
}
