using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;
using BusinessLayer.Services;
using SteamProfile.Views;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IUserService userService;
        private readonly Frame loginViewFrame;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string errorMessage;

        public LoginViewModel(Frame frame)
        {
            userService = App.UserService;
            loginViewFrame = frame;
        }

        [RelayCommand]
        private async Task Login()
        {
            try
            {
                ErrorMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Please enter both username and password.";
                    return;
                }

                var user = userService.Login(Username, Password);
                if (user != null)
                {
                    // Navigate to profile page after successful login
                    App.CurrentUser = user;
                    App.InitViewModels();
                    loginViewFrame.Navigate(typeof(ProfilePage), user.UserId);  // !!!!!!!
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
        }

        [RelayCommand]
        private void NavigateToRegister()
        {
            loginViewFrame.Navigate(typeof(RegisterPage));
        }

        [RelayCommand]
        private void NavigateToForgotPassword()
        {
            loginViewFrame.Navigate(typeof(ForgotPasswordPage));
        }
    }
}
