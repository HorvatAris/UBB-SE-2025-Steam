using System.IO;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Services.Proxies;
using Microsoft.Extensions.Configuration;
using BusinessLayer.Services;
using SteamProfile.Services.Proxies;

namespace SteamProfile.Services
{
    public static class ServiceFactory
    {
        private static string apiBaseUrl = "https://localhost:7262/api/"; // Default URL

        // Initialize the factory with configuration
        static ServiceFactory()
        {
            try
            {
                // load configuration from appsettings.json
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true);

                var configuration = builder.Build();
                var configUrl = configuration["ApiSettings:BaseUrl"];

                if (!string.IsNullOrEmpty(configUrl))
                {
                    apiBaseUrl = configUrl;
                }
            }
            catch
            {
                // In case of any issues, use the default URL
            }
        }

        // Set API base URL manually
        public static void SetApiBaseUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                apiBaseUrl = url;
            }
        }

        // Create session service instance
        public static ISessionService CreateSessionService()
        {
            return new SessionServiceProxy(apiBaseUrl);
        }

        // Create user service instance
        public static IUserService CreateUserService()
        {
            var sessionService = CreateSessionService();
            return new UserServiceProxy(sessionService, apiBaseUrl);
        }

        // Create friend request service instance
        public static IFriendRequestService CreateFriendRequestService()
        {
            return new FriendRequestServiceProxy(apiBaseUrl);
        }

        // Create wallet service instance
        public static IWalletService CreateWalletService()
        {
            var userService = CreateUserService();
            return new WalletServiceProxy(userService, apiBaseUrl);
        }

        // Create collections service instance
        public static ICollectionsService CreateCollectionsService()
        {
            return new CollectionsServiceProxy(apiBaseUrl);
        }

        // Create features service instance
        public static IFeaturesService CreateFeaturesService()
        {
            var userService = CreateUserService();
            return new FeaturesServiceProxy(userService, apiBaseUrl);
        }

        // Create friends service instance
        public static IFriendsService CreateFriendsService()
        {
            var userService = CreateUserService();
            return new FriendsServiceProxy(userService, apiBaseUrl);
        }

        // Create achievements service instance
        public static IAchievementsService CreateAchievementsService()
        {
            return new AchievementsServiceProxy(apiBaseUrl);
        }

        // Create owned games service instance
        public static IOwnedGamesService CreateOwnedGamesService()
        {
            return new OwnedGamesServiceProxy(apiBaseUrl);
        }

        // Create review service instance
        public static IReviewService CreateReviewService()
        {
            return new ReviewServiceProxy(apiBaseUrl);
        }

        // Create news service instance
        public static INewsService CreateNewsService()
        {
            var userService = CreateUserService();
            return new NewsServiceProxy(userService, apiBaseUrl);
        }

        // Create forum service instance
        public static IForumService CreateForumService()
        {
            var userService = CreateUserService();
            return new ForumServiceProxy(userService, apiBaseUrl);
        }

        // Create password reset service instance
        public static IPasswordResetService CreatePasswordResetService()
        {
            return new PasswordResetServiceProxy(apiBaseUrl);
        }

        // Create friend service instance
        public static IFriendService CreateFriendService()
        {
            return new FriendServiceProxy(apiBaseUrl);
        }
        public static TestServiceProxy CreateTestService()
        {
            return new TestServiceProxy(apiBaseUrl);
        }

        public static ChatServiceProxy CreateChatService()
        {
            return new ChatServiceProxy(apiBaseUrl);
        }
    }
}