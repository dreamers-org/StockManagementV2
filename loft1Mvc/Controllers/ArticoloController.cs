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
	public class ArticoloController : Controller
	{
		private readonly Loft1Context _context;

		public ArticoloController(Loft1Context context)
		{
			_context = context;
		}

		// GET: Articoli
		public async Task<IActionResult> Index()
		{
			return View(await _context.Articoli.Where(x => x.Annullato == false).ToListAsync());
		}

		// GET: Articoli/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var articoli = await _context.Articoli
				.FirstOrDefaultAsync(m => m.Id == id);
			if (articoli == null)
			{
				return NotFound();
			}

			return View(articoli);
		}

		// GET: Articoli/Create
		public IActionResult Create()
		{
			List<string> tipoProdotto = new List<string>();

			tipoProdotto = (from tipo in _context.TipoProdotto
							select tipo.Tipo).ToList();

			ViewBag.tipiProdotto = tipoProdotto;
			return View();
		}

		// POST: Articoli/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Fornitore,Codice,Descrizione,Colore,Xxxs,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,Xxxxl,TrancheConsegna,PrezzoAcquisto,PrezzoVendita,TipoProdotto")] Articolo articolo)
		{
			//TODO Upload immagine
			//TODO Ecco come implementare la sessione
			//_httpContextAccessor.HttpContext.Session.SetString($"{User.Identity.Name}", $"#{id}");
			//TODO gestire claim e comportamento diversificato: se la fra o un rappresentante arrivano in visualizzazione su questa pagina il comportamento non deve essere lo stesso
			//OrdiniDaiClienti = await _context.OrdiniDaiClienti.ToListAsync();
			//OrdiniDaiClienti = (from ordine in OrdiniDaiClienti
			//					where ordine.Rappresentante == User.Identity.Name
			//					select ordine).ToList();
			if (ModelState.IsValid)
			{
				//TODO: Controllare al momento dell'inserimento che la coppia codice-colore non sia già stata inserita in precedenza
				_context.Add(articolo);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(articolo);
		}

		// GET: Articoli/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var articoli = await _context.Articoli.FindAsync(id);
			if (articoli == null)
			{
				return NotFound();
			}
			return View(articoli);
		}

		// POST: Articoli/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Fornitore,Codice,Descrizione,Colore,Xxxs,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,Xxxxl,TrancheConsegna,PrezzoAcquisto,PrezzoVendita")] Articolo articolo)
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
					if (!ArticoliExists(articolo.Id))
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
			return View(articolo);
		}

		// GET: Articoli/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var articolo = await _context.Articoli
				.FirstOrDefaultAsync(m => m.Id == id);
			if (articolo == null)
			{
				return NotFound();
			}

			return View(articolo);
		}

		// POST: Articoli/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var articolo = await _context.Articoli.FindAsync(id);
			_context.Articoli.Remove(articolo);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ArticoliExists(int id)
		{
			return _context.Articoli.Any(e => e.Id == id);
		}
	}
}
