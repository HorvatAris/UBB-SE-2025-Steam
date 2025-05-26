// PostDetailViewModel.cs
using BusinessLayer.Models;
using System.Collections.Generic;

namespace SteamProfileWeb.ViewModels
{
    public class PostDetailViewModel
    {
        public ForumPost Post { get; set; }
        public List<ForumComment> Comments { get; set; }
        public int CurrentUserId { get; set; }
    }
}