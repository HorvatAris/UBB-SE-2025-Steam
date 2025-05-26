using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamProfileWeb.Services;
using SteamProfileWeb.ViewModels;

namespace SteamProfileWeb.Controllers
{
    /// <summary>
    /// Controller responsible for user authentication: login, registration, and logout.
    /// </summary>
    public class AuthController : Controller
    {
        private readonly IAuthManager authManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authManager">Service to handle authentication operations.</param>
        public AuthController(IAuthManager authManager)
        {
            this.authManager = authManager;
        }

        /// <summary>
        /// Displays the login page.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Displays the registration page.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Processes a login POST request.
        /// </summary>
        /// <param name="model">User credentials from the form.</param>
        /// <param name="returnUrl">URL to redirect to after successful login.</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                bool result = await authManager.LoginAsync(model.UsernameOrEmail, model.Password);
                if (result)
                {
                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Processes a registration POST request.
        /// </summary>
        /// <param name="model">User registration data from the form.</param>
        /// <param name="returnUrl">URL to redirect to after successful registration.</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                bool success = await authManager.RegisterAsync(
                    model.Username,
                    model.Email,
                    model.Password,
                    model.IsDeveloper);

                if (success)
                {
                    // Redirect to login page on successful registration
                    return RedirectToAction(nameof(Login), new { returnUrl });
                }

                ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Processes a logout request.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await authManager.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Displays the access denied page.
        /// </summary>
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// Redirects to a local URL or falls back to home.
        /// </summary>
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}