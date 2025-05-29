using SteamHub.ApiContract.Models;
using System.Collections.Generic;

namespace SteamHub.Web.ViewModels
{
    public class PostDetailViewModel
    {
        public ForumPost Post { get; set; } = new ForumPost();
        public List<ForumComment> Comments { get; set; } = new List<ForumComment>();
        public int CurrentUserId { get; set; }
    }
}