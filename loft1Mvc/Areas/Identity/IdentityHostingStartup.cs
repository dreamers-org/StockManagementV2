using System;
using loft1Mvc.Areas.Identity.Data;
using loft1Mvc.Models;
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
				services.AddDbContext<IdentityContext>(options => options.UseSqlServer(context.Configuration.GetConnectionString("IdentityContextConnection")));

				services.AddIdentity<GenericUser, IdentityRole>(config =>
				{
					config.SignIn.RequireConfirmedEmail = true;
				})

					.AddEntityFrameworkStores<IdentityContext>()
					 .AddDefaultTokenProviders();
			});
		}
	}
}