using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;
using BusinessLayer.Services;
using SteamProfile.Views;
using BusinessLayer.Exceptions;
using BusinessLayer.Validators;
using Windows.UI.WebUI;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Services.Interfaces;
namespace SteamProfile.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private IWalletService WalletService { get; set; }
        private IUserService UserService { get; set; }
        private IUserProfilesRepository UserProfilesRepository { get; set; }
        private readonly Frame frame;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private string errorMessage;

        // New property for developer status
        [ObservableProperty]
        private bool isDeveloper;

        public RegisterViewModel(Frame frame)
        {
            WalletService = App.WalletService;
            UserService = App.UserService;
            UserProfilesRepository = App.UserProfileRepository;
            this.frame = frame;
        }

        [RelayCommand]
        private async Task Register()
        {
            try
            {
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    ErrorMessage = "All fields are required.";
                    return;
                }

                // Validate email format
                if (!UserValidator.IsEmailValid(Email))
                {
                    ErrorMessage = "Invalid email.";
                    return;
                }

                // Ensure email and username are unique
                UserService.ValidateUserAndEmail(Email, Username);

                // Validate the password
                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Passwords do not match.";
                    return;
                }

                if (!UserValidator.IsPasswordValid(Password))
                {
                    ErrorMessage = "Password must be at least 8 characters long and contain at least one lowercase letter, one uppercase letter, one number, and one special character (@_.,/%^#$!%*?&).";
                    return;
                }

                var user = new User
                {
                    Username = Username,
                    Email = Email,
                    Password = Password,
                    IsDeveloper = IsDeveloper,
                    UserAchievements = new List<UserAchievement>()
                };

                var createdUser = UserService.CreateUser(user);

                if (createdUser != null)
                {
                    // Navigate to login page on successful registration
                    UserProfilesRepository.CreateProfile(createdUser.UserId);
                    WalletService.CreateWallet(createdUser.UserId);
                    frame.Navigate(typeof(LoginPage));
                }
                else
                {
                    ErrorMessage = "Failed to create account. Please try again.";
                }
            }
            catch (EmailAlreadyExistsException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (UsernameAlreadyTakenException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
        }

        [RelayCommand]
        private void NavigateToLogin()
        {
            frame.Navigate(typeof(LoginPage));
        }
    }
}