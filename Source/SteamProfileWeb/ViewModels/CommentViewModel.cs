using BusinessLayer.Models;

namespace SteamProfileWeb.ViewModels
{
    public class CommentViewModel
    {
        public Comment Comment { get; set; }
        public User Author { get; set; }
        public bool IsCurrentUserAuthor { get; set; }
    }
}
