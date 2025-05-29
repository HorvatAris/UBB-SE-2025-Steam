using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using BusinessLayer.Data;
using BusinessLayer.Repositories;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Services;
using SteamProfile.ViewModels;
using BusinessLayer.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SteamProfile.Views;
using Microsoft.Extensions.Configuration;
using BusinessLayer.DataContext;
using Microsoft.EntityFrameworkCore.SqlServer;
using SteamProfile.Services;
using BusinessLayer.Services.Proxies;
using Microsoft.EntityFrameworkCore.Infrastructure;
using BusinessLayer.Models;

namespace SteamProfile
{
    public partial class App : Application
    {
        // Configuration settings
        private static bool useRemoteServices = false; // Set to true to use proxy services
        private static string apiBaseUrl = "https://localhost:7262/api/";

        // Steam Community part
        private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

        public static void InitViewModels()
        {
            if (UsersViewModel == null)
            {
                // This means its not remote
                UsersViewModel = UsersViewModel.Instance;
                AddGameToCollectionViewModel = new AddGameToCollectionViewModel(CollectionsService, UserService);
                FriendsViewModel = new FriendsViewModel(FriendsService, UserService);
                AddFriendsViewModel = new AddFriendsViewModel(FriendsService, UserService);
                CollectionGamesViewModel = new CollectionGamesViewModel(CollectionsService);
                CollectionsViewModel = new CollectionsViewModel(CollectionsService, UserService);
            }
        }

        private static void ConfigureServices()
        {
            // Build configuration from appsettings.json
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Check if we should use remote services
            if (config["UseRemoteServices"] != null)
            {
                useRemoteServices = bool.Parse(config["UseRemoteServices"]);
            }

            // Get API base URL if specified
            if (config["ApiSettings:BaseUrl"] != null)
            {
                apiBaseUrl = config["ApiSettings:BaseUrl"];
            }

            if (useRemoteServices)
            {
                ConfigureProxyServices();
                return;
            }

            // Continue with local service configuration
            // Read connection string
            var connectionString = config.GetConnectionString("DefaultConnection");

            // Build DbContextOptions for the DataContext
            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            // Instantiate and register EF Core DataContext
            var dataContext = new ApplicationDbContext(dbContextOptions);
            Services[typeof(ApplicationDbContext)] = dataContext;

            // EF Core repositories
            var sessionRepository = new SessionRepository(dataContext);
            Services[typeof(ISessionRepository)] = sessionRepository;

            var walletRepository = new WalletRepository(dataContext);
            Services[typeof(IWalletRepository)] = walletRepository;

            var reviewRepository = new ReviewRepository(dataContext);
            Services[typeof(IReviewRepository)] = reviewRepository;

            var newsRepository = new NewsRepository(dataContext);
            Services[typeof(INewsRepository)] = newsRepository;

            var passwordResetRepository = new PasswordResetRepository(dataContext);
            Services[typeof(IPasswordResetRepository)] = passwordResetRepository;

            var friendshipRepository = new FriendshipsRepository(dataContext);
            Services[typeof(IFriendshipsRepository)] = friendshipRepository;

            var friendRequestRepository = new FriendRequestRepository(dataContext);
            Services[typeof(IFriendRequestRepository)] = friendRequestRepository;

            var userRepository = new UsersRepository(dataContext);
            Services[typeof(IUsersRepository)] = userRepository;

            var featureRepository = new FeaturesRepository(dataContext);
            Services[typeof(IFeaturesRepository)] = featureRepository;

            var collectionsRepository = new CollectionsRepository(dataContext);
            Services[typeof(ICollectionsRepository)] = collectionsRepository;

            var achievementsRepository = new AchievementsRepository(dataContext);
            Services[typeof(IAchievementsRepository)] = achievementsRepository;

            Services[typeof(IForumRepository)] = new ForumRepository(GetService<ApplicationDbContext>());

            Services[typeof(IChatRepository)] = new ChatRepository(GetService<ApplicationDbContext>());

            // This is the old repository that is not used anymore (needs to be removed)
            var friendRepository = new FriendRepository(dataContext);
            Services[typeof(IFriendRepository)] = friendRepository;

            // Configuration
            string currentUsername = "JaneSmith"; // This would come from authentication

            // Register database connection
            var dbConnection = new DatabaseConnection();
            Services[typeof(DatabaseConnection)] = dbConnection;

            // Register repositories
            // Register services
            var friendService = new FriendService(friendRepository);
            Services[typeof(IFriendService)] = friendService;

            var reviewService = new ReviewService(reviewRepository);
            Services[typeof(IReviewService)] = reviewService;

            var sessionService = new SessionService(sessionRepository, userRepository);
            Services[typeof(ISessionService)] = sessionService;

            var userService = new UserService(userRepository, sessionService);
            Services[typeof(IUserService)] = userService;

            var walletService = new WalletService(walletRepository, userService);
            Services[typeof(IWalletService)] = walletService;

            Services[typeof(ICollectionsService)] = new CollectionsService(collectionsRepository);

            var featuresService = new FeaturesService(featureRepository, userService, walletService);
            Services[typeof(IFeaturesService)] = featuresService;

            Services[typeof(INewsService)] = new NewsService(GetService<INewsRepository>(), GetService<IUserService>());

            Services[typeof(IForumService)] = new ForumService(GetService<IForumRepository>());
            // Ensure static instance is initialized for legacy code
            ((ForumService)Services[typeof(IForumService)]).Initialize((ForumService)Services[typeof(IForumService)]);

            var friendRequestService = new FriendRequestService(friendRequestRepository, friendService);
            Services[typeof(IFriendRequestService)] = friendRequestService;

            // Register view models
            var friendRequestViewModel = new FriendRequestViewModel(friendRequestService, currentUsername);
            Services[typeof(FriendRequestViewModel)] = friendRequestViewModel;
        }

        // Configure proxy services for remote API usage
        private static void ConfigureProxyServices()
        {
            // Set the API base URL for all proxies
            ServiceFactory.SetApiBaseUrl(apiBaseUrl);

            // Configure session service first since many others depend on it
            var sessionService = ServiceFactory.CreateSessionService();
            Services[typeof(ISessionService)] = sessionService;
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var connectionString = config.GetConnectionString("DefaultConnection");

            var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            var dataContext = new ApplicationDbContext(dbContextOptions);
            Services[typeof(ApplicationDbContext)] = dataContext;

            var userRepository = new UsersRepository(dataContext);
            Services[typeof(IUsersRepository)] = userRepository;

            var passwordResetRepository = new PasswordResetRepository(dataContext);
            Services[typeof(IPasswordResetRepository)] = passwordResetRepository;

            var collectionsRepository = new CollectionsRepository(dataContext);
            Services[typeof(ICollectionsRepository)] = collectionsRepository;

            // Configure user service next as it's also a common dependency
            var userService = ServiceFactory.CreateUserService();
            Services[typeof(IUserService)] = userService;

            // Configure all other services
            Services[typeof(IFriendRequestService)] = ServiceFactory.CreateFriendRequestService();
            Services[typeof(IWalletService)] = ServiceFactory.CreateWalletService();
            Services[typeof(ICollectionsService)] = ServiceFactory.CreateCollectionsService();
            Services[typeof(IFeaturesService)] = ServiceFactory.CreateFeaturesService();
            Services[typeof(IFriendsService)] = ServiceFactory.CreateFriendsService();
            Services[typeof(IAchievementsService)] = ServiceFactory.CreateAchievementsService();
            Services[typeof(IReviewService)] = ServiceFactory.CreateReviewService();
            Services[typeof(INewsService)] = ServiceFactory.CreateNewsService();
            Services[typeof(IForumService)] = ServiceFactory.CreateForumService();
            Services[typeof(IPasswordResetService)] = ServiceFactory.CreatePasswordResetService();
            Services[typeof(IFriendService)] = ServiceFactory.CreateFriendService();
            Services[typeof(IChatRepository)] = new ChatRepository(GetService<ApplicationDbContext>());
        }

        public static T GetService<T>()
            where T : class
        {
            if (Services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered");
        }

        // Services
        public static IAchievementsService AchievementsService { get; private set; }
        public static IFeaturesService FeaturesService { get; private set; }
        public static ICollectionsService CollectionsService { get; private set; }
        public static IWalletService WalletService { get; private set; }
        public static IUserService UserService { get; private set; }
        public static IFriendsService FriendsService { get; private set; }
        public static AuthenticationService AuthenticationService { get; private set; }
        public static IForumService ForumService { get; private set; }

        // View Models
        public static AddGameToCollectionViewModel AddGameToCollectionViewModel { get; private set; }
        public static CollectionGamesViewModel CollectionGamesViewModel { get; private set; }
        public static CollectionsViewModel CollectionsViewModel { get; private set; }
        public static UsersViewModel UsersViewModel { get; private set; }
        public static FriendsViewModel FriendsViewModel { get; private set; }
        public static AddFriendsViewModel AddFriendsViewModel { get; private set; }

        public static User CurrentUser { get; set; } = new User();

        public static IPasswordResetService PasswordResetService { get; private set; }
        public static ISessionService SessionService { get; private set; }
        public static ICollectionsRepository CollectionsRepository { get; private set; }
        public static PasswordResetRepository PasswordResetRepository { get; private set; }
        public static IUsersRepository UserRepository { get; private set; }

        public static IChatRepository ChatRepository { get; private set; }

        static App()
        {
            // Wire up EF Core and all new repositories and services
            ConfigureServices();

            if (useRemoteServices)
            {
                InitializeRemoteServices();
            }
            else
            {
                InitializeLocalServices();
            }
        }

        private static void InitializeLocalServices()
        {
            var navigationService = NavigationService.Instance;
            var achievementsRepository = GetService<IAchievementsRepository>();

            // EF-Core repositories
            UserRepository = GetService<IUsersRepository>();
            PasswordResetRepository = (PasswordResetRepository)GetService<IPasswordResetRepository>();
            CollectionsRepository = GetService<ICollectionsRepository>();
            ChatRepository = GetService<IChatRepository>();

            // Initialize all services
            SessionService = (SessionService)GetService<ISessionService>();
            UserService = GetService<IUserService>();
            AchievementsService = new AchievementsService(achievementsRepository);
            CollectionsService = GetService<ICollectionsService>();
            AuthenticationService = new AuthenticationService(UserRepository);
            FriendsService = new FriendsService(GetService<IFriendshipsRepository>(), UserService);
            PasswordResetService = new PasswordResetService(PasswordResetRepository, UserService);
            FeaturesService = (FeaturesService)GetService<IFeaturesService>();
            WalletService = new WalletService(GetService<IWalletRepository>(), UserService);
            ForumService = (ForumService)GetService<IForumService>();
            ForumService.Initialize(GetService<IForumService>());

            // Initialize the View Models
            InitViewModels();

            // initialize the achievements off of the EF-based AchievementsService
            InitializeAchievements();
        }

        private static void InitializeRemoteServices()
        {
            // For remote services, we're using the service proxies
            // All services are now implemented as proxies
            var navigationService = NavigationService.Instance;
            UserService = GetService<IUserService>();
            UserRepository = GetService<IUsersRepository>();
            PasswordResetRepository = (PasswordResetRepository)GetService<IPasswordResetRepository>();
            CollectionsRepository = GetService<ICollectionsRepository>();
            ChatRepository = GetService<IChatRepository>();

            // Some services may need a cast to a specific type
            try
            {
                SessionService = GetService<ISessionService>();
            }
            catch
            {
                // ignore
            }

            // Regular service assignments
            try
            {
                AchievementsService = GetService<IAchievementsService>();
            }
            catch
            {
                // ignore
            }

            try
            {
                CollectionsService = GetService<ICollectionsService>();
            }
            catch
            {
                // ignore
            }

            try
            {
                FriendsService = GetService<IFriendsService>();
            }
            catch
            {
                // ignore
            }

            try
            {
                WalletService = GetService<IWalletService>();
            }
            catch
            {
                // ignore
            }

            try
            {
                ForumService = GetService<IForumService>();
            }
            catch
            {
                // ignore
            }

            // Services that need a specific cast
            try
            {
                FeaturesService = GetService<IFeaturesService>();
            }
            catch
            {
                // ignore
            }

            try
            {
                PasswordResetService = GetService<IPasswordResetService>();
            }
            catch
            {
                // ignore
            }
        }
        private static void InitializeAchievements()
        {
            if (!useRemoteServices && AchievementsService != null)
            {
                AchievementsService.InitializeAchievements();
            }
        }

        private Window mainWindow;

        public App()
        {
            this.InitializeComponent();
        }

        public Window MainWindow { get; set; }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            mainWindow = new MainWindow();
            // NavigationService.Instance.Initialize(m_window.Content as Frame); // Ensure the frame is passed
            mainWindow.Activate();
        }
    }

    // Helper class for services that haven't been proxied yet
    public class ThrowingServiceProxy<T> : IDisposable
    {
        private readonly string serviceName;

        public ThrowingServiceProxy(string serviceName)
        {
            this.serviceName = serviceName;
        }

        public void Dispose()
        {
            // No resources to dispose
        }

        // This allows the proxy to be cast to any interface but will throw when methods are called
        public static implicit operator T(ThrowingServiceProxy<T> proxy)
        {
            throw new NotImplementedException($"The {proxy.serviceName} proxy has not been implemented yet. " +
                $"You need to implement a proxy for this service to use it with the remote API.");
        }
    }
}