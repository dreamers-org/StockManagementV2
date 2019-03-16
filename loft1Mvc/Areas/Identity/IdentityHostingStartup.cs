using System;
using loft1Mvc.Areas.Identity.Data;
using StockManagement.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(loft1Mvc.Areas.Identity.IdentityHostingStartup))]
namespace loft1Mvc.Areas.Identity
{
	public class IdentityHostingStartup : IHostingStartup
	{
		public void Configure(IWebHostBuilder builder)
		{
			builder.ConfigureServices((context, services) =>
			{
                //var lockoutOptions = new LockoutOptions()
                //{
                //	DefaultLockoutTimeSpan = TimeSpan.FromDays(365),
                //	MaxFailedAccessAttempts = 3
                //};

                services.AddDbContext<IdentityContext>(options => options.UseSqlServer(context.Configuration.GetConnectionString("IdentityContextConnection")));
                //services.AddDbContext<IdentityContext>(options => options.UseSqlServer(context.Configuration.GetConnectionString("IdentityProduction")));

				services.AddIdentity<GenericUser, IdentityRole>(config =>
				{
					//config.Lockout = lockoutOptions; //TODO LUCA: SERVE? UTILE?
					config.SignIn.RequireConfirmedEmail = false;
				})

					.AddEntityFrameworkStores<IdentityContext>()
					 .AddDefaultTokenProviders();
			});
		}
	}
}