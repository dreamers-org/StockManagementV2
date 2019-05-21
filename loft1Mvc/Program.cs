using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using StockManagement;
using System;
using System.Configuration;

namespace loft1Mvc
{
    public class Program
	{
        static IConfiguration _configuration;
        public Program(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }
		public static void Main(string[] args)
		{
			try
			{
                string connectionString = _configuration.GetConnectionString("StockConnection");

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
