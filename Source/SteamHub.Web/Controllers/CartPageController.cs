using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.UsersGames;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using System.Threading.Tasks;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class CartPageController : Controller
    {
        private readonly ICartService cartService;
        private readonly IUserGameService userGameService;
        private readonly IUserDetails user;

        public CartPageController(ICartService cartService, IUserGameService userGameService)
        {
            this.cartService = cartService;
            this.userGameService = userGameService;
            this.user = this.cartService.GetUser(); // neaparat
        }

        public async Task<IActionResult> Index()
        {
            var games = await cartService.GetCartGamesAsync(this.user.UserId);
            var cart_view_model = new CartPageViewModel
            {
                CartGames = games,
                TotalPrice = await cartService.GetTotalSumToBePaidAsync()
            };

            return View(cart_view_model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int gameId)
        {
            var game = (await cartService.GetCartGamesAsync(this.user.UserId)).FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
                var request = new UserGameRequest
                {
                    GameId = game.GameId,
                    UserId = this.user.UserId,
                };
                await cartService.RemoveGameFromCartAsync(request);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(string selectedPaymentMethod)
        {
            if (selectedPaymentMethod == "PayPal")
                return RedirectToAction(nameof(PaypalPayment));

            if (selectedPaymentMethod == "Credit Card")
                return RedirectToAction(nameof(CreditCardPayment));

            if (selectedPaymentMethod == "Steam Wallet")
                return await SteamWalletPayment();

            TempData["Error"] = "Selected payment method is not supported.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> PaypalPayment()
        {
            var paypal_view_model = new PaypalPaymentViewModel
            {
                AmountToPay = await cartService.GetTotalSumToBePaidAsync()
            };

            return View(paypal_view_model);
        }

        [HttpPost]
        public async Task<IActionResult> PaypalPayment(PaypalPaymentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var processor = new PaypalProcessor();
            bool success = await processor.ProcessPaymentAsync(model.Email, model.Password, model.AmountToPay);

            if (success)
            {
                var games = await cartService.GetCartGamesAsync(this.user.UserId);
                var request = new PurchaseGamesRequest
                {
                    UserId = this.user.UserId,
                    Games = games.ToList(),
                    IsWalletPayment = false
                };
                await userGameService.PurchaseGamesAsync(request);
                await cartService.RemoveGamesFromCartAsync(games);

                model.IsSuccess = true;
                model.PointsEarned = userGameService.LastEarnedPoints;
                model.Message = $"Payment successful! You earned {model.PointsEarned} points.";

                return View(model);
            }

            model.IsSuccess = false;
            model.Message = "Payment failed. Please check your credentials.";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreditCardPayment()
        {
            var credit_card_view_model = new CreditCardPaymentViewModel
            {
                TotalAmount = await cartService.GetTotalSumToBePaidAsync()
            };

            return View(credit_card_view_model);
        }

        [HttpPost]
        public async Task<IActionResult> CreditCardPayment(CreditCardPaymentViewModel credit_card_view_model)
        {
            if (!ModelState.IsValid)
                return View(credit_card_view_model);

            var processor = new CreditCardProcessor();
            bool success = await processor.ProcessPaymentAsync(credit_card_view_model.CardNumber, credit_card_view_model.ExpirationDate, credit_card_view_model.CVV, credit_card_view_model.OwnerName);

            if (success)
            {
                var games = await cartService.GetCartGamesAsync(this.user.UserId);
                var request = new PurchaseGamesRequest
                {
                    UserId = this.user.UserId,
                    Games = games.ToList(),
                    IsWalletPayment = false
                };
                await userGameService.PurchaseGamesAsync(request);
                await cartService.RemoveGamesFromCartAsync(games);

                credit_card_view_model.IsSuccess = true;
                credit_card_view_model.PointsEarned = userGameService.LastEarnedPoints;
                credit_card_view_model.Message = $"Payment successful! You earned {credit_card_view_model.PointsEarned} points.";

                return View(credit_card_view_model); // Stay on the same page to show success
            }

            credit_card_view_model.IsSuccess = false;
            credit_card_view_model.Message = "Payment failed. Please try again.";
            return View(credit_card_view_model);
        }

        private async Task<IActionResult> SteamWalletPayment()
        {
            var total = await cartService.GetTotalSumToBePaidAsync();
            if (user.WalletBalance < (float)total)
            {
                TempData["Error"] = "Insufficient Steam Wallet funds.";
                return RedirectToAction(nameof(Index));
            }

            var games = await cartService.GetCartGamesAsync(this.user.UserId);
            var request = new PurchaseGamesRequest
            {
                UserId = this.user.UserId,
                Games = games.ToList(),
                IsWalletPayment = true
            };
            await userGameService.PurchaseGamesAsync(request);
            await cartService.RemoveGamesFromCartAsync(games);

            TempData["PointsEarned"] = userGameService.LastEarnedPoints;
            return RedirectToAction("Index", "HomePage");
        }

    }
}
