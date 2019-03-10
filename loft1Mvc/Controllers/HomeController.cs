using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using loft1Mvc.Models;
using Serilog;

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
