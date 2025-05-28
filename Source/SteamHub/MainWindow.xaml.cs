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
        private FriendServiceProxy friendsService;
        private FeaturesServiceProxy featuresService;
        private WalletServiceProxy walletService;
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

            this.userService = new UserServiceProxy(_httpClientFactory);
            this.sessionService = new SessionServiceProxy(_httpClientFactory);

            // Start with login page
            ShowLoginPage();
        }

        private void ShowLoginPage()
        {
            var loginPage = new LoginPage(this.userService, OnLoginSuccess);
            LoginFrame.Content = loginPage;
            LoginOverlay.Visibility = Visibility.Visible;
            NavView.Visibility = Visibility.Collapsed;
        }

        private void ShowRegisterPage()
        {
            var registerPage = new RegisterPage(this.userService);
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
            this.user = loggedInUser;

            // Initialize services that require the logged-in user
            this.achievementsService = new AchievementsServiceProxy(_httpClientFactory);
            this.tradeService = new TradeServiceProxy(_httpClientFactory, loggedInUser);
            this.marketplaceService = new MarketplaceServiceProxy(_httpClientFactory, loggedInUser);
            this.pointShopService = new PointShopServiceProxy(_httpClientFactory, loggedInUser);
            this.inventoryService = new InventoryServiceProxy(_httpClientFactory, loggedInUser);
            this.gameService = new GameServiceProxy(_httpClientFactory);
            this.cartService = new CartServiceProxy(_httpClientFactory, loggedInUser);
            this.userGameService = new UserGameServiceProxy(_httpClientFactory, loggedInUser);
            this.developerService = new DeveloperServiceProxy(_httpClientFactory, loggedInUser);
            this.friendsService = new FriendServiceProxy();
            this.achievementsService = new AchievementsServiceProxy(_httpClientFactory);
            this.collectionServiceProxy = new CollectionsServiceProxy();
            this.featuresService = new FeaturesServiceProxy(_httpClientFactory);
            this.walletService = new WalletServiceProxy(_httpClientFactory, loggedInUser);

            // Hide login overlay and show main content
            LoginOverlay.Visibility = Visibility.Collapsed;
            NavView.Visibility = Visibility.Visible;

            // Navigate to home page
            this.ContentFrame.Content = new HomePage(this.gameService, this.cartService, this.userGameService);
        }


        public void ResetToHomePage()
        {
            this.ContentFrame.Content = new HomePage(this.gameService, this.cartService, this.userGameService);
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                var tag = args.SelectedItemContainer.Tag.ToString();
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
                        this.ContentFrame.Content = new InventoryPage(this.inventoryService);
                        break;
                    case "marketplace":
                        this.ContentFrame.Content = new MarketplacePage(this.marketplaceService);
                        break;
                    case "trading":
                        this.ContentFrame.Content = new TradingPage(this.tradeService, this.userService, this.gameService);
                        break;
                    case "LoginPage":
                        ShowLoginPage();
                        break;
                    case "RegisterPage":
                        ShowRegisterPage();
                        break;
                    case "profile":
                        this.ContentFrame.Content = new ProfilePage(this.userService, friendsService, featuresService,this.collectionServiceProxy, achievementsService, this.user);
                        break;
                    case "ForgotPasswordPage":
                        ShowLoginPage();
                        break;
                    case "AchievementsPage":
                        this.ContentFrame.Content = new AchievementsPage(this.userService, this.achievementsService);
                        break;
                    case "Wallet":
                        this.ContentFrame.Navigate(typeof(WalletPage), this.walletService);
                        break;
                    case "CollectionsPage":
                        this.ContentFrame.Content = new CollectionsPage(this.collectionServiceProxy , this.userService);
                        break;
                }
            }

            if (this.NavView != null)
            {
                this.NavView.SelectedItem = null;
            }
        }
    }
}