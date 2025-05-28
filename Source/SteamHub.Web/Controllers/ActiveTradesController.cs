using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SteamHub.ApiContract.Models.ItemTrade;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;

namespace SteamHub.Web.Controllers
{
	[Authorize]
	public class ActiveTradesController : Controller
	{
		private readonly IUserService user_service;
		private readonly ITradeService trade_service;

		public ActiveTradesController(IUserService userService, ITradeService tradeService)
		{
			user_service = userService;
			trade_service = tradeService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var currentUser = trade_service.GetCurrentUser();
			if (currentUser == null)
				return RedirectToAction("Login", "Account");

			var users = await user_service.GetAllUsersAsync();
			var activeTrades = await trade_service.GetActiveTradesAsync(currentUser.UserId);

			var trades_view_model = new ActiveTradesViewModel
			{
				CurrentUserId = currentUser.UserId,
				Users = users.Select(user => new SelectListItem
				{
					Value = user.UserId.ToString(),
					Text = user.Username
				}).ToList(),
				ActiveTrades = activeTrades.Select(trade => trade.ToTradeViewModelDetails()),
				CanAcceptOrDeclineTrade = true // Simplified assumption
			};

			return View(trades_view_model);
		}

		[HttpPost]
		public async Task<IActionResult> LoadUserActiveTrades(ActiveTradesViewModel model)
		{
			var users = await user_service.GetAllUsersAsync();
			var trades = model.CurrentUserId.HasValue
				? await trade_service.GetActiveTradesAsync(model.CurrentUserId.Value)
				: new List<ItemTrade>();

			model.Users = users.Select(user => new SelectListItem
			{
				Value = user.UserId.ToString(),
				Text = user.Username
			}).ToList();

			model.ActiveTrades = trades.Select(trade => trade.ToTradeViewModelDetails());
			model.CanAcceptOrDeclineTrade = true;

			return View("Index", model);
		}

		[HttpPost]
		public async Task<IActionResult> RespondToTrade(int TradeId, string action)
		{
			var currentUser = trade_service.GetCurrentUser();
			var trades = await trade_service.GetActiveTradesAsync(currentUser.UserId);
			var trade = trades.FirstOrDefault(trade => trade.TradeId == TradeId);

			if (trade == null)
				return NotFound();

			if (action == "Accept")
			{
				bool isSourceUser = trade.SourceUser.UserId == currentUser.UserId;
				await trade_service.AcceptTradeAsync(trade, isSourceUser);
			}
			else if (action == "Decline")
			{
				trade_service.DeclineTradeRequest(trade); 
			}

			return RedirectToAction("Index", "ActiveTrades");
		}
	}
}
