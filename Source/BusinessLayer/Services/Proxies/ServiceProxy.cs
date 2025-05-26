using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessLayer.Models;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Services.Proxies
{
    public class ServiceProxy
    {
        protected readonly string baseUrl;
        // Static HttpClient with a reasonable timeout
        private static readonly HttpClient StaticHttpClient;
        private static string authToken;

        // Static constructor to configure the HttpClient once
        static ServiceProxy()
        {
            StaticHttpClient = new HttpClient();
            StaticHttpClient.Timeout = TimeSpan.FromSeconds(30); // Set a 30-second timeout

            // Configure default headers
            StaticHttpClient.DefaultRequestHeaders.Accept.Clear();
            StaticHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Store the session info on the client side
        protected static UserWithSessionDetails CurrentUser { get; private set; }

        public ServiceProxy(string baseUrl = "https://localhost:7262/api/")
        {
            this.baseUrl = baseUrl;

            // Update auth token if needed
            SetAuthTokenSafely(authToken);
        }

        // Safe method to update auth token
        private void SetAuthTokenSafely(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            try
            {
                if (StaticHttpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    StaticHttpClient.DefaultRequestHeaders.Remove("Authorization");
                }

                StaticHttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting auth token: {ex.Message}");
                // Don't throw - just log the error
            }
        }

        protected T GetSync<T>(string endpoint)
        {
            try
            {
                // Use a new task and wait for it, which avoids potential deadlocks
                var task = Task.Run(() => StaticHttpClient.GetAsync($"{baseUrl}{endpoint}"));
                var response = task.GetAwaiter().GetResult();
                return HandleResponseSync<T>(response);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GET Error for {endpoint}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
        }

        protected T PostSync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                Debug.WriteLine($"POST Request to {endpoint}: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Use a new task and wait for it, which avoids potential deadlocks
                var task = Task.Run(() => StaticHttpClient.PostAsync($"{baseUrl}{endpoint}", content));
                var response = task.GetAwaiter().GetResult();

                return HandleResponseSync<T>(response);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"POST Error for {endpoint}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
        }

        protected void PostSync(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                Debug.WriteLine($"POST Request to {endpoint}: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Use a new task and wait for it, which avoids potential deadlocks
                var task = Task.Run(() => StaticHttpClient.PostAsync($"{baseUrl}{endpoint}", content));
                var response = task.GetAwaiter().GetResult();

                EnsureSuccessStatusCodeSync(response);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"POST Error for {endpoint}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
        }

        protected T PutSync<T>(string endpoint, object data)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(data),
                    Encoding.UTF8,
                    "application/json");

                // Use a new task and wait for it, which avoids potential deadlocks
                var task = Task.Run(() => StaticHttpClient.PutAsync($"{baseUrl}{endpoint}", content));
                var response = task.GetAwaiter().GetResult();

                return HandleResponseSync<T>(response);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PUT Error for {endpoint}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
        }

        protected T DeleteSync<T>(string endpoint)
        {
            try
            {
                // Use a new task and wait for it, which avoids potential deadlocks
                var task = Task.Run(() => StaticHttpClient.DeleteAsync($"{baseUrl}{endpoint}"));
                var response = task.GetAwaiter().GetResult();

                return HandleResponseSync<T>(response);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DELETE Error for {endpoint}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
        }

        // Keep the async methods for completeness but we'll use the sync ones
        protected async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await StaticHttpClient.GetAsync($"{baseUrl}{endpoint}").ConfigureAwait(false);
                return await HandleResponse<T>(response).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
        }

        protected async Task<T> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(data),
                    Encoding.UTF8,
                    "application/json");

                var response = await StaticHttpClient.PostAsync($"{baseUrl}{endpoint}", content).ConfigureAwait(false);
                return await HandleResponse<T>(response).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
        }

        protected async Task PostAsync(string endpoint, object data)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(data),
                    Encoding.UTF8,
                    "application/json");

                var response = await StaticHttpClient.PostAsync($"{baseUrl}{endpoint}", content).ConfigureAwait(false);
                await EnsureSuccessStatusCode(response).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
        }

        protected async Task<T> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(data),
                    Encoding.UTF8,
                    "application/json");

                var response = await StaticHttpClient.PutAsync($"{baseUrl}{endpoint}", content).ConfigureAwait(false);
                return await HandleResponse<T>(response).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
        }

        protected async Task<T> DeleteAsync<T>(string endpoint)
        {
            try
            {
                var response = await StaticHttpClient.DeleteAsync($"{baseUrl}{endpoint}").ConfigureAwait(false);
                return await HandleResponse<T>(response).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
        }

        private T HandleResponseSync<T>(HttpResponseMessage response)
        {
            EnsureSuccessStatusCodeSync(response);

            var task = Task.Run(() => response.Content.ReadAsStringAsync());
            var json = task.GetAwaiter().GetResult();

            Debug.WriteLine($"Response: {json}");

            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private void EnsureSuccessStatusCodeSync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var task = Task.Run(() => response.Content.ReadAsStringAsync());
                var content = task.GetAwaiter().GetResult();

                Debug.WriteLine($"Error Response: {content}");

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        throw new UnauthorizedAccessException("Authentication required");
                    case System.Net.HttpStatusCode.Forbidden:
                        throw new UnauthorizedAccessException("You don't have permission to access this resource");
                    case System.Net.HttpStatusCode.NotFound:
                        throw new RepositoryException("Resource not found");
                    default:
                        throw new ServiceException($"API error: {response.StatusCode}. {content}");
                }
            }
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            await EnsureSuccessStatusCode(response).ConfigureAwait(false);

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private async Task EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        throw new UnauthorizedAccessException("Authentication required");
                    case System.Net.HttpStatusCode.Forbidden:
                        throw new UnauthorizedAccessException("You don't have permission to access this resource");
                    case System.Net.HttpStatusCode.NotFound:
                        throw new RepositoryException("Resource not found");
                    default:
                        throw new ServiceException($"API error: {response.StatusCode}. {content}");
                }
            }
        }

        // Method to set the auth token after successful login
        protected void SetAuthToken(string token)
        {
            authToken = token;
            SetAuthTokenSafely(token);
        }

        // Method to store current user session
        protected void SetCurrentUser(UserWithSessionDetails user)
        {
            CurrentUser = user;
        }

        // Method to clear the current user session
        protected void ClearCurrentUser()
        {
            CurrentUser = null;
        }
    }
}