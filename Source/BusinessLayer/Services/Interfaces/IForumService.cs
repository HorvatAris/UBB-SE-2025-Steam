using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface IForumService
    {
        // at the moment, implementations of IForumService must contain a static field
        // that holds the initialised service
        public void Initialize(IForumService forumService);
        public int GetCurrentUserId();
        public List<ForumPost> GetPagedPosts(uint pageNumber, uint pageSize, bool positiveScoreOnly = false, int? gameId = null, string? filter = null);
        public List<ForumPost> GetTopPosts(TimeSpanFilter filter);
        public int GetPostCount(bool positiveScoreOnly = false, int? gameId = null, string? filter = null);
        public void VoteOnPost(int postId, int voteValue);
        public void VoteOnComment(int commentId, int voteValue);
        public List<ForumComment> GetComments(int postId);
        public void DeleteComment(int commentId);
        public void CreateComment(string body, int postId, string date);
        public void DeletePost(int postId);
        public void CreatePost(string title, string body, string date, int? gameId);
    }
}
