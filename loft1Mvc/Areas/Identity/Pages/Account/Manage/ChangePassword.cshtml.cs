using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using loft1Mvc.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
namespace loft1Mvc.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<GenericUser> _userManager;
        private readonly SignInManager<GenericUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<GenericUser> userManager,
            SignInManager<GenericUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Campo obbligatorio.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password corrente")]
            public string OldPassword { get; set; }

            [Required(ErrorMessage = "Campo obbligatorio.")]
            [StringLength(100, ErrorMessage = "Password troppo corta", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nuova password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Conferma password")]
            [Compare("NewPassword", ErrorMessage = "Le due password non coincidono")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Impossibile trovare l'utente con ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Impossibile trovare l'utente con ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                StatusMessage = "La nuova password non rispetta i requisiti minimi di sicurezza.";
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("L'utente ha cambiato password con successo.");
            StatusMessage = "La tua password è stata cambiata con successo.";

            return RedirectToPage();
        }
    }
}
