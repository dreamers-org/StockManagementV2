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
using StockManagement.Models.ViewModels;

namespace StockManagement.Controllers
{
    public class ArticoloController : Controller
    {
        private readonly StockV2Context _context;

        public ArticoloController(StockV2Context context)
        {
            _context = context;
        }

        [Authorize(Roles = "SuperAdmin, Commesso, Titolare")]
        public async Task<IActionResult> Index()
        {
            var stockV2Context = _context.Articolo.Where(x => x.Annullato == false).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation);
            return View(await stockV2Context.ToListAsync());
        }

        [Authorize(Roles = "SuperAdmin, Commesso, Titolare")]
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

        [Authorize(Roles = "SuperAdmin, Commesso, Titolare")]
        public IActionResult Create()
        {
            ViewData["IdCollezione"] = new SelectList(_context.Collezione, "Id", "Nome");
            ViewData["IdFornitore"] = new SelectList(_context.Fornitore, "Id", "Nome");
            ViewData["IdTipo"] = new SelectList(_context.Tipo, "Id", "Nome");
            return View();
        }

        [Authorize(Roles = "SuperAdmin, Commesso, Titolare")]
        public IActionResult CreateAnnullamento(Guid id)
        {
            var articolo = _context.Articolo.Where(x => x.Id == id && x.Annullato == false).FirstOrDefault();
            var articoloDaAnnullare = new ArticoloAnnullatoViewModel()
            {
                Codice = articolo.Codice,
                Colore = articolo.Colore,
                isXxsToBeNullified = !articolo.isXxsActive,
                isXsToBeNullified = !articolo.isXsActive,
                isSToBeNullified = !articolo.isSActive,
                isMToBeNullified = !articolo.isMActive,
                isLToBeNullified = !articolo.isLActive,
                isXlToBeNullified = !articolo.isXlActive,
                isXxlToBeNullified = !articolo.isXxlActive,
                isXxxlToBeNullified = !articolo.isXxxlActive,
                isTagliaUnicaToBeNullified = !articolo.isTagliaUnicaActive,
                isAllToBeNullified = articolo.Annullato
            };
            return View("CreateAnnullamento", articoloDaAnnullare);
        }

        [Authorize(Roles = "SuperAdmin, Commesso, Titolare")]
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

        [Authorize(Roles = "SuperAdmin, Commesso, Titolare")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAnnullamento(ArticoloAnnullatoViewModel articoloAnnullato)
        {
            if (ModelState.IsValid)
            {
                //if (ArticoloExists(articoloAnnullato.Codice, articoloAnnullato.Colore))
                //{
                //    HttpContext.Session.SetString("ErrorMessage", "L'articolo esiste già.");
                //    return RedirectToAction("Create");
                //}
                if (!ArticoloExists(articoloAnnullato.Codice, articoloAnnullato.Colore))
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var articolo = _context.Articolo.Where(x => x.Codice == articoloAnnullato.Codice && x.Colore == articoloAnnullato.Colore && x.Annullato == false).FirstOrDefault();
                    if (articoloAnnullato.isAllToBeNullified)
                    {
                        articolo.isXxsActive = false;
                        articolo.isXsActive = false;
                        articolo.isSActive = false;
                        articolo.isMActive = false;
                        articolo.isLActive = false;
                        articolo.isXlActive = false;
                        articolo.isXxlActive = false;
                        articolo.isXxxlActive = false;
                        articolo.isTagliaUnicaActive = false;
                        articolo.Annullato = true;
                    }
                    else
                    {
                        articolo.isXxsActive = !articoloAnnullato.isXxsToBeNullified;
                        articolo.isXsActive = !articoloAnnullato.isXsToBeNullified;
                        articolo.isSActive = !articoloAnnullato.isSToBeNullified;
                        articolo.isMActive = !articoloAnnullato.isMToBeNullified;
                        articolo.isLActive = !articoloAnnullato.isLToBeNullified;
                        articolo.isXlActive = !articoloAnnullato.isXlToBeNullified;
                        articolo.isXxlActive = !articoloAnnullato.isXxlToBeNullified;
                        articolo.isXxxlActive = !articoloAnnullato.isXxxlToBeNullified;
                        articolo.isTagliaUnicaActive = !articoloAnnullato.isTagliaUnicaToBeNullified;
                    }

                    Log.Warning($"{User.Identity.Name} --> annullato articolo: {articolo.Codice}");
                    _context.Update(articolo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

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
                    Articolo old = _context.Articolo.AsNoTracking().Where(x => x.Id == articolo.Id).FirstOrDefault();
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

        [Authorize(Roles = "SuperAdmin, Commesso, Titolare")]
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

        public async Task<IActionResult> Ordina(string Parametro)
        {
            IOrderedQueryable<Articolo> context;
            switch (Parametro)
            {
                case "Data":
                    context = _context.Articolo.Where(x => x.Annullato == false).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation).OrderByDescending(x => x.DataInserimento);
                    return View("Index", await context.ToListAsync());
                case "Codice":
                    context = _context.Articolo.Where(x => x.Annullato == false).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation).OrderBy(x => x.Codice);
                    return View("Index", await context.ToListAsync());
                case "Fornitore":
                    context = _context.Articolo.Where(x => x.Annullato == false).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation).OrderBy(x => x.IdFornitoreNavigation.Nome);
                    return View("Index", await context.ToListAsync());
                default:
                    context = _context.Articolo.Where(x => x.Annullato == false).Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation).OrderByDescending(x => x.DataInserimento);
                    return View("Index", await context.ToListAsync());
            }
        }

        #region MetodiLatoCliente

        public ActionResult getTaglieDisponibili(string Codice, string Colore)
        {
            TaglieNonAttiveArticolo result = new TaglieNonAttiveArticolo();
            result.isArticoloValido = false;
            if (ArticoloExists(Codice, Colore))
            {
                Articolo articolo = _context.Articolo.Where(x => x.Codice == Codice && x.Annullato == false && x.Colore == Colore).FirstOrDefault();
                if (articolo != null)
                {
                    result.isArticoloValido = true;
                    result.Xxs = !(articolo.Xxs && articolo.isXxsActive);
                    result.Xs = !(articolo.Xs && articolo.isXsActive);
                    result.S = !(articolo.S && articolo.isSActive);
                    result.M = !(articolo.M && articolo.isMActive);
                    result.L = !(articolo.L && articolo.isLActive);
                    result.Xl = !(articolo.Xl && articolo.isXlActive);
                    result.Xxl = !(articolo.Xxl && articolo.isXxlActive);
                    result.Xxxl = !(articolo.Xxxl && articolo.isXxxlActive);
                    result.TagliaUnica = !(articolo.TagliaUnica && articolo.isTagliaUnicaActive);
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
            public bool isArticoloValido { get; set; }
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
