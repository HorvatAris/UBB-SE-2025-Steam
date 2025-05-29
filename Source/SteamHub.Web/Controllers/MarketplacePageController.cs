using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class MarketplaceController : Controller
    {
        private readonly IMarketplaceService _marketplaceService;
        private readonly IUserDetails _user;

        public MarketplaceController(IMarketplaceService marketplaceService, IUserDetails user)
        {
            _marketplaceService = marketplaceService;
            _user = user;
        }

        public async Task<IActionResult> Index()
        {
            if (_user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new MarketplaceViewModel
            {
                CurrentUser = new User(_user),
                Items = await _marketplaceService.GetAllListingsAsync()
            };

            viewModel.InitializeFilters();

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> BuyItem([FromQuery] int itemId)
        {
            try
            {
                if (_user == null)
                {
                    return Json(new { success = false, message = "User not authenticated." });
                }

                var currentUser = new User(_user);
                
                var listings = await _marketplaceService.GetAllListingsAsync();
                var item = listings.FirstOrDefault(i => i.ItemId == itemId && i.IsListed);

                if (item == null)
                {
                    return Json(new { success = false, message = "Item not available." });
                }

                var success = await _marketplaceService.BuyItemAsync(item, currentUser.UserId);

                if (success)
                {
                    return Json(new { success = true, message = "Item purchased successfully!" });
                }

                return Json(new { success = false, message = "Purchase failed." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Unexpected error: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ApplyFilters(string search, string game, string type, string rarity)
        {
            var items = await _marketplaceService.GetAllListingsAsync();

            var filtered = items.ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                filtered = filtered.Where(item =>
                    item.ItemName.ToLower().Contains(s) ||
                    item.Description.ToLower().Contains(s)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(game))
                filtered = filtered.Where(i => i.Game.GameTitle == game).ToList();

            if (!string.IsNullOrWhiteSpace(type))
                filtered = filtered.Where(i => i.ItemName.Split('|')[0].Trim() == type).ToList();


            return Json(filtered.Select(i => new
            {
                i.ItemId,
                i.ItemName,
                i.Price,
                i.ImagePath,
                i.IsListed,
                Game = i.Game.GameTitle
            }));
        }
    }
}
