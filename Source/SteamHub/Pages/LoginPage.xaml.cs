using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ViewModels;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.Pages
{
    /// <summary>
    /// Represents the login page of the application.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private IUserService userService;

        /// <summary>
        /// Gets the ViewModel used for data binding on this page.
        /// </summary>
        public LoginViewModel ViewModel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPage"/> class.
        /// </summary>
        /// <param name="userService">The user service to handle user-related operations.</param>
        public LoginPage(IUserService userService)
        {
            this.userService = userService;
            this.InitializeComponent();

            // Subscribe to the Loaded event to initialize ViewModel after UI is ready
            this.Loaded += LoginPage_Loaded;
        }

        /// <summary>
        /// Handles the Loaded event of the page.
        /// Initializes the ViewModel and sets the DataContext for data binding.
        /// </summary>
        private void LoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = new LoginViewModel(this.Frame, userService);
            this.DataContext = ViewModel;
        }

        /// <summary>
        /// Handles the Forgot Password button click event.
        /// Navigates to the ForgotPasswordPage.
        /// </summary>
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ForgotPasswordPage));
        }
    }
}