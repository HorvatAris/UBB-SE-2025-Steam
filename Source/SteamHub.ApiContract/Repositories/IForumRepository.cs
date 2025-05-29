using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models;

namespace SteamHub.ApiContract.Repositories.Interfaces
{
    public interface IForumRepository
    {
        Task<List<ForumPost>> GetTopPostsAsync(TimeSpanFilter filter);
        Task CreatePostAsync(string title, string body, int authorId, string date, int? gameId);
        Task DeletePostAsync(int postId);
        Task CreateCommentAsync(string body, int postId, string date, int authorId);
        Task DeleteCommentAsync(int commentId);
        Task VoteOnPostAsync(int postId, int voteValue, int userId);
        Task VoteOnCommentAsync(int commentId, int voteValue, int userId);
        Task<int> GetPostCountAsync(bool positiveScoreOnly = false, int? gameId = null, string? filter = null);
        Task<List<ForumPost>> GetPagedPostsAsync(uint pageNumber, uint pageSize, bool positiveScoreOnly = false, int? gameId = null, string? filter = null);
        Task<List<ForumComment>> GetCommentsAsync(int postId);
    }
}