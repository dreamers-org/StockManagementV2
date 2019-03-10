using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockManagement.Models;

namespace StockManagement.Controllers
{
	public class ArticoloController : Controller
	{
		private readonly Loft1Context _context;

		public ArticoloController(Loft1Context context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.Articoli.Where(x => x.Annullato == false).ToListAsync());
		}

		public IActionResult Create()
		{
			List<string> tipoProdotto = new List<string>();

			tipoProdotto = (from tipo in _context.TipoProdotto
							select tipo.Tipo).ToList();

			ViewBag.tipiProdotto = tipoProdotto;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Fornitore,Codice,Descrizione,Colore,Xxxs,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,Xxxxl,TrancheConsegna,PrezzoAcquisto,PrezzoVendita,TipoProdotto, Genere")] ArticoloViewModel art)
		{
			//TODO LUCA: Upload immagine
			if (ModelState.IsValid)
			{
				if (ArticoloExists(art.Codice, art.Colore))
				{
					ViewBag.ErrorMessage = $"E' già stato inserito in precedenza l'articolo {art.Codice} nella variante colore {art.Colore}";
					RedirectToAction("Create");
				}

				//TODO PENSARE A COME CASTARE
				Articolo result = art as Articolo;
				result.Operatore = User.Identity.Name;
				result.DataInserimento = DateTime.Now.ToLongDateString();
				_context.Add(result);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(art);
		}

		private bool ArticoloExists(string codice, string color)
		{
			return _context.Articoli.Any(e => e.Codice == codice && e.Colore.ToLower() == color.ToLower());
		}

		public async Task<IActionResult> getTxtValues(string Codice)
		{
			TempObject result = new TempObject();
			var articolo = await _context.Articoli
				.FirstOrDefaultAsync(m => m.Codice == Codice && m.Annullato==false);
			if (articolo == null)
			{
				result = new TempObject
				{
					Fornitore = "",
					Descrizione = "",
					PrezzoAcquisto = "",
					PrezzoVendita = "",
					TrancheConsegna = "",
					GenereProdotto = "Uomo",
					TipoProdotto = "T-shirt"
				};
			}
			else
			{
				result = new TempObject
				{
					Fornitore = articolo.Fornitore,
					Descrizione = articolo.Descrizione,
					PrezzoAcquisto = articolo.PrezzoAcquisto.ToString(),
					PrezzoVendita = articolo.PrezzoVendita.ToString(),
					TrancheConsegna = string.Format(articolo.TrancheConsegna, "dd/MM/YYYY"),
					GenereProdotto = articolo.Genere,
					TipoProdotto = articolo.TipoProdotto
				};
			}
			return Json(result);
		}

		protected class TempObject
		{
			public string Fornitore { get; set; }
			public string Descrizione { get; set; }
			public string PrezzoAcquisto { get; set; }
			public string PrezzoVendita { get; set; }
			public string TrancheConsegna { get; set; }
			public string GenereProdotto { get; set; }
			public string TipoProdotto { get; set; }
		}
	}
}
