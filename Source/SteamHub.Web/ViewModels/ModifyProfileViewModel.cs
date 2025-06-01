using SteamHub.ApiContract.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SteamHub.Web.ViewModels
{
    public class ModifyProfileViewModel
    {
        // Don't bind this property from form data
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public IUserService? UserService { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        public string? SelectedImageName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public string? SuccessMessage { get; set; }

        public string? ErrorMessage { get; set; }

        public bool CanSave => !string.IsNullOrWhiteSpace(Description) || ProfilePicture != null;
    }
}