using System.Text;
using CabManagementSystem.Models;
using CabManagementSystem.Models.Enums;
using CabManagementSystem.Models.ViewModels;
using CabManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace CabManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ICustomerService _customerService;
        private readonly ILoyaltyPointsService _loyaltyService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            ICustomerService customerService,
            ILoyaltyPointsService loyaltyService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _customerService = customerService;
            _loyaltyService = loyaltyService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToRoleDashboard();

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToRoleDashboard();
                }
                if (result.RequiresTwoFactor)
                {
                    await SendTwoFactorCodeAsync();
                    return RedirectToAction(nameof(LoginWith2fa), new { rememberMe = model.RememberMe, returnUrl });
                }
                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty,
                        "You must confirm your email before logging in. Check your inbox or request a new confirmation link.");
                    ViewData["ShowResendConfirmation"] = true;
                    return View(model);
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Your password has been changed.";
                return RedirectToRoleDashboard();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var roles = await _userManager.GetRolesAsync(user);
            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? "No Role",
                TwoFactorEnabled = user.TwoFactorEnabled
            };

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var roles = await _userManager.GetRolesAsync(user);
            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? "No Role"
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            ModelState.Remove("Email");
            ModelState.Remove("Role");

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Account",
                    new { email = model.Email, token = token }, Request.Scheme);

                if (resetLink != null)
                {
                    await _emailService.SendPasswordResetAsync(model.Email, resetLink);
                }
            }

            // Always show the same message to prevent email enumeration
            TempData["SuccessMessage"] = "If an account with that email exists, a password reset email has been sent.";

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string? email, string? token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Invalid password reset link.";
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["SuccessMessage"] = "Password has been reset successfully.";
                return RedirectToAction("Login");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Password has been reset successfully. You can now log in.";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToRoleDashboard();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CustomerRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(model);
            }

            var nameParts = model.Name.Trim().Split(' ', 2);
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = nameParts[0],
                LastName = nameParts.Length > 1 ? nameParts[1] : string.Empty,
                Role = nameof(UserRole.Customer),
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, nameof(UserRole.Customer));

                var customer = new Customer
                {
                    Name = model.Name.Trim(),
                    Email = model.Email,
                    Phone = model.Phone,
                    Address = model.Address
                };
                await _customerService.CreateCustomerAsync(customer);

                // Award signup loyalty bonus
                try
                {
                    await _loyaltyService.AwardSignupBonusAsync(customer.Id);
                }
                catch
                {
                    // Non-critical: registration succeeded, bonus failure shouldn't block
                }

                await SendEmailConfirmationLinkAsync(user);
                return RedirectToAction(nameof(RegisterConfirmation), new { email = model.Email });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterConfirmation(string? email)
        {
            ViewData["Email"] = email;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string? userId, string? token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Invalid email confirmation link.";
                return RedirectToAction("Login");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid email confirmation link.";
                return RedirectToAction("Login");
            }

            string decodedToken;
            try
            {
                decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            }
            catch (FormatException)
            {
                TempData["ErrorMessage"] = "Invalid email confirmation link.";
                return RedirectToAction("Login");
            }

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Email confirmed. You can now log in.";
            }
            else
            {
                TempData["ErrorMessage"] = "Email confirmation failed. The link may have expired.";
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ResendConfirmation(string? email)
        {
            return View(new ResendConfirmationViewModel { Email = email ?? string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmation(ResendConfirmationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
            {
                await SendEmailConfirmationLinkAsync(user);
            }

            // Always show the same message to prevent email enumeration
            TempData["SuccessMessage"] = "If an unconfirmed account with that email exists, a new confirmation link has been sent.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string? returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                return RedirectToAction("Login");

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginWith2faViewModel { RememberMe = rememberMe });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
                return View(model);

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                return RedirectToAction("Login");

            var code = model.TwoFactorCode.Trim();
            var result = await _signInManager.TwoFactorSignInAsync(
                TokenOptions.DefaultEmailProvider, code, model.RememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                return RedirectToRoleDashboard();
            }

            ModelState.AddModelError(string.Empty, "Invalid verification code.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendTwoFactorCode(bool rememberMe, string? returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                return RedirectToAction("Login");

            await SendTwoFactorCodeAsync();
            TempData["SuccessMessage"] = "A new verification code has been sent to your email.";
            return RedirectToAction(nameof(LoginWith2fa), new { rememberMe, returnUrl });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactor()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Two-factor authentication enabled. You will receive a code by email at each login.";
            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactor()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Two-factor authentication disabled.";
            return RedirectToAction(nameof(Profile));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task SendEmailConfirmationLinkAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmLink = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token = encodedToken }, Request.Scheme);

            if (confirmLink != null && user.Email != null)
            {
                await _emailService.SendEmailConfirmationAsync(user.Email, confirmLink);
            }
        }

        private async Task SendTwoFactorCodeAsync()
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user?.Email == null)
                return;

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
            await _emailService.SendTwoFactorCodeAsync(user.Email, code);
        }

        private IActionResult RedirectToRoleDashboard()
        {
            if (User.IsInRole(nameof(UserRole.Admin)))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            if (User.IsInRole(nameof(UserRole.FinanceManager)))
                return RedirectToAction("Index", "Dashboard", new { area = "Finance" });
            if (User.IsInRole(nameof(UserRole.HRManager)))
                return RedirectToAction("Index", "Dashboard", new { area = "HR" });
            if (User.IsInRole(nameof(UserRole.TravelManager)))
                return RedirectToAction("Index", "Dashboard", new { area = "Travel" });
            if (User.IsInRole(nameof(UserRole.Customer)))
                return RedirectToAction("Index", "Dashboard", new { area = "CustomerPortal" });
            return RedirectToAction("Index", "Home");
        }
    }
}
