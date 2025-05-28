using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;

namespace SteamHub.Web.Controllers
{
	[Authorize]
	public class TradeHistoryController : Controller
	{
		private readonly IUserService user_service;
		private readonly ITradeService trade_service;

		public TradeHistoryController(IUserService userService, ITradeService tradeService)
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

			var allUsers = await user_service.GetAllUsersAsync();
			var history = await trade_service.GetTradeHistoryAsync(currentUser.UserId);

			var trade_history_view_model = new TradeHistoryViewModel
			{
				CurrentUserId = currentUser.UserId,
				Users = allUsers.Select(user => new SelectListItem
				{
					Value = user.UserId.ToString(),
					Text = user.Username
				}).ToList(),
				TradeHistory = history.Select(tradeHistory => tradeHistory.ToTradeHistoryViewModelDetails()).ToList()
			};

			return View(trade_history_view_model);
		}

		[HttpPost]
		public async Task<IActionResult> LoadUserTradeHistory(TradeHistoryViewModel trade_history_view_model)
		{
			var allUsers = await user_service.GetAllUsersAsync();

			var history = trade_history_view_model.CurrentUserId.HasValue
				? await trade_service.GetTradeHistoryAsync(trade_history_view_model.CurrentUserId.Value)
				: new List<ApiContract.Models.ItemTrade.ItemTrade>();

			trade_history_view_model.Users = allUsers.Select(user => new SelectListItem
			{
				Value = user.UserId.ToString(),
				Text = user.Username
			}).ToList();

			trade_history_view_model.TradeHistory = history.Select(tradeHistory => tradeHistory.ToTradeHistoryViewModelDetails()).ToList();

			return View("Index", trade_history_view_model);
		}
	}
}
