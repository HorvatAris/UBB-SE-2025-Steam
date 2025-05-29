using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Validators;
using SteamHub.Pages;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ViewModels
{
    public partial class AccountSettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string currentPassword = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private string successMessage = string.Empty;

        [ObservableProperty]
        private string passwordConfirmationError = string.Empty;

        [ObservableProperty]
        private Visibility emailErrorMessageVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility passwordErrorMessageVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility usernameErrorMessageVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility passwordConfirmationErrorVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility successMessageVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private bool updateEmailEnabled = false;

        [ObservableProperty]
        private bool updateUsernameEnabled = false;

        [ObservableProperty]
        private bool updatePasswordEnabled = false;

        // Event to request the View to show the password confirmation dialog
        public event EventHandler RequestPasswordConfirmation;

        private readonly IUserService userService;
        private Func<Task> pendingAction;

        public AccountSettingsViewModel(IUserService userService)
        {
            this.userService = userService;
            _ = LoadUserDataAsync();
        }

        private async Task LoadUserDataAsync()
        {
            var currentUser = await userService.GetCurrentUserAsync();
            if (currentUser != null)
            {
                username = currentUser.Username;
                email = currentUser.Email;
            }
        }

        partial void OnPasswordChanged(string value)
        {
            ValidatePassword(value);
        }

        private void ValidatePassword(string password)
        {
            bool isValid = UserValidator.IsPasswordValid(password);
            PasswordErrorMessageVisibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            UpdatePasswordEnabled = isValid;
        }

        partial void OnEmailChanged(string value)
        {
            ValidateEmail(value);
        }

        private void ValidateEmail(string email)
        {
            bool isValid = UserValidator.IsEmailValid(email);
            EmailErrorMessageVisibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            UpdateEmailEnabled = isValid;
        }

        partial void OnUsernameChanged(string value)
        {
            _ = ValidateUsernameAsync(value);
        }

        private async Task ValidateUsernameAsync(string username)
        {
            bool isValid = await IsValidUsernameAsync(username);
            UsernameErrorMessageVisibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            UpdateUsernameEnabled = isValid;
        }

        private async Task<bool> IsValidUsernameAsync(string username)
        {
            if (!UserValidator.IsValidUsername(username))
            {
                return false;
            }
            var user = await userService.GetUserByUsernameAsync(username);
            return user == null;
        }

        [RelayCommand]
        private async Task UpdateUsername()
        {
            pendingAction = async () =>
            {
                if (await userService.UpdateUserUsernameAsync(Username, CurrentPassword))
                {
                    ShowSuccessMessage("Username updated successfully!");
                }
                else
                {
                    ErrorMessage = "Failed to update username. Please try again.";
                }
            };

            RequestPasswordConfirmation?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private async Task UpdateEmail()
        {
            pendingAction = async () =>
            {
                if (await userService.UpdateUserEmailAsync(Email, CurrentPassword))
                {
                    ShowSuccessMessage("Email updated successfully!");
                }
                else
                {
                    ErrorMessage = "Failed to update email. Please try again.";
                }
            };

            RequestPasswordConfirmation?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private async Task UpdatePassword()
        {
            pendingAction = async () =>
            {
                if (await userService.UpdateUserPasswordAsync(Password, CurrentPassword))
                {
                    ShowSuccessMessage("Password updated successfully!");
                    Password = string.Empty;
                }
                else
                {
                    ErrorMessage = "Failed to update password. Please try again.";
                }
            };

            RequestPasswordConfirmation?.Invoke(this, EventArgs.Empty);
        }
        /*
        [RelayCommand]
        private void Logout()
        {
            userService.Logout();
            NavigationService.Instance.Navigate(typeof(LoginPage));
        }

        [RelayCommand]
        private void DeleteAccount()
        {
            pendingAction = () =>
            {
                userService.DeleteUser(userService.GetCurrentUser().UserId);
                NavigationService.Instance.Navigate(typeof(LoginPage));
            };
            RequestPasswordConfirmation?.Invoke(this, EventArgs.Empty);
        }*/

        private async Task<bool> ValidateCurrentPassword()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                ErrorMessage = "Current password is required.";
                return false;
            }
            return await userService.VerifyUserPasswordAsync(CurrentPassword);
        }

        private void ShowSuccessMessage(string message)
        {
            SuccessMessage = message;
            SuccessMessageVisibility = Visibility.Visible;

            // Hide success message after 3 seconds
            Task.Delay(3000).ContinueWith(_ =>
            {
                SuccessMessageVisibility = Visibility.Collapsed;
                SuccessMessage = string.Empty;
            });
        }

        public async Task<bool> VerifyPassword(string password)
        {
            return await userService.VerifyUserPasswordAsync(password);
        }

        public async Task ExecutePendingAction()
        {
            if (pendingAction != null)
            {
                await pendingAction();
                pendingAction = null;
            }
            CurrentPassword = string.Empty;
        }

        public void CancelPendingAction()
        {
            pendingAction = null;
            CurrentPassword = string.Empty;
        }
    }
}