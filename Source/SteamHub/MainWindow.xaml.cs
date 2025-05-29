// <copyright file="MainWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.Common;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.ServiceProxies;
using SteamHub.Pages;
using SteamHub.Web;
using SteamHub.ViewModels;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SteamHub
{
    /// <summary>
    /// Main window.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private User user;
        private GameServiceProxy gameService;
        private CartServiceProxy cartService;
        private UserGameServiceProxy userGameService;
        private DeveloperServiceProxy developerService;
        private PointShopServiceProxy pointShopService;
        private InventoryServiceProxy inventoryService;
        private MarketplaceServiceProxy marketplaceService;
        private TradeServiceProxy tradeService;
        private UserServiceProxy userService;
        private SessionServiceProxy sessionService;
        private FriendsServiceProxy friendsService;

        private FeaturesServiceProxy featuresService;
        private WalletServiceProxy walletService;
        private FriendRequestServiceProxy friendRequestService;
        private AchievementsServiceProxy achievementsService;
        private CollectionsServiceProxy collectionServiceProxy;

        private readonly IHttpClientFactory _httpClientFactory;

        public MainWindow()
        {
            this.InitializeComponent();

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
            };

            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var configuration = configBuilder.Build();
            var services = new ServiceCollection();
            services.AddHttpClient("SteamHubApi", client =>
            {
                client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).ConfigurePrimaryHttpMessageHandler(() => new NoSslCertificateValidationHandler());
            var provider = services.BuildServiceProvider();

            _httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

            this.userService = new UserServiceProxy();
            this.sessionService = new SessionServiceProxy(_httpClientFactory);

            // Start with login page
            ShowLoginPage();
        }

        private void ShowLoginPage()
        {
            // Pass the LoginFrame as the navigation frame for login/register navigation
            var loginPage = new LoginPage(LoginFrame, this.userService, OnLoginSuccess);
            LoginFrame.Content = loginPage;
            LoginOverlay.Visibility = Visibility.Visible;
            NavView.Visibility = Visibility.Collapsed;
        }

        private void ShowRegisterPage()
        {
            // Pass the LoginFrame as the navigation frame - you'll need to update RegisterPage constructor too
            var registerPage = new RegisterPage(LoginFrame, this.userService);
            LoginFrame.Content = registerPage;
        }

        private void LoginFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType == typeof(RegisterPage))
            {
                ShowRegisterPage();
            }
            else if (e.SourcePageType == typeof(LoginPage))
            {
                ShowLoginPage();
            }
        }

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Show confirmation dialog
                var dialog = new ContentDialog
                {
                    Title = "Logout",
                    Content = "Are you sure you want to logout?",
                    PrimaryButtonText = "Yes",
                    SecondaryButtonText = "No",
                    DefaultButton = ContentDialogButton.Secondary,
                    XamlRoot = this.Content.XamlRoot
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    // Call logout on the user service
                    await this.userService.LogoutAsync();

                    // Clear user data
                    this.user = null;

                    // Reset services that require user context
                    this.tradeService = null;
                    this.marketplaceService = null;
                    this.pointShopService = null;
                    this.inventoryService = null;
                    this.cartService = null;
                    this.userGameService = null;
                    this.developerService = null;
                    this.friendsService = null;
                    this.friendRequestService = null;

                    // Show login page
                    ShowLoginPage();
                }
            }
            catch (Exception ex)

            {
                // Show error dialog
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"An error occurred during logout: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }

        private void OnLoginSuccess(User loggedInUser)
        {
            try
            {
                if (loggedInUser == null)
                {
                    throw new ArgumentNullException(nameof(loggedInUser));
                }

                this.user = loggedInUser;
                Debug.WriteLine($"Login successful for user: {loggedInUser.Username}");

                // Initialize services that require the logged-in user
                InitializeUserServices(loggedInUser);

                // Hide login overlay and show main content
                LoginOverlay.Visibility = Visibility.Collapsed;
                NavView.Visibility = Visibility.Visible;

                // Navigate to home page
                NavigateToPage("HomePage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnLoginSuccess: {ex.Message}");
                ShowErrorDialog("Login Error", $"Failed to initialize application: {ex.Message}");
            }
        }

        private void InitializeUserServices(User user)
        {
            Debug.WriteLine("Initializing user services...");

            try
            {
                this.achievementsService = new AchievementsServiceProxy();
                this.tradeService = new TradeServiceProxy();
                this.marketplaceService = new MarketplaceServiceProxy(_httpClientFactory, user);
                this.pointShopService = new PointShopServiceProxy(_httpClientFactory, user);
                this.inventoryService = new InventoryServiceProxy(_httpClientFactory);
                this.gameService = new GameServiceProxy(_httpClientFactory);
                this.cartService = new CartServiceProxy(_httpClientFactory, user);
                this.userGameService = new UserGameServiceProxy(_httpClientFactory, user);
                this.developerService = new DeveloperServiceProxy(_httpClientFactory, user);
                this.friendsService = new FriendsServiceProxy(_httpClientFactory, user);
                this.achievementsService = new AchievementsServiceProxy();
                this.collectionServiceProxy = new CollectionsServiceProxy();
                this.featuresService = new FeaturesServiceProxy(_httpClientFactory);
                this.walletService = new WalletServiceProxy();
                this.friendRequestService = new FriendRequestServiceProxy(_httpClientFactory, user);

                Debug.WriteLine("User services initialized successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing services: {ex.Message}");
                throw new Exception("Failed to initialize user services", ex);
            }
        }

        private void NavigateToPage(string tag)
        {
            try
            {
                if (user == null)
                {
                    Debug.WriteLine("Cannot navigate: No user is logged in");
                    ShowLoginPage();
                    return;
                }

                Debug.WriteLine($"Navigating to page: {tag}");

                switch (tag)
                {
                    case "HomePage":
                        this.ContentFrame.Content = new HomePage(this.gameService, this.cartService, this.userGameService);
                        break;
                    case "CartPage":
                        this.ContentFrame.Content = new CartPage(this.cartService, this.userGameService);
                        break;
                    case "PointsShopPage":
                        this.ContentFrame.Content = new PointsShopPage(this.pointShopService);
                        break;
                    case "WishlistPage":
                        this.ContentFrame.Content = new WishListView(this.userGameService, this.gameService, this.cartService);
                        break;
                    case "DeveloperModePage":
                        this.ContentFrame.Content = new DeveloperModePage(this.developerService);
                        break;
                    case "inventory":
                        this.ContentFrame.Content = new InventoryPage(this.inventoryService, this.userService);
                        break;
                    case "marketplace":
                        this.ContentFrame.Content = new MarketplacePage(this.marketplaceService);
                        break;
                    case "trading":
                        this.ContentFrame.Content = new TradingPage(this.tradeService, this.userService, this.gameService);
                        break;
                    case "friends":
                        this.ContentFrame.Content = new FriendsPage(this.friendsService);
                        break;
                    case "LoginPage":
                        ShowLoginPage();
                        break;
                    case "RegisterPage":
                        ShowRegisterPage();
                        break;

                    case "profile":
                        this.ContentFrame.Content = new ProfilePage(this.userService, friendsService, featuresService, this.collectionServiceProxy, achievementsService, this.user);
                        break;
                    case "AchievementsPage":
                        this.ContentFrame.Content = new AchievementsPage(this.achievementsService, this.userService);
                        break;
                    case "Wallet":
                        this.ContentFrame.Navigate(typeof(WalletPage), (this.walletService, this.userService));
                        break;
                    default:
                        Debug.WriteLine($"Unknown page tag: {tag}");
                        break;
                }

                Debug.WriteLine($"Successfully navigated to {tag}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Navigation error: {ex.Message}");
                ShowErrorDialog("Navigation Error", $"Failed to navigate to page: {ex.Message}");
            }
        }

        private async Task ShowErrorDialog(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await dialog.ShowAsync();
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                var tag = args.SelectedItemContainer.Tag.ToString();

                if (tag == "LoginPage" || tag == "RegisterPage")
                {
                    switch (tag)
                    {
                        case "LoginPage":
                            ShowLoginPage();
                            break;
                        case "RegisterPage":
                            ShowRegisterPage();
                            break;
                    }
                }
                else
                {
                    NavigateToPage(tag);
                }
            }

            if (this.NavView != null)
            {
                this.NavView.SelectedItem = null;
            }
        }

        public void ResetToHomePage()
        {
            NavigateToPage("HomePage");
        }
    }
}