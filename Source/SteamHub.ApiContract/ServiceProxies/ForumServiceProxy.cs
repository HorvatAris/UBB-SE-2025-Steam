using System;
using System.Collections.Generic;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.ServiceProxies;

namespace SteamHub.ApiContract.Services.Proxies
{
    public class ForumServiceProxy : ServiceProxy, IForumService
    {
        private readonly IUserService userService;

        public ForumServiceProxy(IUserService userService, string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public int GetCurrentUserId()
        {
            var currentUser = userService.GetCurrentUserAsync().GetAwaiter().GetResult();
            return currentUser != null ? (int)currentUser.UserId : 0;
        }

#nullable enable
        public List<ForumPost> GetPagedPosts(uint pageNumber, uint pageSize, bool positiveScoreOnly = false, int? gameId = null, string? filter = null)
        {
            try
            {
                string queryParams = $"?page={pageNumber}&size={pageSize}&positiveOnly={positiveScoreOnly}";

                if (gameId.HasValue)
                {
                    queryParams += $"&gameId={gameId.Value}";
                }

                if (!string.IsNullOrEmpty(filter))
                {
                    queryParams += $"&filter={Uri.EscapeDataString(filter)}";
                }

                return GetAsync<List<ForumPost>>($"Forum/posts{queryParams}").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return new List<ForumPost>();
            }
        }

        public List<ForumPost> GetTopPosts(TimeSpanFilter filter)
        {
            try
            {
                return GetAsync<List<ForumPost>>($"Forum/top-posts?filter={filter}").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return new List<ForumPost>();
            }
        }

        public int GetPostCount(bool positiveScoreOnly = false, int? gameId = null, string? filter = null)
        {
            try
            {
                string queryParams = $"?positiveOnly={positiveScoreOnly}";

                if (gameId.HasValue)
                {
                    queryParams += $"&gameId={gameId.Value}";
                }
                if (!string.IsNullOrEmpty(filter))
                {
                    queryParams += $"&filter={Uri.EscapeDataString(filter)}";
                }

                return GetAsync<int>($"Forum/posts/count{queryParams}").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public void VoteOnPost(int postId, int voteValue)
        {
            try
            {
                PostAsync($"Forum/posts/{postId}/vote", new { VoteValue = voteValue }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error voting on post: {ex.Message}");
            }
        }

        public void VoteOnComment(int commentId, int voteValue)
        {
            try
            {
                PostAsync($"Forum/comments/{commentId}/vote", new { VoteValue = voteValue }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error voting on comment: {ex.Message}");
            }
        }

        public List<ForumComment> GetComments(int postId)
        {
            try
            {
                return GetAsync<List<ForumComment>>($"Forum/posts/{postId}/comments").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return new List<ForumComment>();
            }
        }

        public void DeleteComment(int commentId)
        {
            try
            {
                DeleteAsync<object>($"Forum/comments/{commentId}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting comment: {ex.Message}");
            }
        }

        public void CreateComment(string body, int postId, string date)
        {
            try
            {
                PostAsync("Forum/comments", new
                {
                    Body = body,
                    PostId = postId,
                    Date = date
                }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating comment: {ex.Message}");
            }
        }

        public void DeletePost(int postId)
        {
            try
            {
                DeleteAsync<object>($"Forum/posts/{postId}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting post: {ex.Message}");
            }
        }

        public void CreatePost(string title, string body, string date, int? gameId)
        {
            try
            {
                PostAsync("Forum/posts", new
                {
                    Title = title,
                    Body = body,
                    Date = date,
                    GameId = gameId
                }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating post: {ex.Message}");
            }
        }

        public void Initialize(IForumService instance)
        {
            // This method is only called on the server side
        }
    }
}