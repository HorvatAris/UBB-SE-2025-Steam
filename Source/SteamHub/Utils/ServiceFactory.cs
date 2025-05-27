using System.IO;

using Microsoft.Extensions.Configuration;
using SteamHub.ApiContract.ServiceProxies;
using SteamHub.ApiContract.Services.Interfaces;


namespace SteamProfile.Services
{
    public static class ServiceFactory
    {
        private static string apiBaseUrl = "https://localhost:7241/api/"; // Default URL

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
            //???
            return new SessionServiceProxy(null);
        }

        // Create user service instance
       

        // Create friend request service instance
       
        //public static IAchievementsService CreateAchievementsService()
        //{
        //    return new AchievementsServiceProxy(apiBaseUrl);
        //}

        // Create owned games service instance
       
        public static IPasswordResetService CreatePasswordResetService()
        {
            return new PasswordResetServiceProxy(apiBaseUrl);
        }
     
    }
}