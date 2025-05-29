using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.ServiceProxies;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class ForumServiceProxy : ServiceProxy, IForumService
    {
        private readonly IUserService userService;

        public ForumServiceProxy(IUserService userService, string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<int> GetCurrentUserIdAsync()
        {
            var currentUser = await userService.GetCurrentUserAsync();
            return currentUser != null ? (int)currentUser.UserId : 0;
        }

        public async Task<List<ForumPost>> GetPagedPostsAsync(uint pageNumber, uint pageSize, bool positiveScoreOnly = false, int? gameId = null, string? filter = null)
        {
            try
            {
                string queryParams = $"?page={pageNumber}&size={pageSize}&positiveOnly={positiveScoreOnly}";
                if (gameId.HasValue) queryParams += $"&gameId={gameId.Value}";
                if (!string.IsNullOrEmpty(filter)) queryParams += $"&filter={Uri.EscapeDataString(filter)}";
                return await GetAsync<List<ForumPost>>($"Forum/posts{queryParams}");
            }
            catch (Exception)
            {
                return new List<ForumPost>();
            }
        }

        public async Task<List<ForumPost>> GetTopPostsAsync(TimeSpanFilter filter)
        {
            try
            {
                return await GetAsync<List<ForumPost>>($"Forum/top-posts?filter={filter}");
            }
            catch (Exception)
            {
                return new List<ForumPost>();
            }
        }

        public async Task<int> GetPostCountAsync(bool positiveScoreOnly = false, int? gameId = null, string? filter = null)
        {
            try
            {
                string queryParams = $"?positiveOnly={positiveScoreOnly}";
                if (gameId.HasValue) queryParams += $"&gameId={gameId.Value}";
                if (!string.IsNullOrEmpty(filter)) queryParams += $"&filter={Uri.EscapeDataString(filter)}";
                return await GetAsync<int>($"Forum/posts/count{queryParams}");
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task VoteOnPostAsync(int postId, int voteValue)
        {
            try
            {
                await PostAsync($"Forum/posts/{postId}/vote", new { VoteValue = voteValue });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error voting on post: {ex.Message}");
            }
        }

        public async Task VoteOnCommentAsync(int commentId, int voteValue)
        {
            try
            {
                await PostAsync($"Forum/comments/{commentId}/vote", new { VoteValue = voteValue });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error voting on comment: {ex.Message}");
            }
        }

        public async Task<List<ForumComment>> GetCommentsAsync(int postId)
        {
            try
            {
                return await GetAsync<List<ForumComment>>($"Forum/posts/{postId}/comments");
            }
            catch (Exception)
            {
                return new List<ForumComment>();
            }
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            try
            {
                await DeleteAsync<object>($"Forum/comments/{commentId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting comment: {ex.Message}");
            }
        }

        public async Task CreateCommentAsync(string body, int postId, string date)
        {
            try
            {
                await PostAsync("Forum/comments", new { Body = body, PostId = postId, Date = date });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating comment: {ex.Message}");
            }
        }

        public async Task DeletePostAsync(int postId)
        {
            try
            {
                await DeleteAsync<object>($"Forum/posts/{postId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting post: {ex.Message}");
            }
        }

        public async Task CreatePostAsync(string title, string body, string date, int? gameId)
        {
            try
            {
                await PostAsync("Forum/posts", new { Title = title, Body = body, Date = date, GameId = gameId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating post: {ex.Message}");
            }
        }
    }
}