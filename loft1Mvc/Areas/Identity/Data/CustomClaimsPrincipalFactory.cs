using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using StockManagement;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace loft1Mvc.Areas.Identity.Data
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<GenericUser,IdentityRole>
    {
        public CustomClaimsPrincipalFactory(UserManager<GenericUser> userManager, RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor) { }

        public async override Task<ClaimsPrincipal> CreateAsync(GenericUser user)
        {
            try
            {
                var principal = await base.CreateAsync(user);

                if (!string.IsNullOrWhiteSpace(user.UserName))
                {
                    ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                          new Claim("AgenziaRappresentanza", user.AgenziaRappresentanza),
                          new Claim("Regione", user.Regione)
                });
                }

                return principal;
            }
            catch (Exception ex)
            {
                Utility.GestioneErrori(ex);
                throw;
            }
        }
    }
}
