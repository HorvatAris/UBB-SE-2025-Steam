using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SteamProfileWeb.ViewModels;
using System.IO;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

public class SettingsController : Controller
{
    private readonly IFeaturesService featuresService;
    private readonly IUserService userService;

    public SettingsController(IFeaturesService featuresService, IUserService userService)
    {
        this.featuresService = featuresService;
        this.userService = userService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ManageFeatures()
    {
        var model = featuresService.GetFeaturesByCategories();
        return View(model);
    }

    public IActionResult AccountSettings()
    {
        var user = userService.GetCurrentUser();
        var model = new AccountSettingsViewModel
        {
            Username = user?.Username,
            Email = user?.Email
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AccountSettings(AccountSettingsViewModel model, string action)
    {
        var user = userService.GetCurrentUser();
        if (user == null)
        {
            model.ErrorMessage = "User not found or not logged in.";
            return View(model);
        }

        switch (action)
        {
            case "UpdateUsername":
                try
                {
                    userService.UpdateUserUsername(user.UserId, model.Username);

                    var refreshedUser = userService.GetCurrentUser();
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, refreshedUser.Username),
                };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync("Identity.Application", principal).Wait();

                    TempData["SuccessMessage"] = "Username updated successfully!";
                    return RedirectToAction("AccountSettings");
                }
                catch (Exception ex)
                {
                    model.ErrorMessage = "Failed to update username. " + ex.Message;
                }
                break;
            case "UpdateEmail":
                try
                {
                    userService.UpdateUserEmail(user.UserId, model.Email);
                    TempData["SuccessMessage"] = "Email updated successfully!";
                    return RedirectToAction("AccountSettings");
                }
                catch (Exception ex)
                {
                    model.ErrorMessage = "Failed to update email. " + ex.Message;
                }
                break;
            case "UpdatePassword":
                try
                {
                    userService.UpdateUserPassword(user.UserId, model.Password);
                    TempData["SuccessMessage"] = "Password updated successfully!";
                    return RedirectToAction("AccountSettings");
                }
                catch (Exception ex)
                {
                    model.ErrorMessage = "Failed to update password. " + ex.Message;
                }
                break;
            case "DeleteAccount":
                await HttpContext.SignOutAsync("Identity.Application");
                userService.Logout();
                userService.DeleteUser(user.UserId);
                return RedirectToAction("Index", "Home");
            case "Logout":
                userService.Logout();
                return RedirectToAction("Index", "Home");
        }

        // Reload user info for display
        var updatedUser = userService.GetCurrentUser();
        if (updatedUser != null)
        {
            model.Username = updatedUser.Username;
            model.Email = updatedUser.Email;
        }
        model.Password = "";
        model.CurrentPassword = "";
        return View(model);
    }

    [HttpGet]
    public IActionResult ModifyProfile()
    {
        var user = userService.GetCurrentUser();
        if (user == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var model = new ModifyProfileViewModel
        {
            ProfilePictureUrl = user.ProfilePicturePath
        };

        return View(model);
    }


    [HttpPost]
    public IActionResult ModifyProfile(ModifyProfileViewModel model)
    {
        var user = userService.GetCurrentUser();
        if (user == null)
        {
            model.ErrorMessage = "User not found or not logged in.";
            return View(model);
        }

        // Handle profile picture upload
        if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ProfilePicture.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.ProfilePicture.CopyTo(stream);
            }

            user.ProfilePicturePath = $"/uploads/{fileName}";
        }

        // Save changes (for picture path)
        userService.UpdateUser(user);

        model.SuccessMessage = "Profile updated successfully!";
        model.ProfilePictureUrl = user.ProfilePicturePath;
        return View(model);
    }
}
