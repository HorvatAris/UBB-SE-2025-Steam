using System;
using System.Threading.Tasks;
using BusinessLayer.Exceptions;
using BusinessLayer.Services.Proxies;

namespace SteamProfile.Services.Proxies
{
    public class TestServiceProxy : ServiceProxy
    {
        public TestServiceProxy(string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
        }

        public string Test()
        {
            try
            {
                return GetAsync<string>("Test").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestCors()
        {
            try
            {
                var response = GetAsync<dynamic>("Test/cors").GetAwaiter().GetResult();
                return $"Success: {response.message} at {response.timestamp}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string Echo(string message)
        {
            try
            {
                var response = GetAsync<dynamic>($"Test/echo?message={Uri.EscapeDataString(message)}").GetAwaiter().GetResult();
                return $"Echo Response: {response.message}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}