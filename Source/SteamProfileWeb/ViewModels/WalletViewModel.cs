using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Validators;

using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SteamProfileWeb.ViewModels
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

        // PayPal Details
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? PayPalEmail { get; set; }


        public string BalanceText => $"${Balance:F2}";
        public string PointsText => $"{Points} points";

        // Parameterless constructor for model binding
        public WalletViewModel()
        {
            _walletService = null!;
            SelectedPaymentMethod = string.Empty;
        }

        // Constructor to be called by the Controller for initial view rendering or when repopulating after error
        public WalletViewModel(IWalletService walletService)
        {
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            SelectedPaymentMethod = string.Empty;
        }


        public void RefreshWalletData()
        {
            Balance = _walletService.GetBalance();
            Points = _walletService.GetPoints();
        }

        public void AddFunds(decimal amount)
        {
            _walletService.AddMoney(amount);
            RefreshWalletData();
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