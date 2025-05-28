using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.Common;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Validators;
using SteamHub.Pages;

namespace SteamHub.ViewModels;

/// <summary>
/// ViewModel responsible for handling user registration logic and navigation.
/// </summary>
public partial class RegisterViewModel : ObservableObject
{
    private IUserService UserService { get; set; }
    private readonly Frame frame;

    /// <summary>
    /// Gets or sets the username entered by the user.
    /// </summary>
    [ObservableProperty]
    private string username;

    /// <summary>
    /// Gets or sets the email entered by the user.
    /// </summary>
    [ObservableProperty]
    private string email;

    /// <summary>
    /// Gets or sets the password entered by the user.
    /// </summary>
    [ObservableProperty]
    private string password;

    /// <summary>
    /// Gets or sets the confirmation password entered by the user.
    /// </summary>
    [ObservableProperty]
    private string confirmPassword;

    /// <summary>
    /// Gets or sets the error message to display in the UI.
    /// </summary>
    [ObservableProperty]
    private string errorMessage;

    /// <summary>
    /// Gets or sets a value indicating whether the user is registering as a developer.
    /// </summary>
    [ObservableProperty]
    private bool isDeveloper;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterViewModel"/> class.
    /// </summary>
    /// <param name="frame">The navigation frame used to move between pages.</param>
    /// <param name="userService">The service used to handle user registration and validation.</param>
    public RegisterViewModel(Frame frame, IUserService userService)
    {
        UserService = userService;
        this.frame = frame;
    }

    /// <summary>
    /// Attempts to register a new user after validating all input fields.
    /// Navigates to the login page if successful.
    /// Displays relevant error messages if validation or registration fails.
    /// </summary>
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
            await UserService.ValidateUserAndEmailAsync(Email, Username);

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
                UserRole = IsDeveloper ? UserRole.Developer : UserRole.User
            };

            var createdUser = await UserService.CreateUserAsync(user);

            if (createdUser != null)
            {
                // Navigate to login page on successful registration
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

    /// <summary>
    /// Navigates the user to the login page.
    /// </summary>
    [RelayCommand]
    private void NavigateToLogin()
    {
        frame.Navigate(typeof(LoginPage));
    }
}
