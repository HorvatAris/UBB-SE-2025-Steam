using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.IO;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.Services;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using CommunityToolkit.Mvvm.Input;
using SteamProfile.ViewModels.Base;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.Views;
using BusinessLayer.Services;
using BusinessLayer.Validators;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
using Microsoft.UI.Xaml.Media.Animation;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Windows.ApplicationModel.Contacts;
using BusinessLayer.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SteamProfile.ViewModels
{
    public partial class ForgotPasswordViewModel : ObservableObject
    {
        private readonly IPasswordResetService passwordResetService;
        private string email = string.Empty;
        private string resetCode = string.Empty;
        private string newPassword = string.Empty;
        private string confirmPassword = string.Empty;
        private string statusMessage = string.Empty;
        private SolidColorBrush statusColor = new(Colors.Black);
        private bool showEmailSection = true;
        private bool showCodeSection;
        private bool showPasswordSection;
        private bool showLoginButton;

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string ResetCode
        {
            get => resetCode;
            set => SetProperty(ref resetCode, value);
        }

        public string NewPassword
        {
            get => newPassword;
            set => SetProperty(ref newPassword, value);
        }

        public string ConfirmPassword
        {
            get => confirmPassword;
            set => SetProperty(ref confirmPassword, value);
        }

        public string StatusMessage
        {
            get => statusMessage;
            set => SetProperty(ref statusMessage, value);
        }

        public SolidColorBrush StatusColor
        {
            get => statusColor;
            set => SetProperty(ref statusColor, value);
        }

        public bool ShowEmailSection
        {
            get => showEmailSection;
            set => SetProperty(ref showEmailSection, value);
        }

        public bool ShowCodeSection
        {
            get => showCodeSection;
            set => SetProperty(ref showCodeSection, value);
        }

        public bool ShowPasswordSection
        {
            get => showPasswordSection;
            set => SetProperty(ref showPasswordSection, value);
        }

        public bool ShowLoginButton
        {
            get => showLoginButton;
            set => SetProperty(ref showLoginButton, value);
        }

        public event EventHandler PasswordResetSuccess;

        public ForgotPasswordViewModel(IPasswordResetService passwordResetService)
        {
            this.passwordResetService = passwordResetService;
        }

        [RelayCommand]
        private async Task SendResetCodeAsync()
        {
            try
            {
                var result = await passwordResetService.SendResetCode(Email);
                StatusMessage = result.message;
                StatusColor = new SolidColorBrush(result.isValid ? Colors.Green : Colors.Red);

                if (result.isValid)
                {
                    ShowEmailSection = false;
                    ShowCodeSection = true;
                }
            }
            catch
            {
                StatusMessage = "An error occurred while sending the reset code.";
                StatusColor = new SolidColorBrush(Colors.Red);
            }
        }

        [RelayCommand]
        private void VerifyCode()
        {
            try
            {
                var result = passwordResetService.VerifyResetCode(Email, ResetCode);
                StatusMessage = result.message;
                StatusColor = new SolidColorBrush(result.isValid ? Colors.Green : Colors.Red);

                if (result.isValid)
                {
                    ShowCodeSection = false;
                    ShowPasswordSection = true;
                }
            }
            catch (Exception)
            {
                StatusMessage = "An error occurred while verifying the code.";
                StatusColor = new SolidColorBrush(Colors.Red);
            }
        }

        [RelayCommand]
        private void ResetPassword()
        {
            try
            {
                if (NewPassword != ConfirmPassword)
                {
                    StatusMessage = "Passwords do not match.";
                    StatusColor = new SolidColorBrush(Colors.Red);
                    return;
                }

                var result = passwordResetService.ResetPassword(Email, ResetCode, NewPassword);
                StatusMessage = result.message;
                StatusColor = new SolidColorBrush(result.isValid ? Colors.Green : Colors.Red);

                if (result.isValid)
                {
                    ShowPasswordSection = false;
                    ShowLoginButton = true;
                    PasswordResetSuccess?.Invoke(this, EventArgs.Empty);
                }
            }
            catch
            {
                StatusMessage = "An error occurred while resetting the password.";
                StatusColor = new SolidColorBrush(Colors.Red);
            }
        }
    }
}
