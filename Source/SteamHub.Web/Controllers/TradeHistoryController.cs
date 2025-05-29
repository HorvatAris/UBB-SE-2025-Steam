using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using SteamHub.Web.Services;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.Web.Controllers
{
	[Authorize]
	public class TradeHistoryController : Controller
	{
		private readonly IUserService _userService;
		private readonly ITradeService _tradeService;
		private readonly IUserDetails _userDetails;

		public TradeHistoryController(
			IUserService userService, 
			ITradeService tradeService,
			IUserDetails userDetails)
		{
			_userService = userService;
			_tradeService = tradeService;
			_userDetails = userDetails;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var allUsers = await _userService.GetAllUsersAsync();
			var history = await _tradeService.GetTradeHistoryAsync(_userDetails.UserId);

			var viewModel = new TradeHistoryViewModel
			{
				CurrentUserId = _userDetails.UserId,
				Users = allUsers.Select(user => new SelectListItem
				{
					Value = user.UserId.ToString(),
					Text = user.Username
				}).ToList(),
				TradeHistory = history.Select(tradeHistory => tradeHistory.ToTradeHistoryViewModelDetails()).ToList()
			};

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> LoadUserTradeHistory(TradeHistoryViewModel model)
		{
			var allUsers = await _userService.GetAllUsersAsync();

			var history = model.CurrentUserId.HasValue
				? await _tradeService.GetTradeHistoryAsync(model.CurrentUserId.Value)
				: new List<ApiContract.Models.ItemTrade.ItemTrade>();

			model.Users = allUsers.Select(user => new SelectListItem
			{
				Value = user.UserId.ToString(),
				Text = user.Username
			}).ToList();

			model.TradeHistory = history.Select(tradeHistory => tradeHistory.ToTradeHistoryViewModelDetails()).ToList();

			return View("Index", model);
		}
	}
}
