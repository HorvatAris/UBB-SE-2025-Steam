using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Repositories.Interfaces;
using SteamHub.ApiContract.Models;

namespace SteamHub.ApiContract.Services
{
    public class ForumService : IForumService
    {
        private IForumRepository repository;
        private static readonly object Lock = new object();
        private static IForumService instance;

        private static IForumService GetForumServiceInstance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("ForumService has not been initialized. Call Initialize() first.");
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public Task InitializeAsync(IForumService instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            lock (Lock)
            {
                GetForumServiceInstance = instance;
            }
            return Task.CompletedTask;
        }

        public ForumService(IForumRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<int> GetCurrentUserIdAsync()
        {
            // Replace with real user logic if needed
            return Task.FromResult(1);
        }

        public async Task<List<ForumPost>> GetPagedPostsAsync(uint pageNumber, uint pageSize, bool positiveScoreOnly = false, int? gameId = null, string? filter = null)
        {
            return await repository.GetPagedPostsAsync(pageNumber, pageSize, positiveScoreOnly, gameId, filter);
        }

        public async Task<List<ForumPost>> GetTopPostsAsync(TimeSpanFilter filter)
        {
            return await repository.GetTopPostsAsync(filter);
        }

        public async Task<int> GetPostCountAsync(bool positiveScoreOnly = false, int? gameId = null, string? filter = null)
        {
            return await repository.GetPostCountAsync(positiveScoreOnly, gameId, filter);
        }

        public async Task VoteOnPostAsync(int postId, int voteValue)
        {
            await repository.VoteOnPostAsync(postId, voteValue, await GetCurrentUserIdAsync());
        }

        public async Task VoteOnCommentAsync(int commentId, int voteValue)
        {
            await repository.VoteOnCommentAsync(commentId, voteValue, await GetCurrentUserIdAsync());
        }

        public async Task<List<ForumComment>> GetCommentsAsync(int postId)
        {
            return await repository.GetCommentsAsync(postId);
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            await repository.DeleteCommentAsync(commentId);
        }

        public async Task CreateCommentAsync(string body, int postId, string date)
        {
            await repository.CreateCommentAsync(body, postId, date, await GetCurrentUserIdAsync());
        }

        public async Task DeletePostAsync(int postId)
        {
            await repository.DeletePostAsync(postId);
        }

        public async Task CreatePostAsync(string title, string body, string date, int? gameId)
        {
            await repository.CreatePostAsync(title, body, await GetCurrentUserIdAsync(), date, gameId);
        }
    }
}
