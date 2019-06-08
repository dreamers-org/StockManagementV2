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
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Codice, string Colore, RigaOrdineFornitore rigaOrdineFornitore)
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
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            try
            {
                if (id == null) return NotFound();

                var rigaOrdineFornitore = await _context.RigaOrdineFornitore.FindAsync(id);

                if (rigaOrdineFornitore == null) return NotFound();

                ViewData["Id"] = new SelectList(_context.Articolo, "Id", "Codice", rigaOrdineFornitore.Id);
               
                return View(rigaOrdineFornitore);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, RigaOrdineFornitore rigaOrdineFornitore)
        {
            try
            {
                if (!RigaOrdineFornitoreExists(id)) return NotFound();

                if (ModelState.IsValid)
                {
                    try
                    {
                        RigaOrdineFornitore rigaOrdineFornitoreOld = _context.RigaOrdineFornitore.Where(x => x.Id == id).FirstOrDefault();

                        rigaOrdineFornitoreOld.UtenteModifica = User.Identity.Name;
                        rigaOrdineFornitoreOld.DataModifica = DateTime.Now;
                        rigaOrdineFornitoreOld.Xxs = rigaOrdineFornitore.Xxs;
                        rigaOrdineFornitoreOld.Xs = rigaOrdineFornitore.Xs;
                        rigaOrdineFornitoreOld.S = rigaOrdineFornitore.S;
                        rigaOrdineFornitoreOld.M = rigaOrdineFornitore.M;
                        rigaOrdineFornitoreOld.L = rigaOrdineFornitore.L;
                        rigaOrdineFornitoreOld.Xl = rigaOrdineFornitore.Xl;
                        rigaOrdineFornitoreOld.Xxl = rigaOrdineFornitore.Xxl;
                        rigaOrdineFornitoreOld.Xxxl = rigaOrdineFornitore.Xxxl;
                        rigaOrdineFornitoreOld.Xxxxl = rigaOrdineFornitore.Xxxxl;
                        rigaOrdineFornitoreOld.TagliaUnica = rigaOrdineFornitore.TagliaUnica;
                        _context.Update(rigaOrdineFornitoreOld);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RigaOrdineFornitoreExists(rigaOrdineFornitore.Id)) return NotFound();
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == null) return NotFound();
                if (!RigaOrdineFornitoreExists(id)) return NotFound();

                var rigaOrdineFornitore = await _context.RigaOrdineFornitore.FindAsync(id);
                _context.RigaOrdineFornitore.Remove(rigaOrdineFornitore);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        private bool RigaOrdineFornitoreExists(Guid id)
        {
            try
            {
                return _context.RigaOrdineFornitore.Any(e => e.Id == id);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }
    }
}
