using BusinessLayer.Models;

namespace SteamProfileWeb.ViewModels
{
    public class NewsPostViewModel
    {
        public Post Post { get; set; }
        public User Author { get; set; }
        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
        public string NewCommentContent { get; set; } = string.Empty;
        public bool IsCurrentUserAuthor { get; set; }
    }
}
