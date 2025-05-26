using System;
using System.Collections.Generic;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Services.Proxies
{
    public class NewsServiceProxy : ServiceProxy, INewsService
    {
        private readonly IUserService userService;

        public NewsServiceProxy(IUserService userService, string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public bool DeleteComment(int commentId)
        {
            try
            {
                return DeleteAsync<bool>($"News/comments/{commentId}").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeletePost(int postId)
        {
            try
            {
                return DeleteAsync<bool>($"News/posts/{postId}").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DislikePost(int postId)
        {
            try
            {
                return PostAsync<bool>($"News/posts/{postId}/dislike", null).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void ExecutePostMethodOnEditMode(bool editMode, string postText, int postId)
        {
            try
            {
                if (editMode && !string.IsNullOrEmpty(postText))
                {
                    UpdatePost(postId, FormatAsPost(postText));
                }
                else if (!string.IsNullOrEmpty(postText))
                {
                    SavePost(FormatAsPost(postText));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing post method: {ex.Message}");
            }
        }

        public string FormatAsPost(string text)
        {
            try
            {
                return PostAsync<string>("News/format", new { Text = text }).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                // If server formatting fails, provide client-side basic formatting
                return $"<html><body>{text}</body></html>";
            }
        }

        public bool LikePost(int postId)
        {
            try
            {
                return PostAsync<bool>($"News/posts/{postId}/like", null).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<Comment> LoadNextComments(int postId)
        {
            try
            {
                return GetAsync<List<Comment>>($"News/posts/{postId}/comments").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return new List<Comment>();
            }
        }

        public List<Post> LoadNextPosts(int pageNumber, string searchedText)
        {
            try
            {
                return GetAsync<List<Post>>($"News/posts?page={pageNumber}&search={Uri.EscapeDataString(searchedText ?? string.Empty)}").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return new List<Post>();
            }
        }

        public bool RemoveRatingFromPost(int postId)
        {
            try
            {
                return DeleteAsync<bool>($"News/posts/{postId}/rating").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SaveComment(int postId, string commentContent)
        {
            try
            {
                return PostAsync<bool>($"News/posts/{postId}/comments", new { Content = commentContent }).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SavePost(string postContent)
        {
            try
            {
                return PostAsync<bool>("News/posts", new { Content = postContent }).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SetCommentMethodOnEditMode(bool editMode, int commentId, int postId, string commentText)
        {
            if (editMode)
            {
                return UpdateComment(commentId, FormatAsPost(commentText));
            }
            else
            {
                return SaveComment(postId, FormatAsPost(commentText));
            }
        }

        public string SetStringOnEditMode(bool editMode)
        {
            return editMode ? "Save" : "Post Comment";
        }

        public bool UpdateComment(int commentId, string newCommentContent)
        {
            try
            {
                return PutAsync<bool>($"News/comments/{commentId}", new { Content = newCommentContent }).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdatePost(int postId, string newPostContent)
        {
            try
            {
                return PutAsync<bool>($"News/posts/{postId}", new { Content = newPostContent }).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}