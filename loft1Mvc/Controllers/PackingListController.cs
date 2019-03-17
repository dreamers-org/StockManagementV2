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
    public class PackingListController : Controller
    {
        private readonly StockV2Context _context;

        public PackingListController(StockV2Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.ViewPackingList.ToListAsync());
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Details(Guid? id)
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Codice, string Colore, [Bind("Id,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,TagliaUnica,DataInserimento,UtenteInserimento")] PackingList packingList)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(Codice) || string.IsNullOrEmpty(Codice))
                {
                    //Errore
                    return View(packingList);
                }
                else
                {
                    Guid _idArticolo = _context.Articolo.Where(x => x.Codice == Codice && x.Colore == Colore).Select(x => x.Id).FirstOrDefault();
                    packingList.DataInserimento = DateTime.Now;
                    packingList.Id = Guid.NewGuid();
                    packingList.IdArticolo = _idArticolo;
                    packingList.UtenteInserimento = User.Identity.Name;
                    _context.Add(packingList);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(packingList);
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Edit(Guid? id)
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

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,TagliaUnica,DataInserimento,UtenteInserimento")] PackingList packingList)
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

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(Guid? id)
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

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var packingList = await _context.PackingList.FindAsync(id);
            _context.PackingList.Remove(packingList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PackingListExists(Guid id)
        {
            return _context.PackingList.Any(e => e.Id == id);
        }
    }
}
