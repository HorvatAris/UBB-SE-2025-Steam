using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Exceptions;

namespace SteamHub.ApiContract.ServiceProxies
{
    /// <summary>
    /// Base class for service proxies, handling HTTP communication, serialization, and authentication headers.
    /// </summary>
    public class ServiceProxy
    {
        private static readonly HttpClient StaticHttpClient;
        private static string _authToken;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        /// <summary>
        /// Gets or sets the current user session details.
        /// </summary>
        protected static UserWithSessionDetails CurrentUser { get; private set; }

        /// <summary>
        /// Initializes static members of the <see cref="ServiceProxy"/> class.
        /// Configures a shared <see cref="HttpClient"/> with default headers and timeout.
        /// </summary>
        static ServiceProxy()
        {
            StaticHttpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            StaticHttpClient.DefaultRequestHeaders.Accept.Clear();
            StaticHttpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProxy"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL for API endpoints.</param>
        public ServiceProxy(string baseUrl = "https://localhost:7241/api/")
        {
            BaseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            SetAuthTokenSafely(_authToken);
        }

        /// <summary>
        /// Gets the base URL used for all HTTP requests.
        /// </summary>
        protected string BaseUrl { get; }

        /// <summary>
        /// Safely applies the authorization token to the default request headers.
        /// </summary>
        /// <param name="token">The bearer token.</param>
        private void SetAuthTokenSafely(string token)
        {
            if (string.IsNullOrEmpty(token)) return;

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

        #region Asynchronous HTTP Methods

        /// <summary>
        /// Sends a GET request asynchronously and deserializes the JSON response.
        /// </summary>
        protected async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await StaticHttpClient.GetAsync(BaseUrl + endpoint).ConfigureAwait(false);
                return await HandleResponseAsync<T>(response).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"GET Error for {endpoint}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GET Error for {endpoint}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Sends a POST request with JSON content asynchronously and deserializes the JSON response.
        /// </summary>
        protected async Task<T> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                Debug.WriteLine($"POST Request to {endpoint}: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await StaticHttpClient.PostAsync(BaseUrl + endpoint, content).ConfigureAwait(false);
                return await HandleResponseAsync<T>(response).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"POST Error for {endpoint}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"POST Error for {endpoint}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Sends a POST request with JSON content asynchronously without expecting a response body.
        /// </summary>
        protected async Task PostAsync(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                Debug.WriteLine($"POST Request to {endpoint}: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await StaticHttpClient.PostAsync(BaseUrl + endpoint, content).ConfigureAwait(false);
                await EnsureSuccessStatusCodeAsync(response).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"POST Error for {endpoint}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"POST Error for {endpoint}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Sends a PUT request with JSON content asynchronously and deserializes the JSON response.
        /// </summary>
        protected async Task<T> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(data, _jsonOptions), Encoding.UTF8, "application/json");
                var response = await StaticHttpClient.PutAsync(BaseUrl + endpoint, content).ConfigureAwait(false);
                return await HandleResponseAsync<T>(response).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"PUT Error for {endpoint}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PUT Error for {endpoint}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Sends a PUT request with JSON content asynchronously without expecting a response body.
        /// </summary>
        protected async Task PutAsync(string endpoint, object data)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(data, _jsonOptions), Encoding.UTF8, "application/json");
                var response = await StaticHttpClient.PutAsync(BaseUrl + endpoint, content).ConfigureAwait(false);
                await EnsureSuccessStatusCodeAsync(response).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"PUT Error for {endpoint}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PUT Error for {endpoint}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Sends a DELETE request asynchronously and deserializes the JSON response.
        /// </summary>
        protected async Task<T> DeleteAsync<T>(string endpoint)
        {
            try
            {
                var response = await StaticHttpClient.DeleteAsync(BaseUrl + endpoint).ConfigureAwait(false);
                return await HandleResponseAsync<T>(response).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"DELETE Error for {endpoint}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DELETE Error for {endpoint}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Sends a DELETE request asynchronously without expecting a response body.
        /// </summary>
        protected async Task DeleteAsync(string endpoint)
        {
            try
            {
                var response = await StaticHttpClient.DeleteAsync(BaseUrl + endpoint).ConfigureAwait(false);
                await EnsureSuccessStatusCodeAsync(response).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"DELETE Error for {endpoint}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new ServiceException($"Network error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DELETE Error for {endpoint}: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Response Handling

        /// <summary>
        /// Ensures the response status code indicates success, throwing exceptions for error statuses (async).
        /// </summary>
        private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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

        /// <summary>
        /// Deserializes a JSON response asynchronously after ensuring success.
        /// </summary>
        private async Task<T> HandleResponseAsync<T>(HttpResponseMessage response)
        {
            await EnsureSuccessStatusCodeAsync(response).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            Debug.WriteLine($"Response JSON: {json}");
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        #endregion

        /// <summary>
        /// Sets the bearer token for authorization on subsequent requests.
        /// </summary>
        protected void SetAuthToken(string token)
        {
            _authToken = token;
            SetAuthTokenSafely(token);
        }

        /// <summary>
        /// Sets the current user session details.
        /// </summary>
        protected void SetCurrentUser(UserWithSessionDetails user)
        {
            CurrentUser = user;
        }

        /// <summary>
        /// Clears the current user session details.
        /// </summary>
        protected void ClearCurrentUser()
        {
            CurrentUser = null;
        }
    }
}
