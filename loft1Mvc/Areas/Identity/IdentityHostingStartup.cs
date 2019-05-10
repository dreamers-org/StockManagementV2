using System;
using loft1Mvc.Areas.Identity.Data;
using StockManagement.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockManagement;

[assembly: HostingStartup(typeof(loft1Mvc.Areas.Identity.IdentityHostingStartup))]
namespace loft1Mvc.Areas.Identity
{
	public class IdentityHostingStartup : IHostingStartup
	{
		public void Configure(IWebHostBuilder builder)
		{
            try
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
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
		}
	}
}