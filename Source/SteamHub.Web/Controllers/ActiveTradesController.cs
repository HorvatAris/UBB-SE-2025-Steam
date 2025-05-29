using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SteamHub.ApiContract.Models.ItemTrade;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using SteamHub.Web.Services;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.Web.Controllers
{
	[Authorize]
	public class ActiveTradesController : Controller
	{
		private readonly IUserDetails _userDetails;
		private readonly IUserService _userService;
		private readonly ITradeService _tradeService;

		public ActiveTradesController(IUserDetails userDetails, IUserService userService, ITradeService tradeService)
		{
			_userDetails = userDetails;
			_userService = userService;
			_tradeService = tradeService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var users = await _userService.GetAllUsersAsync();
			var activeTrades = await _tradeService.GetActiveTradesAsync(_userDetails.UserId);

			var viewModel = new ActiveTradesViewModel
			{
				CurrentUserId = _userDetails.UserId,
				Users = users.Select(user => new SelectListItem
				{
					Value = user.UserId.ToString(),
					Text = user.Username
				}).ToList(),
				ActiveTrades = activeTrades.Select(trade => trade.ToTradeViewModelDetails()),
				CanAcceptOrDeclineTrade = true
			};

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> LoadUserActiveTrades(ActiveTradesViewModel model)
		{
			var users = await _userService.GetAllUsersAsync();
			var trades = model.CurrentUserId.HasValue
				? await _tradeService.GetActiveTradesAsync(model.CurrentUserId.Value)
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
			var trades = await _tradeService.GetActiveTradesAsync(_userDetails.UserId);
			var trade = trades.FirstOrDefault(trade => trade.TradeId == TradeId);

			if (trade == null)
				return NotFound();

			if (action == "Accept")
			{
				bool isSourceUser = trade.SourceUser.UserId == _userDetails.UserId;
				await _tradeService.AcceptTradeAsync(trade, isSourceUser);
			}
			else if (action == "Decline")
			{
				_tradeService.DeclineTradeRequest(trade); 
			}

			return RedirectToAction("Index", "ActiveTrades");
		}
	}
}
