using BusinessLayer.Models;
using System;
using System.Collections.Generic;

namespace SteamProfileWeb.ViewModels
{
    public class NewsViewModel
    {
        public List<Post> Posts { get; set; } = new List<Post>();
        public int CurrentPage { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
        public bool IsDeveloper { get; set; }
        public int TotalPages { get; set; }

        public const int PageSize = 9;
    }
}
