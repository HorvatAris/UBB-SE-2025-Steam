using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using SteamHub.ApiContract.Models.ItemTrade;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using SteamHub.Web.Services;
using System.Security.Claims;

namespace SteamHub.Web.Controllers
{
	[Authorize]
	public class TradePageController : Controller
	{
		private readonly IUserService _userService;
		private readonly IGameService _gameService;
		private readonly ITradeService _tradeService;
		private readonly IUserDetails _userDetails;

		public TradePageController(
			IUserService userService, 
			IGameService gameService, 
			ITradeService tradeService,
			IUserDetails userDetails)
		{
			_userService = userService;
			_gameService = gameService;
			_tradeService = tradeService;
			_userDetails = userDetails;
		}

		public async Task<IActionResult> Index()
		{
			var allUsers = await _userService.GetAllUsersAsync();
			var games = await _gameService.GetAllGamesAsync();

			var viewModel = new TradeViewModel
			{
				CurrentUserId = _userDetails.UserId,
				Users = allUsers.Select(user => new SelectListItem { Value = user.UserId.ToString(), Text = user.Username }).ToList(),
				AvailableUsers = allUsers.Where(user => user.UserId != _userDetails.UserId)
										 .Select(user => new SelectListItem { Value = user.UserId.ToString(), Text = user.Username }).ToList(),
				Games = games.Select(game => new SelectListItem { Value = game.GameId.ToString(), Text = game.GameTitle }).ToList(),
				SourceUserItems = await _tradeService.GetUserInventoryAsync(_userDetails.UserId),
			};

			return View(viewModel);
		}

		public async Task<IActionResult> CreateTradeOffer(TradeViewModel model)
		{
			if (model.SelectedUserId == null)
			{
				model.ErrorMessage = "Both users must be selected.";
				return View("Index", await RebuildModel(model));
			}

			if (!model.SelectedSourceItemIds.Any() && !model.SelectedDestinationItemIds.Any())
			{
				model.ErrorMessage = "Select at least one item to trade.";
				return View("Index", await RebuildModel(model));
			}

			var sourceItems = await _tradeService.GetUserInventoryAsync(_userDetails.UserId);
			var destinationItems = await _tradeService.GetUserInventoryAsync(model.SelectedUserId.Value);

			var selectedSourceItems = sourceItems.Where(item => model.SelectedSourceItemIds.Contains(item.ItemId)).ToList();
			var selectedDestinationItems = destinationItems.Where(item => model.SelectedDestinationItemIds.Contains(item.ItemId)).ToList();

			var sourceUser = new User
			{
				UserId = _userDetails.UserId,
				Username = _userDetails.Username,
				Email = _userDetails.Email,
				UserRole = _userDetails.UserRole,
				PointsBalance = _userDetails.PointsBalance,
				WalletBalance = _userDetails.WalletBalance
			};

			var allUsers = await _userService.GetAllUsersAsync();
			var destinationUser = allUsers.FirstOrDefault(user => user.UserId == model.SelectedUserId.Value);
			if(destinationUser == null)
			{
				model.ErrorMessage = "Not a valid user.";
				return View("Index", await RebuildModel(model));
			}

			var gameOfTrade = await _gameService.GetGameByIdAsync(model.SelectedGameId ?? 0);
			if (gameOfTrade == null)
			{
				model.ErrorMessage = "Not a valid game.";
				return View("Index", await RebuildModel(model));
			}

			var trade = new ItemTrade
			{
				SourceUser = sourceUser,
				DestinationUser = destinationUser,
				GameOfTrade = gameOfTrade,
				TradeDescription = model.TradeDescription,
				TradeDate = DateTime.UtcNow,
				TradeStatus = "Pending",
				SourceUserItems = selectedSourceItems,
				DestinationUserItems = selectedDestinationItems,
				AcceptedBySourceUser = true,
				AcceptedByDestinationUser = false
			};

			await _tradeService.CreateTradeAsync(trade);

			model.SuccessMessage = "Trade offer created successfully!";
			return View("Index", await RebuildModel(model));
		}

		public async Task<IActionResult> LoadSelectedUser(TradeViewModel model)
		{
			return View("Index", await RebuildModel(model));
		}

		public async Task<IActionResult> LoadSelectedGame(TradeViewModel model)
		{
			return View("Index", await RebuildModel(model));
		}

		private async Task<TradeViewModel> RebuildModel(TradeViewModel model)
		{
			var allUsers = await _userService.GetAllUsersAsync();
			var games = await _gameService.GetAllGamesAsync();

			var sourceInventory = await _tradeService.GetUserInventoryAsync(_userDetails.UserId);
			sourceInventory = sourceInventory.Where(item => item.Game.GameId == model.SelectedGameId).ToList();
			var destinationInventory = await _tradeService.GetUserInventoryAsync(model.SelectedUserId ?? 0);
			destinationInventory = destinationInventory.Where(item => item.Game.GameId == model.SelectedGameId).ToList();

			return new TradeViewModel
			{
				CurrentUserId = _userDetails.UserId,
				SelectedUserId = model.SelectedUserId,
				SelectedGameId = model.SelectedGameId,
				TradeDescription = model.TradeDescription,
				SelectedSourceItemIds = model.SelectedSourceItemIds ?? new(),
				SelectedDestinationItemIds = model.SelectedDestinationItemIds ?? new(),
				Users = allUsers.Select(user => new SelectListItem { Value = user.UserId.ToString(), Text = user.Username }).ToList(),
				AvailableUsers = allUsers.Where(user => user.UserId != _userDetails.UserId)
										 .Select(user => new SelectListItem { Value = user.UserId.ToString(), Text = user.Username }).ToList(),
				Games = games.Select(game => new SelectListItem { Value = game.GameId.ToString(), Text = game.GameTitle }).ToList(),
				SourceUserItems = sourceInventory,
				DestinationUserItems = destinationInventory,
				SelectedSourceItems = sourceInventory.Where(item => model.SelectedSourceItemIds.Contains(item.ItemId)).ToList(),
				SelectedDestinationItems = destinationInventory.Where(item => model.SelectedDestinationItemIds.Contains(item.ItemId)).ToList(),
				ErrorMessage = model.ErrorMessage,
				SuccessMessage = model.SuccessMessage
			};
		}
	}
}
