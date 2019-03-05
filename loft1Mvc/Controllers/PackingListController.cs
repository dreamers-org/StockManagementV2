using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using loft1Mvc.Models;

namespace StockManagement.Controllers
{
    public class PackingListController : Controller
    {
        private readonly Loft1Context _context;

        public PackingListController(Loft1Context context)
        {
            _context = context;
        }

        // GET: PackingList
        public async Task<IActionResult> Index()
        {
            return View(await _context.PackingList.ToListAsync());
        }

        // GET: PackingList/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var packingList = await _context.PackingList
                .FirstOrDefaultAsync(m => m.Id == id);
            if (packingList == null)
            {
                return NotFound();
            }

            return View(packingList);
        }

        // GET: PackingList/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PackingList/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Codice,Fornitore,Variante,Xxxs,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,Xxxxl,attr1,attr2")] PackingList packingList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(packingList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(packingList);
        }

        // GET: PackingList/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var packingList = await _context.PackingList.FindAsync(id);
            if (packingList == null)
            {
                return NotFound();
            }
            return View(packingList);
        }

        // POST: PackingList/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codice,Fornitore,Variante,Xxxs,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,Xxxxl,attr1,attr2")] PackingList packingList)
        {
            if (id != packingList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(packingList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackingListExists(packingList.Id))
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
            return View(packingList);
        }

        // GET: PackingList/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var packingList = await _context.PackingList
                .FirstOrDefaultAsync(m => m.Id == id);
            if (packingList == null)
            {
                return NotFound();
            }

            return View(packingList);
        }

        // POST: PackingList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var packingList = await _context.PackingList.FindAsync(id);
            _context.PackingList.Remove(packingList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PackingListExists(int id)
        {
            return _context.PackingList.Any(e => e.Id == id);
        }
    }
}
