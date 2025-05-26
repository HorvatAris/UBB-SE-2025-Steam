// CreatePostViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace SteamProfileWeb.ViewModels
{
    public class CreatePostViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        public int? GameId { get; set; }
    }
}