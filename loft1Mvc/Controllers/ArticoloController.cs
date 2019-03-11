using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Controllers
{
	[Authorize(Roles = "Commesso,Titolare,SuperAdmin")]
	public class ArticoloController : Controller
    {
        private readonly StockV2Context _context;

        public ArticoloController(StockV2Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stockV2Context = _context.Articolo.Include(a => a.IdCollezioneNavigation).Include(a => a.IdFornitoreNavigation).Include(a => a.IdTipoNavigation);
            return View(await stockV2Context.ToListAsync());
        }

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

        public IActionResult Create()
        {
            ViewData["IdCollezione"] = new SelectList(_context.Collezione, "Id", "Nome");
            ViewData["IdFornitore"] = new SelectList(_context.Fornitore, "Id", "Nome");
            ViewData["IdTipo"] = new SelectList(_context.Tipo, "Id", "Nome");
            return View();
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Codice,Descrizione,IdFornitore,Colore,Xxs,Xs,S,M,L,Xl,Xxl,TagliaUnica,TrancheConsegna,Genere,IdTipo,Annullato,PrezzoAcquisto,PrezzoVendita,Foto,Video,IdCollezione,DataInserimento,DataModifica,UtenteInserimento,UtenteModifica,Xxxl")] Articolo articolo)
        {
            if (id != articolo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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

        private bool ArticoloExists(Guid id)
        {
            return _context.Articolo.Any(e => e.Id == id);
        }


		private bool ArticoloExists(string codice, string colore)
		{
			return _context.Articolo.Any(e => e.Codice == codice && e.Colore == colore);
		}

		public async Task<IActionResult> getTxtValues(string Codice)
		{
			TempObject result = new TempObject();
			var articolo = await _context.Articolo
				.FirstOrDefaultAsync(m => m.Codice == Codice && m.Annullato == false);
			if (articolo != null)
			{
				Fornitore fornitore = await _context.Fornitore.FindAsync(articolo.IdFornitore);
				Tipo tipo = await _context.Tipo.FindAsync(articolo.IdTipo);
				Collezione collezione = await _context.Collezione.FindAsync(articolo.IdCollezione);
				result = new TempObject
				{
					Fornitore = fornitore.Nome,
					Descrizione = articolo.Descrizione,
					PrezzoAcquisto = articolo.PrezzoAcquisto.ToString(),
					PrezzoVendita = articolo.PrezzoVendita.ToString(),
					TrancheConsegna = articolo.TrancheConsegna.ToString("yyyy-MM-dd"),
					GenereProdotto = articolo.Genere,
					TipoProdotto = tipo.Nome,
					Collezione = collezione.Nome,
					IdFornitore = articolo.IdFornitore.ToString(),
					IdCollezione = articolo.IdCollezione.ToString(),
					IdTipoProdotto = articolo.IdTipo.ToString()
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
		}


        #region MetodiLatoCliente

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

        #endregion
    }
}
