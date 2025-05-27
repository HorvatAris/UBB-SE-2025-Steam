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
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.Common;
using SteamHub.ApiContract.Proxies;
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
        private PasswordResetServiceProxy passwordResetService;
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
            this.passwordResetService = new PasswordResetServiceProxy();

            if (this.ContentFrame == null)
            {
                throw new Exception("ContentFrame is not initialized.");
            }

            // Start with login page
            ShowLoginPage();
        }

        private void ShowLoginPage()
        {
            var loginPage = new LoginPage(this.userService, OnLoginSuccess);
            this.ContentFrame.Content = loginPage;
        }

        private void OnLoginSuccess(User loggedInUser)
        {
            this.user = loggedInUser;

            // Initialize services that require the logged-in user
            this.tradeService = new TradeServiceProxy(_httpClientFactory, loggedInUser);
            this.marketplaceService = new MarketplaceServiceProxy(_httpClientFactory, loggedInUser);
            this.pointShopService = new PointShopServiceProxy(_httpClientFactory, loggedInUser);
            this.inventoryService = new InventoryServiceProxy(_httpClientFactory, loggedInUser);
            this.gameService = new GameServiceProxy(_httpClientFactory);
            this.cartService = new CartServiceProxy(_httpClientFactory, loggedInUser);
            this.userGameService = new UserGameServiceProxy(_httpClientFactory, loggedInUser);
            this.developerService = new DeveloperServiceProxy(_httpClientFactory, loggedInUser);

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
                        ShowLoginPage();
                        break;
                    case "ForgotPasswordPage":
                        this.ContentFrame.Content = new ForgotPasswordPage(this.passwordResetService);
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