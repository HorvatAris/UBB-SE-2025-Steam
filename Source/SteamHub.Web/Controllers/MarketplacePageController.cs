using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class MarketplaceController : Controller
    {
        private readonly IMarketplaceService marketplace_service;

        public MarketplaceController(IMarketplaceService marketplaceService)
        {
            marketplace_service = marketplaceService;
        }

        public async Task<IActionResult> Index()
        {
            var allUsers = await marketplace_service.GetAllUsersAsync();
            var currentUsername = User.Identity?.Name;
            var currentUser = allUsers.FirstOrDefault(u => u.Username == currentUsername);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            marketplace_service.User = currentUser;

            var marketplace_view_model = new MarketplaceViewModel
            {
                CurrentUser = currentUser,
                Items = await marketplace_service.GetAllListingsAsync()
            };

            marketplace_view_model.InitializeFilters();

            return View(marketplace_view_model);
        }


        [HttpPost]
        public async Task<IActionResult> BuyItem([FromQuery] int itemId)
        {
            try
            {
                var allUsers = await marketplace_service.GetAllUsersAsync();
                var currentUsername = User.Identity?.Name;
                var currentUser = allUsers.FirstOrDefault(u => u.Username == currentUsername);

                if (currentUser == null)
                {
                    return Json(new { success = false, message = "User not authenticated." });
                }

                marketplace_service.User = currentUser;

                var listings = await marketplace_service.GetAllListingsAsync();
                var item = listings.FirstOrDefault(i => i.ItemId == itemId && i.IsListed);

                if (item == null)
                {
                    return Json(new { success = false, message = "Item not available." });
                }

                var success = await marketplace_service.BuyItemAsync(item, currentUser.UserId);

                if (success)
                {
                    return Json(new { success = true, message = "Item purchased successfully!" });
                }

                return Json(new { success = false, message = "Purchase failed." });
            }
            catch (Exception exception)
            {
                return Json(new { success = false, message = $"Unexpected error: {exception.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ApplyFilters(string search, string game, string type, string rarity)
        {
            var items = await marketplace_service.GetAllListingsAsync();

            var filtered_items = items.ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                filtered_items = filtered_items.Where(item =>
                    item.ItemName.ToLower().Contains(s) ||
                    item.Description.ToLower().Contains(s)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(game))
                filtered_items = filtered_items.Where(i => i.Game.GameTitle == game).ToList();

            if (!string.IsNullOrWhiteSpace(type))
                filtered_items = filtered_items.Where(i => i.ItemName.Split('|')[0].Trim() == type).ToList();


            return Json(filtered_items.Select(item => new
            {
                item.ItemId,
                item.ItemName,
                item.Price,
                item.ImagePath,
                item.IsListed,
                Game = item.Game.GameTitle
            }));
        }
    }
}
