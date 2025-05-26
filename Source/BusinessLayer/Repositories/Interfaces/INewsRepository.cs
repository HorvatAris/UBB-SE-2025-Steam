using System;
using System.Collections.Generic;
using BusinessLayer.Models;

namespace BusinessLayer.Repositories.Interfaces
{
    public interface INewsRepository
    {
        int AddCommentToPost(int postId, string commentContent, int userId, DateTime commentDate);
        int AddPostToDatabase(int userId, string postContent, DateTime postDate);
        int AddRatingToPost(int postId, int userId, int ratingType);
        int DeleteCommentFromDatabase(int commentId);
        int DeletePostFromDatabase(int postId);
        List<Comment> LoadFollowingComments(int postId);
        List<Post> LoadFollowingPosts(int pageNumber, int userId, string seachedText);
        int RemoveRatingFromPost(int postId, int userId);
        int UpdateComment(int commentId, string commentContent);
        int UpdatePost(int postId, string postContent);
        int UpdatePostDislikeCount(int postId);
        int UpdatePostLikeCount(int postId);
    }
}