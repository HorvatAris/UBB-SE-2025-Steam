using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Validators;

namespace SteamHub.Web.ViewModels
{
	public class WalletViewModel : IValidatableObject
	{
		private readonly IWalletService _walletService;

		public decimal Balance { get; private set; }
		public int Points { get; private set; }

		[ValidateNever]
		public List<string> PaymentMethods { get; } = new List<string> { "Credit Card", "PayPal" };

		[Required(ErrorMessage = "Payment method is required.")]
		public string SelectedPaymentMethod { get; set; }

		[Required(ErrorMessage = "Amount is required.")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than $0.00.")]
		public decimal? AmountToAdd { get; set; }

		public string? CardNumber { get; set; }
		public string? ExpiryDate { get; set; }
		public string? CVV { get; set; }

		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string? PayPalEmail { get; set; }


		public string BalanceText => $"${Balance:F2}";
		public string PointsText => $"{Points} points";

		public IUserDetails user;

		public WalletViewModel()
		{
			_walletService = null!;
			SelectedPaymentMethod = string.Empty;
			user = null!;
		}

		public WalletViewModel(IWalletService walletService, IUserService userService, IUserDetails user)
		{
			_walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
			SelectedPaymentMethod = string.Empty;
			this.user = user;
		}

		public async Task RefreshWalletData()
		{
			Balance = await _walletService.GetBalance(user.UserId);
			Points = await _walletService.GetPoints(user.UserId);
		}

		public async Task AddFunds(decimal amount)
		{
			await _walletService.AddMoney(amount, user.UserId);
			await RefreshWalletData();
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			return PaymentValidator.ValidatePaymentSubmission(
				SelectedPaymentMethod,
				CardNumber,
				ExpiryDate,
				CVV,
				PayPalEmail);
		}
	}
}