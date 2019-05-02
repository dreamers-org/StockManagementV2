using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;
using StockManagement.Models.ViewModels;

namespace StockManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin, Commesso")]
    public class OrdineFornitoreController : Controller
    {
        private readonly StockV2Context _context;

        public OrdineFornitoreController(StockV2Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string orderBy)
        {
            try
            {
                IOrderedQueryable<ViewOrdineFornitoreViewModel> context;
                switch (orderBy)
                {
                    case "Data":
                        HttpContext.Session.SetString("OrderBy", "Data");
                        context = _context.ViewOrdineFornitore.OrderByDescending(x => x.DataInserimento);
                        return View("Index", await context.ToListAsync());
                    case "Codice":
                        HttpContext.Session.SetString("OrderBy", "Codice");
                        context = _context.ViewOrdineFornitore.OrderBy(x => x.Codice);
                        return View("Index", await context.ToListAsync());
                    case "Fornitore":
                        HttpContext.Session.SetString("OrderBy", "Fornitore");
                        context = _context.ViewOrdineFornitore.OrderBy(x => x.Fornitore);
                        return View("Index", await context.ToListAsync());
                    default:
                        var orderByParam = HttpContext.Session.GetString("OrderBy");
                        if (!string.IsNullOrEmpty(orderByParam))
                        {
                            return RedirectToAction(nameof(Index), new { orderBy = orderByParam });
                        }
                        context = _context.ViewOrdineFornitore.OrderByDescending(x => x.DataInserimento);
                        return View("Index", await context.ToListAsync());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Codice, string Colore, string Fornitore, RigaOrdineFornitore rigaOrdineFornitore)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    rigaOrdineFornitore.Id = Guid.NewGuid();
                    rigaOrdineFornitore.IdArticolo = _context.Articolo.Where(x => x.Codice == Codice && x.Colore == Colore).Select(x => x.Id).FirstOrDefault();
                    rigaOrdineFornitore.IdFornitore = _context.Articolo.Where(x => x.Id == rigaOrdineFornitore.IdArticolo).Select(x => x.IdFornitore).FirstOrDefault();
                    rigaOrdineFornitore.UtenteInserimento = User.Identity.Name;
                    rigaOrdineFornitore.DataInserimento = DateTime.Now;
                    _context.Add(rigaOrdineFornitore);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(rigaOrdineFornitore);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            try
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
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, RigaOrdineFornitore rigaOrdineFornitore)
        {
            try
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
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var rigaOrdineFornitore = await _context.RigaOrdineFornitore.FirstOrDefaultAsync(m => m.Id == id);
                if (rigaOrdineFornitore == null)
                {
                    return NotFound();
                }

                return View(rigaOrdineFornitore);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var rigaOrdineFornitore = await _context.RigaOrdineFornitore.FindAsync(id);
                _context.RigaOrdineFornitore.Remove(rigaOrdineFornitore);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool RigaOrdineFornitoreExists(Guid id)
        {
            return _context.RigaOrdineFornitore.Any(e => e.Id == id);
        }
    }
}
