using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;
using BusinessLayer.Services;
using BusinessLayer.Validators;
using SteamProfile.Views;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.ViewModels.ConfigurationsViewModels
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
        private Action pendingAction;

        public AccountSettingsViewModel()
        {
            userService = App.UserService;

            var currentUser = userService.GetCurrentUser();
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
            ValidateUsername(value);
        }

        private void ValidateUsername(string username)
        {
            bool isValid = IsValidUsername(username);
            UsernameErrorMessageVisibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            UpdateUsernameEnabled = isValid;
        }

        private bool IsValidUsername(string username)
        {
            if (!UserValidator.IsValidUsername(username))
            {
                return false;
            }
            return userService.GetUserByUsername(username) == null;
        }

        [RelayCommand]
        private void UpdateUsername()
        {
            pendingAction = () =>
            {
                if (userService.UpdateUserUsername(Username, CurrentPassword))
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
        private void UpdateEmail()
        {
            pendingAction = () =>
            {
                if (userService.UpdateUserEmail(Email, CurrentPassword))
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
        private void UpdatePassword()
        {
            pendingAction = () =>
            {
                if (userService.UpdateUserPassword(Password, CurrentPassword))
                {
                    ShowSuccessMessage("Password updated successfully!");
                    Password = string.Empty; // Clear the password field
                }
                else
                {
                    ErrorMessage = "Failed to update password. Please try again.";
                }
            };

            RequestPasswordConfirmation?.Invoke(this, EventArgs.Empty);
        }

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
        }

        private bool ValidateCurrentPassword()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                ErrorMessage = "Current password is required.";
                return false;
            }
            return userService.VerifyUserPassword(CurrentPassword);
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

        public bool VerifyPassword(string password)
        {
            return userService.VerifyUserPassword(password);
        }

        public void ExecutePendingAction()
        {
            if (pendingAction != null)
            {
                pendingAction();
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