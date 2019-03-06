using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using loft1Mvc.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace StockManagement.Controllers
{
	public class OrdineDalClienteController : Controller
	{
		private readonly Loft1Context _context;

		public OrdineDalClienteController(Loft1Context context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index(string role)
		{
			if (role == "InserimentoInfoPreliminari")
			{
				return View("InfoPreliminariRappresentante");
			}
			if (User.Identity.IsAuthenticated && User.Identity.Name.IndexOf("rappresentante") != -1)  //TODO Sarà da cambiare con user.isinrole
			{
				return View(await _context.OrdiniDaiClienti.Where(x => x.Rappresentante == User.Identity.Name).ToListAsync());
			}
			return View(await _context.OrdiniDaiClienti.ToListAsync());
		}



		[HttpPost]
		public IActionResult IndexRappresentante(InfoPreliminariOrdineCliente info)
		{
			if (ModelState.IsValid)
			{
				if (info != null)
				{
					HttpContext.Session.SetString("OrdineInCorso", "true");
					HttpContext.Session.SetString("Cliente", info.Cliente);
					HttpContext.Session.SetString("Indirizzo", info.Indirizzo);
					HttpContext.Session.SetString("DataConsegna", string.Format("{0:d}", info.DataConsegna));
					ViewBag.Cliente = info.Cliente;
					ViewBag.DataConsegna = string.Format("{0:d}", info.DataConsegna);
					return View("Create");
				}
			}
			return View("InfoPreliminariRappresentante");
		}

		// GET: OrdineDalCliente/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var ordineDalCliente = await _context.OrdiniDaiClienti
				.FirstOrDefaultAsync(m => m.Id == id);
			if (ordineDalCliente == null)
			{
				return NotFound();
			}

			return View(ordineDalCliente);
		}

		public async Task<IActionResult> Create()
		{
			if (User.Identity.IsAuthenticated && User.Identity.Name.IndexOf("rappresentante") != -1)
            {
                ViewBag.ListaOrdini = await _context.OrdiniDaiClienti.Where(x => x.Rappresentante == User.Identity.Name).ToListAsync();

                return View("CreateRappresentante");
			}
			return View("CreateCommesso");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Cliente,Rappresentante,DataOrdine,DataConsegna,Indirizzo,Pagamento,Codice,Descrizione,Colore,Xxxs,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,Xxxxl,attr1,attr2")] OrdineDalCliente ordineDalCliente)
		{
			if (ModelState.IsValid)
			{
				if (string.IsNullOrEmpty(HttpContext.Session.GetString("isOrdineInCorso")))
				{
					HttpContext.Session.SetString("isOrdineInCorso", Guid.NewGuid().ToString()); //l'isOrdineInCorso coincide con l'id dell'ordine
					HttpContext.Session.SetString("Cliente", ordineDalCliente.Cliente);
					HttpContext.Session.SetString("Rappresentante", ordineDalCliente.Rappresentante);
					HttpContext.Session.SetString("DataOrdine", ordineDalCliente.DataOrdine.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
					HttpContext.Session.SetString("DataConsegna", ordineDalCliente.DataConsegna.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
					HttpContext.Session.SetString("Indirizzo", ordineDalCliente.Indirizzo);
					HttpContext.Session.SetString("Pagamento", ordineDalCliente.Pagamento);
				}
				ordineDalCliente.IdOrdine = HttpContext.Session.GetString("isOrdineInCorso");
				ordineDalCliente.DataConsegna = DateTime.ParseExact(HttpContext.Session.GetString("DataConsegna"), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
				ordineDalCliente.DataOrdine = DateTime.ParseExact(HttpContext.Session.GetString("DataOrdine"), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);
				_context.Add(ordineDalCliente);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View("Index", ordineDalCliente);
		}


		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var ordineDalCliente = await _context.OrdiniDaiClienti.FindAsync(id);
			if (ordineDalCliente == null)
			{
				return NotFound();
			}
			return View(ordineDalCliente);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,IdOrdine,Cliente,Rappresentante,DataOrdine,DataConsegna,Indirizzo,Pagamento,Codice,Colore,Xxxs,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,Xxxxl,attr1,attr2")] OrdineDalCliente ordineDalCliente)
		{
			if (id != ordineDalCliente.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(ordineDalCliente);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!OrdineDalClienteExists(ordineDalCliente.Id))
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
			return View(ordineDalCliente);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var ordineDalCliente = await _context.OrdiniDaiClienti
				.FirstOrDefaultAsync(m => m.Id == id);
			if (ordineDalCliente == null)
			{
				return NotFound();
			}

			return View(ordineDalCliente);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var ordineDalCliente = await _context.OrdiniDaiClienti.FindAsync(id);
			_context.OrdiniDaiClienti.Remove(ordineDalCliente);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult CloseOrder()
		{
			HttpContext.Session.Clear();
			return RedirectToAction(nameof(Index));
		}

		private bool OrdineDalClienteExists(int id)
		{
			return _context.OrdiniDaiClienti.Any(e => e.Id == id);
		}

		public IActionResult getTxtValuesFromSession()
		{
			tempObject result = new tempObject
			{
				Cliente = HttpContext.Session.GetString("Cliente"),
				Rappresentante = HttpContext.Session.GetString("Rappresentante"),
				DataOrdine = HttpContext.Session.GetString("DataOrdine"),
				DataConsegna = HttpContext.Session.GetString("DataConsegna"),
				Indirizzo = HttpContext.Session.GetString("Indirizzo"),
				Pagamento = HttpContext.Session.GetString("Pagamento")
			};
			return Json(result);
		}

		protected class tempObject
		{
			public string Cliente { get; set; }
			public string Rappresentante { get; set; }
			public string DataOrdine { get; set; }
			public string DataConsegna { get; set; }
			public string Indirizzo { get; set; }
			public string Pagamento { get; set; }

		}
	}
}
