using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Controllers
{
	public class ArticoloAnnullatoController : Controller
	{
		private readonly Loft1Context _context;

		public ArticoloAnnullatoController(Loft1Context context)
		{
			_context = context;
		}

		// GET: ArticoloAnnullato
		public async Task<IActionResult> Index()
		{
			return View(await _context.Articoli.Where(x => x.Annullato == true).ToListAsync());
		}


		// GET: ArticoloAnnullato/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: ArticoloAnnullato/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Codice,Colore")] ArticoloAnnullato articoloAnnullato)
		{
			if (ModelState.IsValid)
			{
				if (_context.Articoli.Any(a => a.Codice == articoloAnnullato.Codice))
				{
					var articolo = await _context.Articoli.FirstOrDefaultAsync(a => a.Codice == articoloAnnullato.Codice);
					articolo.Annullato = true;
					_context.Update(articolo);

					//_context.Add(articoloAnnullato);
					await _context.SaveChangesAsync();
					return RedirectToAction(nameof(Index));

					//TODO Controllare anche il colore
					//Oppure se il colore non è settato, fare il retrieve di tutti gli articoli con quel codice e cancellarli tutti
				}
			}
			ViewBag.errorMessage = true;
			ViewBag.errorMessageText = "Errore: articolo non cancellato.";
			return View(articoloAnnullato);
		}

		// GET: ArticoloAnnullato/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var articoloAnnullato = await _context.ArticoliAnnullati.FindAsync(id);
			if (articoloAnnullato == null)
			{
				return NotFound();
			}
			return View(articoloAnnullato);
		}

		// POST: ArticoloAnnullato/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Codice,Colore")] ArticoloAnnullato articoloAnnullato)
		{
			if (id != articoloAnnullato.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(articoloAnnullato);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ArticoloAnnullatoExists(articoloAnnullato.Id))
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
			return View(articoloAnnullato);
		}

		// GET: ArticoloAnnullato/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var articoloAnnullato = await _context.ArticoliAnnullati
				.FirstOrDefaultAsync(m => m.Id == id);
			if (articoloAnnullato == null)
			{
				return NotFound();
			}

			return View(articoloAnnullato);
		}

		// POST: ArticoloAnnullato/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var articoloAnnullato = await _context.ArticoliAnnullati.FindAsync(id);
			_context.ArticoliAnnullati.Remove(articoloAnnullato);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ArticoloAnnullatoExists(int id)
		{
			return _context.ArticoliAnnullati.Any(e => e.Id == id);
		}

		public IActionResult SelectColoriFromId(string id)
		{
			var listaColori = _context.Articoli.Where(x => x.Codice == id)
									 .Select(x => new
									 {
										 Colore = x.Colore
									 }).ToList();

			return Json(listaColori);
		}

		public IActionResult SelectDescrizioneFromId(string id)
		{
			string descrizione = _context.Articoli.Where(x => x.Codice == id)
									 .Select(x => x.Descrizione).FirstOrDefault();
			return Json(descrizione);
		}

	}
}
