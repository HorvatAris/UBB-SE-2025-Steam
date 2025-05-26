// AddCommentViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace SteamProfileWeb.ViewModels
{
    public class AddCommentViewModel
    {
        [Required]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Comment content is required")]
        public string Content { get; set; }
    }
}