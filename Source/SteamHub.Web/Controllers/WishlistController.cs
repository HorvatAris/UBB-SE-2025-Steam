using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models.UsersGames;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IUserGameService userGameService;
        private readonly IGameService gameService;

        public WishlistController(IUserGameService userGameService, IGameService gameService)
        {
            this.userGameService = userGameService;
            this.gameService = gameService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search = "", string filter = "", string sort = "")
        {
            var userId = userGameService.GetUser().UserId;
            var games = (await userGameService.GetWishListGamesAsync(userId)).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                games = games.Where(g => g.GameTitle.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                games = filter switch
                {
                    "OVERWHELMINGLYPOSITIVE" => games.Where(game => game.Rating >= 4.5m).ToList(),
                    "VERYPOSITIVE" => games.Where(game => game.Rating >= 4 && game.Rating < 4.5m).ToList(),
                    "MIXED" => games.Where(game => game.Rating >= 2 && game.Rating < 4).ToList(),
                    "NEGATIVE" => games.Where(game => game.Rating < 2).ToList(),
                    _ => games
                };
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                games = sort switch
                {
                    "PRICE_ASC" => games.OrderBy(game => game.Price).ToList(),
                    "PRICE_DESC" => games.OrderByDescending(game => game.Price).ToList(),
                    "RATING_DESC" => games.OrderByDescending(game => game.Rating).ToList(),
                    "DISCOUNT_DESC" => games.OrderByDescending(game => game.Discount).ToList(),
                    _ => games
                };
            }

            var wish_list_view_model = new WishlistViewModel
            {
                WishListGames = games,
                Search = search,
                Filter = filter,
                Sort = sort
            };

            return View(wish_list_view_model);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int gameId)
        {
            var game = await gameService.GetGameByIdAsync(gameId);
            var request = new UserGameRequest
            {
                GameId = game.GameId,
                UserId = userGameService.GetUser().UserId
            };
            if (game != null)
            {
                await userGameService.RemoveGameFromWishlistAsync(request);
            }

            return RedirectToAction("Index");
        }
    }
}
