using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using SteamHub.ApiContract.Models.ItemTrade;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;

namespace SteamHub.Web.Controllers
{
	[Authorize]
	public class TradePageController : Controller
	{
		private readonly IUserService user_service;
		private readonly IGameService game_service;
		private readonly ITradeService trade_service;

		public TradePageController(IUserService userService, IGameService gameService, ITradeService tradeService)
		{
			user_service = userService;
			game_service = gameService;
			trade_service = tradeService;
		}

		public async Task<IActionResult> Index()
		{
			var currentUser = trade_service.GetCurrentUser();
			if (currentUser == null)
				return RedirectToAction("Login", "Account");

			var allUsers = await user_service.GetAllUsersAsync();
			var games = await game_service.GetAllGamesAsync();

			var trade_view_model = new TradeViewModel
			{
				CurrentUserId = currentUser.UserId,
				Users = allUsers.Select(user => new SelectListItem { Value = user.UserId.ToString(), Text = user.Username }).ToList(),
				AvailableUsers = allUsers.Where(user => user.UserId != currentUser.UserId)
										 .Select(user => new SelectListItem { Value = user.UserId.ToString(), Text = user.Username }).ToList(),
				Games = games.Select(game => new SelectListItem { Value = game.GameId.ToString(), Text = game.GameTitle }).ToList(),
				SourceUserItems = await trade_service.GetUserInventoryAsync(currentUser.UserId),
			};

			return View(trade_view_model);
		}

		public async Task<IActionResult> CreateTradeOffer(TradeViewModel trade_view_model)
		{
			var currentUser = trade_service.GetCurrentUser();
			if (currentUser == null)
			{
				trade_view_model.ErrorMessage = "You must be logged in.";
				return View("Index", await RebuildModel(trade_view_model));
			}

			if (currentUser == null || trade_view_model.SelectedUserId == null)
			{
				trade_view_model.ErrorMessage = "Both users must be selected.";
				return View("Index", await RebuildModel(trade_view_model));
			}

			if (!trade_view_model.SelectedSourceItemIds.Any() && !trade_view_model.SelectedDestinationItemIds.Any())
			{
				trade_view_model.ErrorMessage = "Select at least one item to trade.";
				return View("Index", await RebuildModel(trade_view_model));
			}

			var sourceItems = await trade_service.GetUserInventoryAsync(currentUser.UserId);
			var destinationItems = await trade_service.GetUserInventoryAsync(trade_view_model.SelectedUserId.Value);

			var selectedSourceItems = sourceItems.Where(item => trade_view_model.SelectedSourceItemIds.Contains(item.ItemId)).ToList();
			var selectedDestinationItems = destinationItems.Where(item => trade_view_model.SelectedDestinationItemIds.Contains(item.ItemId)).ToList();

			var sourceUser = new User
			{
				UserId = currentUser.UserId,
				Username = currentUser.Username,
				Email = currentUser.Email,
				UserRole = currentUser.UserRole,
				PointsBalance = currentUser.PointsBalance,
				WalletBalance = currentUser.WalletBalance
			};

			var allUsers = await user_service.GetAllUsersAsync();
            var destinationUser = allUsers.FirstOrDefault(user => user.UserId == trade_view_model.SelectedUserId.Value);
			if(destinationUser == null)
			{
                trade_view_model.ErrorMessage = "Not a valid user.";
                return View("Index", await RebuildModel(trade_view_model));
            }

            var gameOfTrade = await game_service.GetGameByIdAsync(trade_view_model.SelectedGameId ?? 0);
            if (gameOfTrade == null)
            {
                trade_view_model.ErrorMessage = "Not a valid game.";
                return View("Index", await RebuildModel(trade_view_model));
            }

            var trade = new ItemTrade
			{
				SourceUser = sourceUser,
				DestinationUser = destinationUser,
				GameOfTrade = gameOfTrade,
				TradeDescription = trade_view_model.TradeDescription,
				TradeDate = DateTime.UtcNow,
				TradeStatus = "Pending",
				SourceUserItems = selectedSourceItems,
				DestinationUserItems = selectedDestinationItems,
				AcceptedBySourceUser = true,
				AcceptedByDestinationUser = false
			};

			await trade_service.CreateTradeAsync(trade);

			trade_view_model.SuccessMessage = "Trade offer created successfully!";
			return View("Index", await RebuildModel(trade_view_model));
		}

		public async Task<IActionResult> LoadSelectedUser(TradeViewModel model)
		{
			return View("Index", await RebuildModel(model));
		}

		public async Task<IActionResult> LoadSelectedGame(TradeViewModel model)
		{
			return View("Index", await RebuildModel(model));
		}

		private async Task<TradeViewModel> RebuildModel(TradeViewModel trade_view_model)
		{
			var currentUser = trade_service.GetCurrentUser();
			var allUsers = await user_service.GetAllUsersAsync();

			var games = await game_service.GetAllGamesAsync();

			var sourceInventory = await trade_service.GetUserInventoryAsync(currentUser.UserId);
			sourceInventory = sourceInventory.Where(item => item.Game.GameId == trade_view_model.SelectedGameId).ToList();
			var destinationInventory = await trade_service.GetUserInventoryAsync(trade_view_model.SelectedUserId ?? 0);
			destinationInventory = destinationInventory.Where(item => item.Game.GameId == trade_view_model.SelectedGameId).ToList();

			return new TradeViewModel
			{
				CurrentUserId = trade_view_model.CurrentUserId,
				SelectedUserId = trade_view_model.SelectedUserId,
				SelectedGameId = trade_view_model.SelectedGameId,
				TradeDescription = trade_view_model.TradeDescription,
				SelectedSourceItemIds = trade_view_model.SelectedSourceItemIds ?? new(),
				SelectedDestinationItemIds = trade_view_model.SelectedDestinationItemIds ?? new(),
				Users = allUsers.Select(user => new SelectListItem { Value = user.UserId.ToString(), Text = user.Username }).ToList(),
				AvailableUsers = allUsers.Where(user => user.UserId != currentUser.UserId)
										 .Select(user => new SelectListItem { Value = user.UserId.ToString(), Text = user.Username }).ToList(),
				Games = games.Select(game => new SelectListItem { Value = game.GameId.ToString(), Text = game.GameTitle }).ToList(),
				SourceUserItems = sourceInventory,
				DestinationUserItems = destinationInventory,
				SelectedSourceItems = sourceInventory.Where(item => trade_view_model.SelectedSourceItemIds.Contains(item.ItemId)).ToList(),
				SelectedDestinationItems = destinationInventory.Where(item => trade_view_model.SelectedDestinationItemIds.Contains(item.ItemId)).ToList(),
				ErrorMessage = trade_view_model.ErrorMessage,
				SuccessMessage = trade_view_model.SuccessMessage
			};
		}
	}
}
