using System.ComponentModel.DataAnnotations;

namespace SteamHub.Web.ViewModels
{
    public class CreatePostViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        public int? GameId { get; set; }
    }
}