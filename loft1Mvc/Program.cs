using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
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
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.MSSqlServer("Data Source=loft1mvc.database.windows.net;Initial Catalog=StockV2;User ID=luca.bellavia.dev;Password=Pallone27@@;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False", "Logs", autoCreateSqlTable: true)
                //.WriteTo.MSSqlServer("Server=tcp:loftserver.database.windows.net,1433;Initial Catalog=Stock;Persist Security Info=False;User ID=loftadmin;Password=loft1loft1@h2o;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30", "Logs", autoCreateSqlTable: true)
                .CreateLogger();
				CreateWebHostBuilder(args).Build().Run();
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message);
				throw;
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseSerilog()
				.UseStartup<Startup>();
	}
}
