using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SteamProfileWeb.ViewModels
{
    public class AccountSettingsViewModel
    {
        [Required]
        [Display(Name = "Username")]
        [StringLength(32, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 32 characters.")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        // For feedback messages
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
    }
}
