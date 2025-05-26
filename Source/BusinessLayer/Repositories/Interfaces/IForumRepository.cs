using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;

namespace BusinessLayer.Repositories.Interfaces
{
    public interface IForumRepository
    {
        public List<ForumPost> GetTopPosts(TimeSpanFilter filter);
        public void CreatePost(string title, string body, int authorId, string date, int? gameId);
        public void DeletePost(int postId);
        public void CreateComment(string body, int postId, string date, int authorId);
        public void DeleteComment(int commentId);
        public void VoteOnPost(int postId, int voteValue, int userId);
        public void VoteOnComment(int commentId, int voteValue, int userId);
        int GetPostCount(bool positiveScoreOnly = false, int? gameId = null, string? filter = null);
        public List<ForumPost> GetPagedPosts(uint pageNumber, uint pageSize, bool positiveScoreOnly = false, int? gameId = null, string? filter = null);
        public List<ForumComment> GetComments(int postId);
    }
}
