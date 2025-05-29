using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.Web.ViewModels
{
    public class CommentViewModel
    {
        public Comment Comment { get; set; }
        public User Author { get; set; }
        public bool IsCurrentUserAuthor { get; set; }
    }
}
