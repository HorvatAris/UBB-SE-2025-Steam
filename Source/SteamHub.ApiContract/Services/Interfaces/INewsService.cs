using SteamHub.ApiContract.Models;

namespace SteamHub.ApiContract.Services.Interfaces
{
    public interface INewsService
    {
        Task<bool> DeleteCommentAsync(int commentId);
        Task<bool> DeletePostAsync(int postId);
        Task<bool> DislikePostAsync(int postId, int userId);
        Task ExecutePostMethodOnEditModeAsync(bool editMode, string postContent, int postId, int userId);
        Task<string> FormatAsPostAsync(string text);
        Task<bool> LikePostAsync(int postId, int userId);
        Task<List<Comment>> LoadNextCommentsAsync(int postId);
        Task<List<Post>> LoadNextPostsAsync(int pageNumber, string searchedText, int userId);
        Task<bool> RemoveRatingFromPostAsync(int postId, int userId);
        Task<bool> SaveCommentAsync(int postId, string commentContent, int userId);
        Task<bool> SavePostAsync(string postContent, int userId);
        Task<bool> SetCommentMethodOnEditModeAsync(bool editMode, int commentId, int postId, string commentContent, int userId);
        string SetStringOnEditMode(bool editMode);
        Task<bool> UpdateCommentAsync(int commentId, string newCommentContent);
        Task<bool> UpdatePostAsync(int postId, string newPostContent);
    }
}