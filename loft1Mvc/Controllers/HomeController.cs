﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Models;
using Serilog;
using Microsoft.AspNetCore.Authorization;

namespace loft1Mvc.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			try
			{
				Log.Error("Test");
			}
			catch (System.Exception)
			{
				Log.Error("Test");
				throw;
			}
			Log.Error("Test");
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[Authorize(Roles = "Titolare,SuperAdmin")]
		public IActionResult Admin()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
