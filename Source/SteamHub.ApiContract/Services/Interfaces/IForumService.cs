using System.Collections.Generic;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models;

namespace SteamHub.ApiContract.Services.Interfaces
{
    public interface IForumService
    {
        Task<int> GetCurrentUserIdAsync();
        Task<List<ForumPost>> GetPagedPostsAsync(uint pageNumber, uint pageSize, bool positiveScoreOnly = false, int? gameId = null, string? filter = null);
        Task<List<ForumPost>> GetTopPostsAsync(TimeSpanFilter filter);
        Task<int> GetPostCountAsync(bool positiveScoreOnly = false, int? gameId = null, string? filter = null);
        Task VoteOnPostAsync(int postId, int voteValue);
        Task VoteOnCommentAsync(int commentId, int voteValue);
        Task<List<ForumComment>> GetCommentsAsync(int postId);
        Task DeleteCommentAsync(int commentId);
        Task CreateCommentAsync(string body, int postId, string date);
        Task DeletePostAsync(int postId);
        Task CreatePostAsync(string title, string body, string date, int? gameId);
    }
}
