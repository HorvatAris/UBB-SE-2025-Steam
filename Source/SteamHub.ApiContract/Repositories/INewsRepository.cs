using SteamHub.ApiContract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Repositories
{
    public interface INewsRepository
    {
        Task<int> AddCommentToPost(int postId, string commentContent, int userId, DateTime commentDate);
        Task<int> AddPostToDatabase(int userId, string postContent, DateTime postDate);
        Task<int> AddRatingToPost(int postId, int userId, int ratingType);
        Task<int> DeleteCommentFromDatabase(int commentId);
        Task<int> DeletePostFromDatabase(int postId);
        Task<List<Comment>> LoadFollowingComments(int postId);
        Task<List<Post>> LoadFollowingPosts(int pageNumber, int userId, string seachedText);
        Task<int> RemoveRatingFromPost(int postId, int userId);
        Task<int> UpdateComment(int commentId, string commentContent);
        Task<int> UpdatePost(int postId, string postContent);
        Task<int> UpdatePostDislikeCount(int postId);
        Task<int> UpdatePostLikeCount(int postId);
    }
}
