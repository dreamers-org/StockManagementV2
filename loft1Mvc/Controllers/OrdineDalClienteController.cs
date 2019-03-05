﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using loft1Mvc.Models;
using Microsoft.AspNetCore.Http;

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
			if (User.IsInRole("Rappresentante"))
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

		// GET: OrdineDalCliente/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: OrdineDalCliente/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Codice,Colore,Xxxs,Xxs,Xs,S,M,L,Xl,Xxl,Xxxl,Xxxxl")] OrdineDalCliente ordineDalCliente)
		{
			if (ordineDalCliente.Colore != "-1")
			{
				ordineDalCliente.Rappresentante = "Rappresentante di test";
				ordineDalCliente.DataConsegna = DateTime.Parse((HttpContext.Session.GetString("DataConsegna")));
				ordineDalCliente.DataOrdine = DateTime.Today;
				ordineDalCliente.Indirizzo = HttpContext.Session.GetString("Indirizzo");
				ordineDalCliente.Pagamento = "Bonifico";
				ordineDalCliente.Cliente = HttpContext.Session.GetString("Cliente");
				_context.Add(ordineDalCliente);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			else
			{
				ViewBag.Error = "Selezionare un colore valido.";
				return View();
			}
		}

		// GET: OrdineDalCliente/Edit/5
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

		// POST: OrdineDalCliente/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

		// GET: OrdineDalCliente/Delete/5
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

		// POST: OrdineDalCliente/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var ordineDalCliente = await _context.OrdiniDaiClienti.FindAsync(id);
			_context.OrdiniDaiClienti.Remove(ordineDalCliente);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool OrdineDalClienteExists(int id)
		{
			return _context.OrdiniDaiClienti.Any(e => e.Id == id);
		}
	}
}
