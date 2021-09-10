using Asc_Time_Tracker.Areas.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Asc_Time_Tracker.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<TimeTrackerUser> _signInManager;
        private readonly UserManager<TimeTrackerUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<TimeTrackerUser> userManager,
            SignInManager<TimeTrackerUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "ASC Code")]
            public string SecretRoleCode { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // Check that the user entered a valid code.
                if (Input.SecretRoleCode != _configuration.GetSection("RoleCodes")["User"] &&
                    Input.SecretRoleCode != _configuration.GetSection("RoleCodes")["Manager"] &&
                    Input.SecretRoleCode != _configuration.GetSection("RoleCodes")["Admin"])
                {
                    ModelState.AddModelError(string.Empty, "Bad ASC code.");
                    return Page();
                }

                var user = new TimeTrackerUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    // Assign user roles and create them if they don't exist.
                    if (Input.SecretRoleCode == _configuration.GetSection("RoleCodes")["User"])
                    {
                        if (!await _roleManager.RoleExistsAsync("User"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("User"));
                        }

                        await _userManager.AddToRoleAsync(user, "User");
                    }
                    else if (Input.SecretRoleCode == _configuration.GetSection("RoleCodes")["Manager"])
                    {
                        if (!await _roleManager.RoleExistsAsync("Manager"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("Manager"));
                        }

                        await _userManager.AddToRoleAsync(user, "Manager");
                    }
                    else if (Input.SecretRoleCode == _configuration.GetSection("RoleCodes")["Admin"])
                    {
                        if (!await _roleManager.RoleExistsAsync("Admin"))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("Admin"));
                        }

                        await _userManager.AddToRoleAsync(user, "Admin");
                    }

                    _logger.LogInformation("User created a new account with password.");

                    // Play intro login script.
                    TempData["FirstLoginFlag"] = "true";

                    await _signInManager.SignInAsync(user, false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}