using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Controllers
{
    [Authorize(Roles = "Commesso,Titolare,SuperAdmin")]
    public class PackingListController : Controller
    {
        #region Costanti&Readonly
        private const string PL = "Id,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,Xxxxl,TagliaUnica,DataInserimento,UtenteInserimento";
        private const string SuperAdmin = "SuperAdmin";
        private readonly StockV2Context _context;
        #endregion

        public PackingListController(StockV2Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.ViewPackingList.OrderBy(x => x.Codice).ToListAsync());
        }

        [Authorize(Roles = SuperAdmin)]
        public async Task<IActionResult> Details(Guid? id)
        {
            try
            {
                if (id == null) return NotFound();

                var packingList = await _context.PackingList.FirstOrDefaultAsync(m => m.Id == id);

                return View(packingList);
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
        public async Task<IActionResult> Create(string Codice, string Colore, [Bind(PL)] PackingList packingList)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(Codice) || string.IsNullOrEmpty(Codice)) return View(packingList);

                    Guid _idArticolo = _context.Articolo.Where(x => x.Codice == Codice && x.Colore == Colore).Select(x => x.Id).FirstOrDefault();
                    packingList.DataInserimento = DateTime.Now;
                    packingList.Id = Guid.NewGuid();
                    packingList.IdArticolo = _idArticolo;
                    packingList.UtenteInserimento = User.Identity.Name;
                    _context.Add(packingList);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(packingList);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize(Roles = "Commessa,SuperAdmin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            try
            {
                if (id == null) return NotFound();

                var packingList = await _context.PackingList.FindAsync(id);

                if (packingList == null) return NotFound();

                var codice = _context.Articolo.Where(x => x.Id == packingList.IdArticolo).Select(x => x.Codice).FirstOrDefault();
                var colore = _context.Articolo.Where(x => x.Id == packingList.IdArticolo).Select(x => x.Colore).FirstOrDefault();
                ViewData["IdPackingList"] = packingList.Id.ToString();
                ViewData["Colore"] = colore;
                ViewData["Codice"] = codice;
                return View("Create", packingList);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }

        [Authorize(Roles = SuperAdmin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind(PL)] PackingList packingList)
        {
            try
            {
                if (id != packingList.Id) return NotFound();

                if (ModelState.IsValid)
                {
                    try
                    {
                        packingList.IdArticolo = _context.PackingList.Where(x => x.Id == id).Select(x => x.IdArticolo).FirstOrDefault();
                        packingList.DataInserimento = DateTime.Now;
                        packingList.UtenteInserimento = User.Identity.Name;
                        _context.Update(packingList);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!PackingListExists(packingList.Id)) return NotFound();
                        Utility.GestioneErrori(User.Identity.Name, ex);
                        throw;
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(packingList);
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize(Roles = "SuperAdmin, Commessa")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            try
            {
                if (id == null) return NotFound();

                var packingList = await _context.PackingList.FindAsync(id);
                _context.PackingList.Remove(packingList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        [Authorize(Roles = SuperAdmin)]
        [HttpPost, ActionName("SuperAdmin, Commessa")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var packingList = await _context.PackingList.FindAsync(id);
                _context.PackingList.Remove(packingList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(User.Identity.Name, ex);
                throw;
            }
        }

        private bool PackingListExists(Guid id)
        {
            return _context.PackingList.Any(e => e.Id == id);
        }
    }
}
