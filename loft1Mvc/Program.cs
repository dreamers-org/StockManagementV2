using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using StockManagement;
using System;

namespace loft1Mvc
{
    public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
                Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.MSSqlServer("Data Source=80.211.159.109;Initial Catalog=Loft1_Stock_PRODUCTION;User ID=administrator;Password=Pallone27@@@;Connect Timeout=30;Encrypt=True;TrustServerCertificate=true;ApplicationIntent=ReadWrite;MultiSubnetFailover=False", "Logs", autoCreateSqlTable: true)
                .CreateLogger();
				CreateWebHostBuilder(args).Build().Run();
			}
			catch (Exception ex)
			{
                Utility.GestioneErrori(ex);
				throw;
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseSerilog()
				.UseStartup<Startup>();
	}
}
