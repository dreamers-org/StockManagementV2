using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Core;
using StockManagement.Models;

namespace StockManagement.Controllers
{
    public class ArticoloController : Controller
    {
        private readonly StockV2Context _context;

        public ArticoloController(StockV2Context context)
        {
            _context = context;
        }

        [Authorize(Roles = "SuperAdmin,Commesso, Titolare")]
        public async Task<IActionResult> Index()
        {
            var stockV2Context = _context.Articolo.Where(x => x.Annullato == false).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation);
            return View(await stockV2Context.ToListAsync());
        }


        [Authorize(Roles = "SuperAdmin,Commesso, Titolare")]
        public async Task<IActionResult> IndexAnnullati()
        {
            var stockV2Context = _context.Articolo.Where(x => x.Annullato == true).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation);
            return View(await stockV2Context.ToListAsync());
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articolo = await _context.Articolo
                .Include(a => a.IdCollezioneNavigation)
                .Include(a => a.IdFornitoreNavigation)
                .Include(a => a.IdTipoNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (articolo == null)
            {
                return NotFound();
            }

            return View(articolo);
        }


        [Authorize(Roles = "SuperAdmin,Commesso, Titolare")]
        public IActionResult Create()
        {
            ViewData["IdCollezione"] = new SelectList(_context.Collezione, "Id", "Nome");
            ViewData["IdFornitore"] = new SelectList(_context.Fornitore, "Id", "Nome");
            ViewData["IdTipo"] = new SelectList(_context.Tipo, "Id", "Nome");
            return View();
        }

        [Authorize(Roles = "SuperAdmin,Commesso, Titolare")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Codice,Descrizione,IdFornitore,Colore,Xxs,Xs,S,M,L,Xl,Xxl,TagliaUnica,TrancheConsegna,Genere,IdTipo,PrezzoAcquisto,PrezzoVendita,IdCollezione,Xxxl")] Articolo articolo)
        {
            if (ModelState.IsValid)
            {
                if (ArticoloExists(articolo.Codice, articolo.Colore))
                {
                    HttpContext.Session.SetString("ErrorMessage", "L'articolo esiste già.");
                    return RedirectToAction("Create");
                }
                articolo.Id = Guid.NewGuid();
                articolo.UtenteInserimento = User.Identity.Name;
                articolo.DataModifica = DateTime.Now;
                articolo.Foto = "";
                articolo.Video = "";
                _context.Add(articolo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCollezione"] = new SelectList(_context.Collezione, "Id", "Nome", articolo.IdCollezione);
            ViewData["IdFornitore"] = new SelectList(_context.Fornitore, "Id", "Nome", articolo.IdFornitore);
            ViewData["IdTipo"] = new SelectList(_context.Tipo, "Id", "Nome", articolo.IdTipo);
            return View();
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articolo = await _context.Articolo.FindAsync(id);
            if (articolo == null)
            {
                return NotFound();
            }
            ViewData["IdCollezione"] = new SelectList(_context.Collezione, "Id", "Nome", articolo.IdCollezione);
            ViewData["IdFornitore"] = new SelectList(_context.Fornitore, "Id", "Nome", articolo.IdFornitore);
            ViewData["IdTipo"] = new SelectList(_context.Tipo, "Id", "Nome", articolo.IdTipo);
            return View(articolo);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Codice,Descrizione,IdFornitore,Colore,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,TagliaUnica,TrancheConsegna,Genere,IdTipo,PrezzoAcquisto,PrezzoVendita,IdCollezione")] Articolo articolo)
        {
            if (id != articolo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Articolo old = _context.Articolo.AsNoTracking().Where(x => x.Id == articolo.Id).ToList().FirstOrDefault();
                    articolo.DataInserimento = old.DataInserimento;
                    articolo.DataModifica = DateTime.Now;
                    articolo.UtenteInserimento = old.UtenteInserimento;
                    articolo.UtenteModifica = User.Identity.Name;
                    articolo.Annullato = old.Annullato;
                    articolo.Foto = "";
                    articolo.Video = "";
                    //Se cambio alcuni dati per un codice devo cambiarli per tutti gli articoli con lo stesso codice ---> errori dipendenze FK se fatto a posteriori!!!!!!
                    //if (old.Descrizione != articolo.Descrizione)
                    //{
                    //   var articoliStessoCodice =  _context.Articolo.Where(x => x.Descrizione == articolo.Descrizione).ToList();
                    //    foreach (var item in articoliStessoCodice)
                    //    {
                    //        item.Descrizione = articolo.Descrizione;
                    //    }
                    //}
                    Log.Warning($"{User.Identity.Name} --> modificato articolo: {articolo.Codice}");
                    _context.Update(articolo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticoloExists(articolo.Id))
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
            ViewData["IdCollezione"] = new SelectList(_context.Collezione, "Id", "Nome", articolo.IdCollezione);
            ViewData["IdFornitore"] = new SelectList(_context.Fornitore, "Id", "Nome", articolo.IdFornitore);
            ViewData["IdTipo"] = new SelectList(_context.Tipo, "Id", "Nome", articolo.IdTipo);
            return View(articolo);
        }


        [Authorize(Roles = "Commesso, Titolare, SuperAdmin")]
        public async Task<IActionResult> Annulla(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articolo = await _context.Articolo.FindAsync(id);
            if (articolo == null)
            {
                return NotFound();
            }
            articolo.Annullato = true;

            _context.Update(articolo);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private bool ArticoloExists(Guid id)
        {
            return _context.Articolo.Any(e => e.Id == id);
        }

        private bool ArticoloExists(string codice, string colore)
        {
            return _context.Articolo.Any(e => e.Codice == codice && e.Colore == colore);
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articolo = await _context.Articolo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (articolo == null)
            {
                return NotFound();
            }

            return View(articolo);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var articolo = await _context.Articolo.FindAsync(id);
            _context.Articolo.Remove(articolo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region MetodiLatoCliente
        public ActionResult getTaglieDisponibili(string Codice, string Colore)
        {
            TaglieNonAttiveArticolo result = new TaglieNonAttiveArticolo();
            Articolo articolo = _context.Articolo.Where(x => x.Codice == Codice && x.Annullato == false && x.Colore == Colore).FirstOrDefault();
            if (articolo != null)
            {
                result = new TaglieNonAttiveArticolo
                {
                    Xxs = !(articolo.Xxs && articolo.isXxsActive),
                    Xs = !(articolo.Xs && articolo.isXsActive),
                    S = !(articolo.S && articolo.isSActive),
                    M = !(articolo.M && articolo.isMActive),
                    L = !(articolo.L && articolo.isLActive),
                    Xl = !(articolo.Xl && articolo.isXlActive),
                    Xxl = !(articolo.Xxl && articolo.isXxlActive),
                    Xxxl = !(articolo.Xxxl && articolo.isXxxlActive),
                    TagliaUnica = !(articolo.TagliaUnica && articolo.isTagliaUnicaActive)
                };
            }
            return Json(result);
        }

        public async Task<bool> verifyCorrectness(string Codice, string Colore)
        {
            var articolo = await _context.Articolo
                .FirstOrDefaultAsync(m => m.Codice == Codice && m.Annullato == false && m.Colore == Colore);
            if (articolo == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IActionResult SelectColoriFromCodice(string codice)
        {
            var listaColori = _context.Articolo.Where(x => x.Codice == codice)
                                     .Select(x => new
                                     {
                                         Colore = x.Colore
                                     }).ToList();

            return Json(listaColori);
        }

        public IActionResult SelectDescrizioneFromCodice(string codice)
        {
            string descrizione = _context.Articolo.Where(x => x.Codice == codice)
                                     .Select(x => x.Descrizione).FirstOrDefault();
            return Json(descrizione);
        }

        protected class TempObject
        {
            public string Fornitore { get; set; }
            public string IdFornitore { get; set; }
            public string Descrizione { get; set; }
            public string PrezzoAcquisto { get; set; }
            public string PrezzoVendita { get; set; }
            public string TrancheConsegna { get; set; }
            public string GenereProdotto { get; set; }
            public string TipoProdotto { get; set; }
            public string IdTipoProdotto { get; set; }
            public string Collezione { get; set; }
            public string IdCollezione { get; set; }
            public bool XXS { get; set; }
            public bool XS { get; set; }
            public bool S { get; set; }
            public bool M { get; set; }
            public bool L { get; set; }
            public bool XL { get; set; }
            public bool XXL { get; set; }
            public bool XXXL { get; set; }
            public bool TagliaUnica { get; set; }
        }

        protected class TaglieNonAttiveArticolo
        {
            public bool Xxs { get; set; }
            public bool Xs { get; set; }
            public bool S { get; set; }
            public bool M { get; set; }
            public bool L { get; set; }
            public bool Xl { get; set; }
            public bool Xxl { get; set; }
            public bool Xxxl { get; set; }
            public bool TagliaUnica { get; set; }

        }

        #endregion
    }
}
