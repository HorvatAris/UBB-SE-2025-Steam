using System.Collections.Generic;
using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface INewsService
    {
        bool DeleteComment(int commentId);
        bool DeletePost(int postId);
        bool DislikePost(int postId);
        void ExecutePostMethodOnEditMode(bool editMode, string postText, int postId);
        string FormatAsPost(string text);
        bool LikePost(int postId);
        List<Comment> LoadNextComments(int postId);
        List<Post> LoadNextPosts(int pageNumber, string searchedText);
        bool RemoveRatingFromPost(int postId);
        bool SaveComment(int postId, string commentContent);
        bool SavePost(string postContent);
        bool SetCommentMethodOnEditMode(bool editMode, int commentId, int postId, string postText);
        string SetStringOnEditMode(bool editMode);
        bool UpdateComment(int commentId, string newCommentContent);
        bool UpdatePost(int postId, string newPostContent);
    }
}