using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ViewModels;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models.User;
using System;

namespace SteamHub.Pages
{
    /// <summary>
    /// Represents the login page of the application.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private readonly IUserService userService;
        private readonly Action<User> onLoginSuccess;

        /// <summary>
        /// Gets the ViewModel used for data binding on this page.
        /// </summary>
        public LoginViewModel ViewModel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPage"/> class.
        /// </summary>
        /// <param name="userService">The user service to handle user-related operations.</param>
        /// <param name="onLoginSuccess">Callback to be invoked when login is successful.</param>
        public LoginPage(IUserService userService, Action<User> onLoginSuccess)
        {
            this.userService = userService;
            this.onLoginSuccess = onLoginSuccess;
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
            ViewModel = new LoginViewModel(this.Frame, userService, onLoginSuccess);
            this.DataContext = ViewModel;
        }
    }
}