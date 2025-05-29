using System;
using System.Threading.Tasks;
using System.Diagnostics;
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
    private readonly Frame navigationFrame;

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
    /// Gets or sets whether the registration is in progress.
    /// </summary>
    [ObservableProperty]
    private bool isLoading;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterViewModel"/> class.
    /// </summary>
    /// <param name="navigationFrame">The navigation frame used to move between pages.</param>
    /// <param name="userService">The service used to handle user registration and validation.</param>
    public RegisterViewModel(Frame navigationFrame, IUserService userService)
    {
        UserService = userService;
        this.navigationFrame = navigationFrame;
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
            IsLoading = true;
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

            Debug.WriteLine($"Attempting to register user: {Username}");

            var user = new User
            {
                Username = Username,
                Email = Email,
                Password = Password,
                UserRole = IsDeveloper ? UserRole.Developer : UserRole.User,
                ProfilePicture = string.Empty
            };

            var createdUser = await UserService.CreateUserAsync(user);

            if (createdUser != null)
            {
                Debug.WriteLine($"Registration successful for user: {createdUser.Username}");
                // Navigate to login page on successful registration
                if (navigationFrame != null)
                {
                    var loginPage = new LoginPage(navigationFrame, UserService, null); // null for onLoginSuccess since we're just going back to login
                    navigationFrame.Content = loginPage;
                }
                else
                {
                    Debug.WriteLine("Navigation frame is null - cannot navigate to login page");
                }
            }
            else
            {
                Debug.WriteLine("Registration failed: CreateUserAsync returned null");
                ErrorMessage = "Failed to create account. Please try again.";
            }
        }
        catch (EmailAlreadyExistsException ex)
        {
            Debug.WriteLine($"Registration failed: Email already exists - {ex.Message}");
            ErrorMessage = ex.Message;
        }
        catch (UsernameAlreadyTakenException ex)
        {
            Debug.WriteLine($"Registration failed: Username already taken - {ex.Message}");
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Registration error: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Navigates the user to the login page.
    /// </summary>
    [RelayCommand]
    private void NavigateToLogin()
    {
        if (navigationFrame != null)
        {
            // Create the LoginPage with required parameters and set it as content
            var loginPage = new LoginPage(navigationFrame, UserService, null); // null for onLoginSuccess since we're just going back to login
            navigationFrame.Content = loginPage;
        }
        else
        {
            Debug.WriteLine("Navigation frame is null - cannot navigate to login page");
        }
    }
}