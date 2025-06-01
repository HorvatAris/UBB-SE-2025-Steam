using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class NewsServiceProxy : ServiceProxy, INewsService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public NewsServiceProxy(System.Net.Http.IHttpClientFactory httpClientFactory, string baseUrl = "http://172.30.245.56:8000/api/")
            : base(baseUrl)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
        }

        public async Task<bool> DeleteCommentAsync(int commentId, int userId)
        {
            try
            {
                await _httpClient.DeleteAsync($"api/News/{userId}/comments/{commentId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting comment for user: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeletePostAsync(int postId, int userId)
        {
            try
            {
                await _httpClient.DeleteAsync($"api/News/{userId}/posts/{postId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting post for user: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DislikePostAsync(int postId, int userId)
        {
            try
            {
                await _httpClient.PostAsync($"api/News/{userId}/posts/{postId}/dislike", null);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting post for user: {ex.Message}");
                return false;
            }
        }

        public async Task ExecutePostMethodOnEditModeAsync(bool editMode, string postText, int postId, int userId)
        {
            try
            {
                string formattedPost = await FormatAsPostAsync(postText);
                if (editMode && !string.IsNullOrEmpty(postText))
                {
                    await UpdatePostAsync(postId, formattedPost, userId);
                }
                else if (!string.IsNullOrEmpty(postText))
                {
                    await SavePostAsync(formattedPost, userId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing post method: {ex.Message}");
            }
        }

        public async Task<string> FormatAsPostAsync(string text)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(new { Text = text }),
                    Encoding.UTF8,
                    "application/json"
                );
                var response = await _httpClient.PostAsync("api/News/format", content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                // If server formatting fails, provide client-side basic formatting
                return $"{text}";
            }
        }

        public async Task<bool> LikePostAsync(int postId, int userId)
        {
            try
            {
                await _httpClient.PostAsync($"api/News/{userId}/posts/{postId}/like", null);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error liking post: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Comment>> LoadNextCommentsAsync(int postId, int userId)
        {
            try
            {
                Console.WriteLine($"User id: {userId}, Post id: {postId}");
                var response = await _httpClient.GetAsync($"api/News/{userId}/posts/{postId}/comments");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<Comment>>(_options);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving comments", ex);
            }
        }

        public async Task<List<Post>> LoadNextPostsAsync(int pageNumber, string searchedText, int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/News/{userId}/posts?page={pageNumber}&search={Uri.EscapeDataString(searchedText ?? string.Empty)}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<Post>>(_options);
                if (result == null)
                {
                    Console.WriteLine("Server returned null response");
                    return new List<Post>();
                }

                Console.WriteLine($"Successfully loaded {result.Count} posts");
                return result;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network error while loading posts: {ex.Message}");
                throw new Exception("Network error while loading posts. Please check your connection and try again.", ex);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"User id: {userId}");

                throw new Exception("Error retrieving posts", ex);
            }
        }

        public async Task<bool> RemoveRatingFromPostAsync(int postId, int userId)
        {
            try
            {
                await _httpClient.DeleteAsync($"api/News/{userId}/posts/{postId}/rating");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error removing rating from post: ", ex);
                return false;
            }
        }

        public async Task<bool> SaveCommentAsync(int postId, string commentContent, int userId)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(new { Content = commentContent }),
                    Encoding.UTF8,
                    "application/json"
                );

                await _httpClient.PostAsync($"api/News/{userId}/posts/{postId}/comments", content);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving comment: ", ex);
                return false;
            }
        }

        public async Task<bool> SavePostAsync(string postContent, int userId)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(new { Content = postContent }),
                    Encoding.UTF8,
                    "application/json"
                );
                await _httpClient.PostAsync($"api/News/{userId}/posts", content);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving post: ", ex);
                return false;
            }
        }

        public async Task<bool> SetCommentMethodOnEditModeAsync(bool editMode, int commentId, int postId, string commentText, int userId)
        {
            try
            {
                string formattedPost = await FormatAsPostAsync(commentText);
                if (editMode)
                {
                    await UpdateCommentAsync(commentId, formattedPost, userId);
                }
                else
                {
                    await SaveCommentAsync(postId, formattedPost, userId);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error setting comment method on edit mode", ex);
                return false;
            }
        }

        public string SetStringOnEditMode(bool editMode)
        {
            return editMode ? "Save" : "Post Comment";
        }

        public async Task<bool> UpdateCommentAsync(int commentId, string newCommentContent, int userId)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(new { Content = newCommentContent }),
                    Encoding.UTF8,
                    "application/json"
                );
                await _httpClient.PutAsync($"api/News/{userId}/comments/{commentId}", content);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating comment: ", ex);
                return false;
            }
        }

        public async Task<bool> UpdatePostAsync(int postId, string newPostContent, int userId)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(new { Content = newPostContent }),
                    Encoding.UTF8,
                    "application/json"
                );
                await _httpClient.PutAsync($"api/News/{userId}/posts/{postId}", content);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating post: ", ex);
                return false;
            }
        }
    }
}
