using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.Web.Controllers
{
	[Authorize]
	public class WalletController : Controller
	{
		private readonly IWalletService _walletService;
		private readonly IUserService _userService;
		private readonly IUserDetails _user;

		public WalletController(IWalletService walletService, IUserService userService, IUserDetails user)
		{
			_walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
			_user = user;
        }

		public async Task<IActionResult> Index()
		{
			var viewModel = new WalletViewModel(_walletService, _userService, _user);
			await viewModel.RefreshWalletData();
			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddFunds(WalletViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				if (viewModel.AmountToAdd.HasValue)
				{
					try
					{
                        await _walletService.AddMoney(viewModel.AmountToAdd.Value, _user.UserId);
						TempData["SuccessMessage"] = $"Successfully added ${viewModel.AmountToAdd:F2} to your wallet using {viewModel.SelectedPaymentMethod}.";
						return RedirectToAction(nameof(Index));
					}
					catch (ArgumentOutOfRangeException ex)
					{
						ModelState.AddModelError(nameof(viewModel.AmountToAdd), "Amount cannot be greater than 500.");
					}
				}
				else
				{
					TempData["AddFundsError"] = "Amount to add cannot be null.";
				}
			}

			var freshViewModel = new WalletViewModel(_walletService, _userService, _user);
			await freshViewModel.RefreshWalletData();
			freshViewModel.AmountToAdd = viewModel.AmountToAdd;
			freshViewModel.SelectedPaymentMethod = viewModel.SelectedPaymentMethod;
			freshViewModel.CardNumber = viewModel.CardNumber;
			freshViewModel.ExpiryDate = viewModel.ExpiryDate;
			freshViewModel.CVV = viewModel.CVV;
			freshViewModel.PayPalEmail = viewModel.PayPalEmail;

			return View("Index", freshViewModel);
		}

	}
}