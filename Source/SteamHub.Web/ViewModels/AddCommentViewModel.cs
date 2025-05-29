using System.ComponentModel.DataAnnotations;

namespace SteamHub.Web.ViewModels
{
    public class AddCommentViewModel
    {
        [Required]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Comment content is required")]
        public string Content { get; set; } = string.Empty;
    }
}