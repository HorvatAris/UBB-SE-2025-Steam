using SteamHub.ApiContract.Models;
using System.Collections.Generic;

namespace SteamHub.Web.ViewModels
{
    public class ForumViewModel
    {
        public List<ForumPost> Posts { get; set; } = new List<ForumPost>();
        public int CurrentPage { get; set; }
        public bool PositiveScoreOnly { get; set; }
        public string SortOption { get; set; } = string.Empty;
        public string SearchFilter { get; set; } = string.Empty;
        public int CurrentUserId { get; set; }

        // Pagination
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public int TotalPosts { get; set; }
    }
}