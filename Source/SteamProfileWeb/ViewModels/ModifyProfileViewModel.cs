using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SteamProfileWeb.ViewModels
{
    public class ModifyProfileViewModel
    {
        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }

        // For displaying the current or newly uploaded picture
        public string ProfilePictureUrl { get; set; }

        [Display(Name = "Profile Description")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        // For feedback messages
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
    }
}
