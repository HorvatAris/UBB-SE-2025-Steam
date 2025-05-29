using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SteamHub.ApiContract.Models.User;

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
            if (StaticHttpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                StaticHttpClient.DefaultRequestHeaders.Remove("Authorization");
            }
            StaticHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        #region Synchronous HTTP Methods

        /// <summary>
        /// Sends a GET request synchronously and deserializes the JSON response.
        /// </summary>
        protected T GetSync<T>(string endpoint)
        {
            try
            {
                var response = Task.Run(() => StaticHttpClient.GetAsync(BaseUrl + endpoint)).GetAwaiter().GetResult();
                return HandleResponseSync<T>(response);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GET Error for {endpoint}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Sends a POST request with JSON content synchronously and deserializes the JSON response.
        /// </summary>
        protected T PostSync<T>(string endpoint, object data)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                var response = Task.Run(() => StaticHttpClient.PostAsync(BaseUrl + endpoint, content)).GetAwaiter().GetResult();
                return HandleResponseSync<T>(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Sends a POST request with JSON content synchronously without expecting a response body.
        /// </summary>
        protected void PostSync(string endpoint, object data)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = Task.Run(() => StaticHttpClient.PostAsync(BaseUrl + endpoint, content)).GetAwaiter().GetResult();
            EnsureSuccessStatusCodeSync(response);
        }

        /// <summary>
        /// Sends a PUT request with JSON content synchronously and deserializes the JSON response.
        /// </summary>
        protected T PutSync<T>(string endpoint, object data)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = Task.Run(() => StaticHttpClient.PutAsync(BaseUrl + endpoint, content)).GetAwaiter().GetResult();
            return HandleResponseSync<T>(response);
        }

        /// <summary>
        /// Sends a DELETE request synchronously and deserializes the JSON response.
        /// </summary>
        protected T DeleteSync<T>(string endpoint)
        {
            var response = Task.Run(() => StaticHttpClient.DeleteAsync(BaseUrl + endpoint)).GetAwaiter().GetResult();
            return HandleResponseSync<T>(response);
        }

        /// <summary>
        /// Sends a PATCH request with JSON content synchronously and deserializes the JSON response.
        /// </summary>
        protected T PatchSync<T>(string endpoint, object data)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), BaseUrl + endpoint)
                {
                    Content = content
                };
                var response = Task.Run(() => StaticHttpClient.SendAsync(request)).GetAwaiter().GetResult();
                return HandleResponseSync<T>(response);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"PATCH Error for {endpoint}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Sends a PATCH request with JSON content synchronously without expecting a response body.
        /// </summary>
        protected void PatchSync(string endpoint, object data)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), BaseUrl + endpoint)
            {
                Content = content
            };
            var response = Task.Run(() => StaticHttpClient.SendAsync(request)).GetAwaiter().GetResult();
            EnsureSuccessStatusCodeSync(response);
        }

        #endregion

        #region Asynchronous HTTP Methods

        /// <summary>
        /// Sends a GET request asynchronously and deserializes the JSON response.
        /// </summary>
        protected async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await StaticHttpClient.GetAsync(BaseUrl + endpoint);
            return await HandleResponseAsync<T>(response);
        }

        /// <summary>
        /// Sends a POST request with JSON content asynchronously and deserializes the JSON response.
        /// </summary>
        protected async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await StaticHttpClient.PostAsync(BaseUrl + endpoint, content).ConfigureAwait(false);
            return await HandleResponseAsync<T>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a POST request with JSON content asynchronously without expecting a response body.
        /// </summary>
        protected async Task PostAsync(string endpoint, object data)
        {
            // this is mostly for debugging purposes
            string jsonPayload = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true // for easier debugging
            });
            var content = new StringContent(
                jsonPayload, Encoding.UTF8, "application/json");
            var response = await StaticHttpClient.PostAsync(BaseUrl + endpoint, content).ConfigureAwait(false);
            await EnsureSuccessStatusCodeAsync(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a PUT request with JSON content asynchronously and deserializes the JSON response.
        /// </summary>
        protected async Task<T> PutAsync<T>(string endpoint, object data)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await StaticHttpClient.PutAsync(BaseUrl + endpoint, content).ConfigureAwait(false);
            return await HandleResponseAsync<T>(response).ConfigureAwait(false);
        }

        protected async Task PatchWithoutResponseAsync(string endpoint, object data)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), BaseUrl + endpoint)
            {
                Content = content
            };
            var response = await StaticHttpClient.SendAsync(request).ConfigureAwait(false);
            await EnsureSuccessStatusCodeAsync(response).ConfigureAwait(false);
        }



        /// <summary>
        /// Sends a DELETE request asynchronously and deserializes the JSON response.
        /// </summary>
        protected async Task<T> DeleteAsync<T>(string endpoint)
        {
            var response = await StaticHttpClient.DeleteAsync(BaseUrl + endpoint).ConfigureAwait(false);
            return await HandleResponseAsync<T>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a PATCH request with JSON content asynchronously and deserializes the JSON response.
        /// </summary>
        protected async Task<T> PatchAsync<T>(string endpoint, object data)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), BaseUrl + endpoint)
            {
                Content = content
            };
            var response = await StaticHttpClient.SendAsync(request).ConfigureAwait(false);
            return await HandleResponseAsync<T>(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a PATCH request with JSON content asynchronously without expecting a response body.
        /// </summary>
        protected async Task PatchAsyncWithoutResponse(string endpoint, object data)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), BaseUrl + endpoint)
            {
                Content = content
            };
            var response = await StaticHttpClient.SendAsync(request).ConfigureAwait(false);
            await EnsureSuccessStatusCodeAsync(response).ConfigureAwait(false);
        }

        #endregion

        #region Response Handling

        /// <summary>
        /// Ensures the response status code indicates success, throwing exceptions for error statuses (sync).
        /// </summary>
        private void EnsureSuccessStatusCodeSync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = Task.Run(() => response.Content.ReadAsStringAsync()).GetAwaiter().GetResult();
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        throw new UnauthorizedAccessException("Authentication required");
                    case System.Net.HttpStatusCode.Forbidden:
                        throw new UnauthorizedAccessException("You don't have permission to access this resource");
                    case System.Net.HttpStatusCode.NotFound:
                        throw new Exception("Resource not found");
                    default:
                        throw new Exception($"API error: {response.StatusCode}. {content}");
                }
            }
        }

        /// <summary>
        /// Deserializes a JSON response synchronously after ensuring success.
        /// </summary>
        private T HandleResponseSync<T>(HttpResponseMessage response)
        {
            EnsureSuccessStatusCodeSync(response);
            var json = Task.Run(() => response.Content.ReadAsStringAsync()).GetAwaiter().GetResult();
            Debug.WriteLine($"Response JSON: {json}");
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        /// <summary>
        /// Ensures the response status code indicates success, throwing exceptions for error statuses (async).
        /// </summary>
        private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        throw new UnauthorizedAccessException("Authentication required");
                    case System.Net.HttpStatusCode.Forbidden:
                        throw new UnauthorizedAccessException("You don't have permission to access this resource");
                    case System.Net.HttpStatusCode.NotFound:
                        throw new Exception("Resource not found");
                    default:
                        throw new Exception($"API error: {response.StatusCode}. {content}");
                }
            }
        }

        /// <summary>
        /// Deserializes a JSON response asynchronously after ensuring success.
        /// </summary>
        private async Task<T> HandleResponseAsync<T>(HttpResponseMessage response)
        {
            await EnsureSuccessStatusCodeAsync(response);
            var json = await response.Content.ReadAsStringAsync();
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