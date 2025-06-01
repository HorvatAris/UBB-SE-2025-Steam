using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IUserService userService;
        private readonly IWebHostEnvironment webHostEnvironment;

        public SettingsController(IUserService userService, IWebHostEnvironment webHostEnvironment)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Modify()
        {
            var user = await userService.GetCurrentUserAsync();
            if (user == null)
                return Unauthorized();

            var viewModel = new ModifyProfileViewModel
            {
                UserService = userService,
                Description = user.Bio ?? string.Empty
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(ModifyProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userService.GetCurrentUserAsync();
            if (user == null)
                return Unauthorized();

            bool hasChanges = false;
            var errors = new List<string>();

            try
            {
                // Handle profile picture update
                if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(model.ProfilePicture.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        errors.Add("Please select a valid image file (jpg, jpeg, png, gif).");
                    }
                    else if (model.ProfilePicture.Length > 5 * 1024 * 1024) // 5MB limit
                    {
                        errors.Add("File size cannot exceed 5MB.");
                    }
                    else
                    {
                        string fileName = $"{Guid.NewGuid()}{fileExtension}";
                        string uploadPath = Path.Combine(webHostEnvironment.WebRootPath, "images", "uploads");
                        Directory.CreateDirectory(uploadPath);

                        string filePath = Path.Combine(uploadPath, fileName);

                        // Save file locally temporarily
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ProfilePicture.CopyToAsync(stream);
                        }

                        // Upload to Imgur using your function (make sure you have your client ID available here)
                        string imgurClientId = "bbf48913b385d7b"; // Replace this with your actual Imgur client ID
                        string imgurLink;
                        try
                        {
                            imgurLink = await UploadImageToImgurAsync(filePath, imgurClientId);
                        }
                        catch (Exception ex)
                        {
                            errors.Add("Failed to upload image to Imgur: " + ex.Message);
                            // Delete the temp file in case of failure
                            if (System.IO.File.Exists(filePath))
                                System.IO.File.Delete(filePath);

                            model.ErrorMessage = string.Join(" ", errors);
                            return View(model);
                        }

                        // Delete the temp file after uploading to Imgur
                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);

                        // Update user profile picture with Imgur URL
                        var picResult = await userService.UpdateProfilePictureAsync(user.UserId, imgurLink);
                        if (picResult)
                        {
                            hasChanges = true;
                        }
                        else
                        {
                            errors.Add("Failed to update profile picture.");
                        }
                    }
                }

                // Handle description update
                if (!string.IsNullOrWhiteSpace(model.Description) && model.Description != user.Bio)
                {
                    var bioResult = await userService.UpdateProfileBioAsync(user.UserId, model.Description);
                    if (bioResult)
                    {
                        hasChanges = true;
                    }
                    else
                    {
                        errors.Add("Failed to update description.");
                    }
                }

                if (errors.Any())
                {
                    model.ErrorMessage = string.Join(" ", errors);
                    return View(model);
                }

                if (hasChanges)
                {
                    model.SuccessMessage = "Profile updated successfully!";
                    model.ProfilePicture = null; // Clear the file input
                }
                else
                {
                    model.ErrorMessage = "No changes were made to your profile.";
                }

                return View(model);
            }
            catch (Exception)
            {
                model.ErrorMessage = "An error occurred while updating your profile. Please try again.";
                return View(model);
            }
        }


        public IActionResult GoBack()
        {
            return RedirectToAction("Index");
        }

        private async Task<string> UploadImageToImgurAsync(string imagePath, string clientId)
        {
            using var client = new HttpClient();
            using var form = new MultipartFormDataContent();
            using var image = new ByteArrayContent(System.IO.File.ReadAllBytes(imagePath));

            image.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
            form.Add(image, "image");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Client-ID", clientId);

            var response = await client.PostAsync("https://api.imgur.com/3/image", form);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new RepositoryException("Imgur upload failed: " + json);
            }

            var link = System.Text.Json.JsonDocument.Parse(json)
                        .RootElement.GetProperty("data")
                        .GetProperty("link").GetString();

            return link ?? throw new RepositoryException("Imgur returned null link.");
        }
    }
}