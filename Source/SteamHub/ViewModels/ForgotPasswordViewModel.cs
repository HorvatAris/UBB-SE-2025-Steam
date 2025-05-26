using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ViewModels;

/// <summary>
/// ViewModel responsible for handling the Forgot Password workflow.
/// It interacts with the password reset service and manages UI state accordingly.
/// </summary>
public partial class ForgotPasswordViewModel : ObservableObject
{
    private readonly IPasswordResetService passwordResetService;

    // Backing fields
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

    // Properties for data binding in the view
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

    // Section visibility flags to control UI flow
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

    /// <summary>
    /// Event triggered when password reset is successful.
    /// Can be used to notify the view or navigate to the login page.
    /// </summary>
    public event EventHandler PasswordResetSuccess;

    /// <summary>
    /// Constructor that receives a password reset service via dependency injection.
    /// </summary>
    public ForgotPasswordViewModel(IPasswordResetService passwordResetService)
    {
        this.passwordResetService = passwordResetService;
    }

    /// <summary>
    /// Sends a reset code to the user's email asynchronously.
    /// Updates the UI status and advances the flow if successful.
    /// </summary>
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

    /// <summary>
    /// Verifies the reset code entered by the user.
    /// If valid, advances the flow to the password reset form.
    /// </summary>
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

    /// <summary>
    /// Resets the user's password if the two passwords match.
    /// Finalizes the flow and notifies listeners on success.
    /// </summary>
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

                // Notify listeners that password reset was successful
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
