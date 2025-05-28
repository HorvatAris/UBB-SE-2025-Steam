using System;
using System.Threading.Tasks;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Pages;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.Session;

namespace SteamHub.ViewModels;

/// <summary>
/// ViewModel for handling login-related operations in the UI.
/// </summary>
public partial class LoginViewModel : ObservableObject
{
    private readonly IUserService userService;
    private readonly Frame loginViewFrame;
    private readonly Action<User> onLoginSuccess;

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
    /// Gets or sets whether the login is in progress.
    /// </summary>
    [ObservableProperty]
    private bool isLoading;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
    /// </summary>
    /// <param name="frame">The frame used for navigation.</param>
    /// <param name="userService">Service for authenticating users.</param>
    /// <param name="onLoginSuccess">Action to invoke on successful login.</param>
    public LoginViewModel(Frame frame, IUserService userService, Action<User> onLoginSuccess)
    {
        this.userService = userService;
        this.loginViewFrame = frame;
        this.onLoginSuccess = onLoginSuccess;
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
            IsLoading = true;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both username and password.";
                return;
            }

            Debug.WriteLine($"Attempting login for user: {Username}");
            var loginResult = await userService.LoginAsync(Username, Password);
            
            if (loginResult != null)
            {
                Debug.WriteLine($"Login successful for user: {loginResult.Username}");
                
                // // Get a fresh copy of the user data to ensure we have the most up-to-date information
                // var currentUser = await userService.GetCurrentUserAsync();
                // if (currentUser == null)
                // {
                //     Debug.WriteLine("Failed to get current user after login");
                //     ErrorMessage = "Login failed: Could not retrieve user data";
                //     return;
                // }

                //Debug.WriteLine($"Successfully retrieved current user data for: {currentUser.Username}");
                Debug.WriteLine("Notifying MainWindow of successful login");
                onLoginSuccess?.Invoke(loginResult);
            }
            else
            {
                Debug.WriteLine("Login failed: Invalid credentials");
                ErrorMessage = "Invalid username or password.";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Login error: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Navigates the user to the registration page.
    /// </summary>
    [RelayCommand]
    private void NavigateToRegister()
    {
        if (loginViewFrame == null)
        {
            throw new InvalidOperationException("Navigation frame is not initialized.");
        }
        loginViewFrame.Navigate(typeof(RegisterPage));
    }
}
