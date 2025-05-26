using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Repositories;
using BusinessLayer.Models;

namespace BusinessLayer.Services
{
    public class ForumService : IForumService
    {
        private IForumRepository repository;
        private static readonly object Lock = new object();
        private static IForumService instance;

        public static IForumService GetForumServiceInstance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("ForumService has not been initialized. Call Initialize() first.");
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        public void Initialize(IForumService instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            lock (Lock)
            {
                GetForumServiceInstance = instance;
            }
        }

        public ForumService(IForumRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public int GetCurrentUserId()
        {
            return 1;
        }
#nullable enable
        public List<ForumPost> GetPagedPosts(uint pageNumber, uint pageSize, bool positiveScoreOnly = false, int? gameId = null, string? filter = null)
        {
            return repository.GetPagedPosts(pageNumber, pageSize, positiveScoreOnly, gameId, filter);
        }

        public List<ForumPost> GetTopPosts(TimeSpanFilter filter)
        {
            return repository.GetTopPosts(filter);
        }

        public int GetPostCount(bool positiveScoreOnly = false, int? gameId = null, string? filter = null)
        {
            return repository.GetPostCount(positiveScoreOnly, gameId, filter);
        }

        public void VoteOnPost(int postId, int voteValue)
        {
            repository.VoteOnPost(postId, voteValue, (int)GetCurrentUserId());
        }

        public void VoteOnComment(int commentId, int voteValue)
        {
            repository.VoteOnComment(commentId, voteValue, (int)GetCurrentUserId());
        }

        public List<ForumComment> GetComments(int postId)
        {
            return repository.GetComments(postId);
        }

        public void DeleteComment(int commentId)
        {
            repository.DeleteComment(commentId);
        }

        public void CreateComment(string body, int postId, string date)
        {
            repository.CreateComment(body, postId, date, (int)GetCurrentUserId());
        }

        public void DeletePost(int postId)
        {
            repository.DeletePost(postId);
        }

        public void CreatePost(string title, string body, string date, int? gameId)
        {
            repository.CreatePost(title, body, (int)GetCurrentUserId(), date, gameId);
        }
    }
}