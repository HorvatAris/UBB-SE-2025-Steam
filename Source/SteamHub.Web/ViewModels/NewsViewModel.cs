using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.Web.ViewModels
{
    public class NewsViewModel
    {
        public List<Post> Posts { get; set; } = new List<Post>();
        public int CurrentPage { get; set; } = 1;
        public string SearchText { get; set; } = string.Empty;
        public bool IsDeveloper { get; set; }
        public int TotalPages { get; set; }

        public const int PageSize = 9;

        public Dictionary<int, User> Authors { get; set; }
}
}
