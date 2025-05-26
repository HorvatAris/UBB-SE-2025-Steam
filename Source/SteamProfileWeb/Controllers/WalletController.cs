using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories.Interfaces;
using SteamProfileWeb.ViewModels;
using System;
using Microsoft.AspNetCore.Authorization;

namespace SteamProfileWeb.Controllers
{
    [Authorize]
    public class WalletController : Controller
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
        }

        public IActionResult Index()
        {
            var viewModel = new WalletViewModel(_walletService);
            viewModel.RefreshWalletData();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFunds(WalletViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.AmountToAdd.HasValue)
                {
                    try
                    {
                        _walletService.AddMoney(viewModel.AmountToAdd.Value);
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

            var freshViewModel = new WalletViewModel(_walletService);
            freshViewModel.RefreshWalletData();
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