using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using loft1Mvc.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace loft1Mvc.Models
{
	public class IdentityContext : IdentityDbContext<GenericUser>
	{
		public IdentityContext(DbContextOptions<IdentityContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
		}
	}
}
