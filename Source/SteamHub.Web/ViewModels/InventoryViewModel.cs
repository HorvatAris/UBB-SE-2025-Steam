using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Item;
using SteamHub.ApiContract.Models.User;
using System.Collections.Generic;

namespace SteamHub.Web.ViewModels
{
    public class InventoryViewModel
    {
        public InventoryViewModel()
        {
            SelectedUserId = 0;
            SelectedGameId = 0;
            SearchText = string.Empty;
            InventoryItems = new List<Item>();
            AvailableGames = new List<Game>();
            AvailableUsers = new List<IUserDetails>();
            StatusMessage = string.Empty;
        }

        public int SelectedUserId { get; set; }
        public int SelectedGameId { get; set; }
        public string SearchText { get; set; }
        public List<Item> InventoryItems { get; set; }
        public List<Game> AvailableGames { get; set; }
        public List<IUserDetails> AvailableUsers { get; set; }
        public string StatusMessage { get; set; }
    }
}