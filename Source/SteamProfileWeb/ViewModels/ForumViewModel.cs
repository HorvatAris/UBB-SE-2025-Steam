// ForumViewModel.cs
using BusinessLayer.Models;
using System.Collections.Generic;

namespace SteamProfileWeb.ViewModels
{
    public class ForumViewModel
    {
        public List<ForumPost> Posts { get; set; }
        public int CurrentPage { get; set; }
        public bool PositiveScoreOnly { get; set; }
        public string SortOption { get; set; }
        public string SearchFilter { get; set; }
        public int CurrentUserId { get; set; }

        // Pagination
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public int TotalPosts { get; set; }
    }
}