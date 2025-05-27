using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ViewModels;

namespace SteamHub.Pages
{
    /// <summary>
    /// Represents the page for users to initiate a password reset process.
    /// </summary>
    public sealed partial class ForgotPasswordPage : Page
    {
        // ViewModel handling the logic and data-binding for the forgot password functionality
        private readonly ForgotPasswordViewModel forgotPasswordViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForgotPasswordPage"/> class.
        /// </summary>
        /// <param name="passwordResetService">The password reset service injected for handling reset operations.</param>
        public ForgotPasswordPage(IPasswordResetService passwordResetService)
        {
            this.InitializeComponent();

            // Instantiate the ViewModel with the injected password reset service
            forgotPasswordViewModel = new ForgotPasswordViewModel(passwordResetService);

            // Set the ViewModel as the DataContext for data binding in the UI
            this.DataContext = forgotPasswordViewModel;

            // Subscribe to the event that signals a successful password reset
            forgotPasswordViewModel.PasswordResetSuccess += OnPasswordResetSuccess;
        }

        /// <summary>
        /// Event handler called when password reset is successful.
        /// Makes the login button visible so the user can navigate back to the login page.
        /// </summary>
        private void OnPasswordResetSuccess(object sender, System.EventArgs eventArguments)
        {
            GoToLoginButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Click event handler for the "Go To Login" button.
        /// Navigates the user to the LoginPage.
        /// </summary>
        private void GoToLogin_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            this.Frame.Navigate(typeof(LoginPage));
        }

        /// <summary>
        /// Click event handler for the "Back To Login" button.
        /// Navigates the user back to the LoginPage.
        /// </summary>
        private void BackToLogin_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            this.Frame.Navigate(typeof(LoginPage));
        }
    }
}
