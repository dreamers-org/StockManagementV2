using System;
using StockManagement.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using StockManagement.Services;
using loft1Mvc.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace loft1Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.Configure<CookiePolicyOptions>(options =>
                {
                    options.CheckConsentNeeded = context => false;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });

                services.AddDbContext<StockV2Context>(options => options.UseSqlServer(Configuration.GetConnectionString("StockV2ContextConnection")));
                //services.AddDbContext<StockV2Context>(options => options.UseSqlServer(Configuration.GetConnectionString("Production")));
                services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                services.AddTransient<IEmailSender, EmailSender>();
                services.AddScoped<IUserClaimsPrincipalFactory<GenericUser>, CustomClaimsPrincipalFactory>();
                services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(30);
                });

                //services.AddAuthentication()
                // .AddCookie(cfg => cfg.SlidingExpiration = true);
                services.ConfigureApplicationCookie(options =>
                {
                    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.LoginPath = "/Identity/Account/Login";
                    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                    options.SlidingExpiration = true;
                });

                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            try
            {

                Serilog.Debugging.SelfLog.Enable(Console.Error);

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseDatabaseErrorPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseCookiePolicy();
                app.UseAuthentication();
                app.UseSession();
                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
            //CreateRoles(serviceProvider).Wait();
        }

        //private async Task CreateRoles(IServiceProvider serviceProvider)
        //{
        //	//adding custom roles
        //	var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //	var UserManager = serviceProvider.GetRequiredService<UserManager<GenericUser>>();
        //	string[] roleNames = { "Rappresentante","Commesso", "Titolare", "SuperAdmin" };
        //	IdentityResult roleResult;

        //          foreach (var roleName in roleNames)
        //          {
        //              //creating the roles and seeding them to the database
        //              var roleExist = await RoleManager.RoleExistsAsync(roleName);
        //              if (!roleExist)
        //              {
        //                  roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
        //              }
        //          }

        //          var _user = await UserManager.FindByEmailAsync("luca@admin.it");
        //	if (_user != null)
        //	{
        //		await UserManager.AddToRoleAsync(_user, "SuperAdmin");
        //	}
        //}


    }
}
