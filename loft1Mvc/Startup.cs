using System;
using loft1Mvc.Models;
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
					options.CheckConsentNeeded = context => true;
					options.MinimumSameSitePolicy = SameSiteMode.None;
				});

				services.AddDbContext<Loft1Context>(options => options.UseSqlServer(Configuration.GetConnectionString("StockContextConnection")));
				services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
				services.AddTransient<IEmailSender, EmailSender>();
				services.AddSession(options =>
				{
					options.IdleTimeout = TimeSpan.FromMinutes(30);
				});

				services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

				//var policy = new AuthorizationPolicyBuilder()
				//.RequireAuthenticatedUser()
				//.RequireRole("Admin", "SuperUser")
				//.Build();

				//services.AddMvc(options =>
				//{
				//	options.Filters.Add(new AuthorizeFilter(policy));
				//	//options.Filters.Add(new AllowAnonymousFilter());
				//});
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message);
				throw;
			}

		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			try
			{

				Serilog.Debugging.SelfLog.Enable(Console.Error);

				if (env.IsDevelopment())
				{
					app.UseDeveloperExceptionPage();
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
		//	string[] roleNames = { "SuperAdmin" };
		//	IdentityResult roleResult;

		//	foreach (var roleName in roleNames)
		//	{
		//		//creating the roles and seeding them to the database
		//		var roleExist = await RoleManager.RoleExistsAsync(roleName);
		//		if (!roleExist)
		//		{
		//			roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
		//		}
		//	}

		//	var _user = await UserManager.FindByEmailAsync("luca@admin.com");
		//	if (_user != null)
		//	{
		//		await UserManager.AddToRoleAsync(_user, "SuperAdmin");
		//	}
		//}


	}
}
