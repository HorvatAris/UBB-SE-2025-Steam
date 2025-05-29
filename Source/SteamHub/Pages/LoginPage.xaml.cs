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
        private readonly Frame navigationFrame;

        /// <summary>
        /// Gets the ViewModel used for data binding on this page.
        /// </summary>
        public LoginViewModel ViewModel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPage"/> class.
        /// </summary>
        /// <param name="navigationFrame">The frame used for navigation between login/register pages.</param>
        /// <param name="userService">The user service to handle user-related operations.</param>
        /// <param name="onLoginSuccess">Callback to be invoked when login is successful.</param>
        public LoginPage(Frame navigationFrame, IUserService userService, Action<User> onLoginSuccess)
        {
            this.navigationFrame = navigationFrame;
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
            ViewModel = new LoginViewModel(navigationFrame, userService, onLoginSuccess);
            this.DataContext = ViewModel;
        }
    }
}