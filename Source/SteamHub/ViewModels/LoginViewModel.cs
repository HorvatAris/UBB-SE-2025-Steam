using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Pages;

namespace SteamHub.ViewModels;

/// <summary>
/// ViewModel for handling login-related operations in the UI.
/// </summary>
public partial class LoginViewModel : ObservableObject
{
    private readonly IUserService userService;
    private readonly Frame loginViewFrame;

    /// <summary>
    /// Gets or sets the username entered by the user.
    /// </summary>
    [ObservableProperty]
    private string username;

    /// <summary>
    /// Gets or sets the password entered by the user.
    /// </summary>
    [ObservableProperty]
    private string password;

    /// <summary>
    /// Gets or sets the error message to display to the user.
    /// </summary>
    [ObservableProperty]
    private string errorMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
    /// </summary>
    /// <param name="frame">The frame used for navigation.</param>
    /// <param name="userService">Service for authenticating users.</param>
    public LoginViewModel(Frame frame, IUserService userService)
    {
        this.userService = userService;
        loginViewFrame = frame;
    }

    /// <summary>
    /// Attempts to log in the user with the provided username and password.
    /// Displays an error message if the login fails.
    /// </summary>
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
                // TODO: Navigate to profile page after successful login
                // loginViewFrame.Navigate(typeof(ProfilePage), user.UserId);
                ErrorMessage = "Logged In Succesfully";
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

    /// <summary>
    /// Navigates the user to the registration page.
    /// </summary>
    [RelayCommand]
    private void NavigateToRegister()
    {
        loginViewFrame.Navigate(typeof(RegisterPage));
    }

    /// <summary>
    /// Navigates the user to the forgot password page.
    /// </summary>
    [RelayCommand]
    private void NavigateToForgotPassword()
    {
        loginViewFrame.Navigate(typeof(ForgotPasswordPage));
    }
}
