using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Services
{
    public class NewsService : INewsService
    {
        private INewsRepository repository;

        private const int POSITIVE_RATING = 1;
        private const int NEGATIVE_RATING = 0;
        private const int DEFAULT_ROWS_AFFECTED_VALUE = 0;
        private const int SUCCESSFUL_EXECUTIONS = 1;
        private const string BUTTON_CONTENT_SAVE = "Save";
        private const string BUTTON_CONTENT_POST = "Post Comment";
        private const string EMPTY_STRING = "";
        public const int PAGE_SIZE = 9;

        public NewsService(INewsRepository newRepository)
        {
            repository = newRepository;
        }

        /// <summary>
        /// Format given text to valid html to be ready to post
        /// </summary>
        /// <param name="text">Given text as string</param>
        /// <returns>New parsed text</returns>
        public Task<string> FormatAsPostAsync(string text)
        {
            SanitizeHtml(ref text);
            ConvertSpecialTagsToHtml(ref text);
            string parsedText = text;
            return Task.FromResult(parsedText);
        }

        /// <summary>
        /// Update the posts like count and add the rating to the post
        /// </summary>
        /// <param name="postId">Target post</param>
        /// <returns>True if the executions were successful, false otherwise</returns>
        public async Task<bool> LikePostAsync(int postId, int userId)
        {
            int rowsAffected = DEFAULT_ROWS_AFFECTED_VALUE;
            rowsAffected += await repository.AddRatingToPost(postId, userId, POSITIVE_RATING);

            if (rowsAffected == SUCCESSFUL_EXECUTIONS)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Update the posts dislike count and add the rating to the post
        /// </summary>
        /// <param name="postId">Target post</param>
        /// <returns>True if the executions were successful, false otherwise</returns>
        public async Task<bool> DislikePostAsync(int postId, int userId)
        {
            int rowsAffected = DEFAULT_ROWS_AFFECTED_VALUE;

            rowsAffected += await repository.AddRatingToPost(postId, userId, NEGATIVE_RATING);

            if (rowsAffected == SUCCESSFUL_EXECUTIONS)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Remove the active user's rating from the post
        /// </summary>
        /// <param name="postId">Target post</param>
        /// <returns>True if the executions were successful, false otherwise</returns>
        public async Task<bool> RemoveRatingFromPostAsync(int postId, int userId)
        {
            int rowsAffected = DEFAULT_ROWS_AFFECTED_VALUE;

            rowsAffected += await repository.RemoveRatingFromPost(postId, userId);

            return CheckRowsAffected(rowsAffected);
        }

        /// <summary>
        /// Add a new comment to the post by the active user
        /// </summary>
        /// <param name="postId">Target post</param>
        /// <param name="commentContent">Contents of the comment</param>
        /// <returns>True if the executions were successful, false otherwise</returns>
        public async Task<bool> SaveCommentAsync(int postId, string commentContent, int userId)
        {
            int rowsAffected = DEFAULT_ROWS_AFFECTED_VALUE;

            rowsAffected += await repository.AddCommentToPost(postId, commentContent.Replace("'", "''"), userId, DateTime.Now);

            return CheckRowsAffected(rowsAffected);
        }

        /// <summary>
        /// Update the active user's comment in the post
        /// </summary>
        /// <param name="commentId">Active user's comment</param>
        /// <param name="newCommentContent">New comment's contents</param>
        /// <returns>True if the executions were successful, false otherwise</returns>
        public async Task<bool> UpdateCommentAsync(int commentId, string newCommentContent, int userId)
        {
            int rowsAffected = DEFAULT_ROWS_AFFECTED_VALUE;

            rowsAffected += await repository.UpdateComment(commentId, newCommentContent.Replace("'", "''"));

            return CheckRowsAffected(rowsAffected);
        }

        /// <summary>
        /// Remove a comment from a post
        /// </summary>
        /// <param name="commentId">Target comment</param>
        /// <returns>True if the executions were successful, false otherwise</returns>
        public async Task<bool> DeleteCommentAsync(int commentId, int userId)
        {
            int rowsAffected = DEFAULT_ROWS_AFFECTED_VALUE;

            rowsAffected += await repository.DeleteCommentFromDatabase(commentId);

            return CheckRowsAffected(rowsAffected);
        }

        /// <summary>
        /// Load all comments of a post
        /// </summary>
        /// <param name="postId">Target post</param>
        /// <returns>List of loaded comments</returns>
        public async Task<List<Comment>> LoadNextCommentsAsync(int postId, int userId)
        {
            return await repository.LoadFollowingComments(postId);
        }

        /// <summary>
        /// Save a post
        /// </summary>
        /// <param name="postContent">Post's content</param>
        /// <returns>True if the executions were successful, false otherwise</returns>
        public async Task<bool> SavePostAsync(string postContent, int userId)
        {
            int rowsAffected = DEFAULT_ROWS_AFFECTED_VALUE;

            rowsAffected += await repository.AddPostToDatabase(userId, postContent.Replace("'", "''"), DateTime.Today);

            return CheckRowsAffected(rowsAffected);
        }

        /// <summary>
        /// Update a post's contents
        /// </summary>
        /// <param name="postId">Target post</param>
        /// <param name="newPostContent">New contents</param>
        /// <returns>True if the executions were successful, false otherwise</returns>
        public async Task<bool> UpdatePostAsync(int postId, string newPostContent, int userId)
        {
            int rowsAffected = DEFAULT_ROWS_AFFECTED_VALUE;

            rowsAffected += await repository.UpdatePost(postId, newPostContent.Replace("'", "''"));

            return CheckRowsAffected(rowsAffected);
        }

        /// <summary>
        /// Delete a post
        /// </summary>
        /// <param name="postId">Target post</param>
        /// <returns>True if the executions were successful, false otherwise</returns>
        public async Task<bool> DeletePostAsync(int postId, int userId)
        {
            int rowsAffected = DEFAULT_ROWS_AFFECTED_VALUE;

            rowsAffected += await repository.DeletePostFromDatabase(postId);

            return CheckRowsAffected(rowsAffected);
        }

        /// <summary>
        /// Load all posts that match the searched text
        /// If the searched text is an empty string, search for all posts
        /// </summary>
        /// <param name="pageNumber">Number of the page to calculate offset</param>
        /// <param name="searchedText">Searched text</param>
        /// <returns>List of found posts</returns>
        public async Task<List<Post>> LoadNextPostsAsync(int pageNumber, string searchedText, int userId)
        {
            return await repository.LoadFollowingPosts(pageNumber, userId, searchedText);
        }

        /// <summary>
        /// Set the correct contents of a button based on the edit mode
        /// </summary>
        /// <param name="editMode">Edit mode</param>
        /// <returns>The correct contents based on edit mode</returns>
        public string SetStringOnEditMode(bool editMode)
        {
            if (editMode)
            {
                return BUTTON_CONTENT_SAVE;
            }
            return BUTTON_CONTENT_POST;
        }

        /// <summary>
        /// Execute the correct execution method for a comment based on the edit mode
        /// </summary>
        /// <param name="editMode">Edit mode</param>
        /// <param name="commentId">Target comment</param>
        /// <param name="postId">Target post</param>
        /// <param name="commentContent">Contents of the comment</param>
        /// <returns>Correct execution results based on the edit mode</returns>
        public async Task<bool> SetCommentMethodOnEditModeAsync(bool editMode, int commentId, int postId, string commentContent, int userId)
        {
            if (editMode)
            {
                return await UpdateCommentAsync(commentId, await FormatAsPostAsync(commentContent), userId);
            }
            else
            {
                return await SaveCommentAsync(postId, await FormatAsPostAsync(commentContent), userId);
            }
        }

        /// <summary>
        /// Use the correct execution method on a post based on the edit mode and the post's contents
        /// </summary>
        /// <param name="editMode">Edit mode</param>
        /// <param name="postContent">Contents of the post</param>
        /// <param name="postId">Target post</param>
        public async Task ExecutePostMethodOnEditModeAsync(bool editMode, string postContent, int postId, int userId)
        {
            if (editMode && postContent != EMPTY_STRING)
            {
                await UpdatePostAsync(postId, await FormatAsPostAsync(postContent), userId);
                return;
            }
            else
            {
                if (postContent != EMPTY_STRING)
                {
                    await SavePostAsync(await FormatAsPostAsync(postContent), userId);
                    return;
                }
            }
        }

        /// <summary>
        /// Build the correct html code with the allowed tags from the given text
        /// </summary>
        /// <param name="htmlCode">Normal text</param>
        private void SanitizeHtml(ref string htmlCode)
        {
            string[] allowed_tags = { "h1", "/h1", "h2", "/h2", "h3", "/h3", "b", "/b", "i", "/i", "s", "/s", "sub", "/sub", "sup", "/sup", "spoiler", "/spoiler", "img.*" };
            htmlCode = Regex.Replace(htmlCode, $@"</?(?!({string.Join('|', allowed_tags)})\b)[^>]*>", string.Empty);
            htmlCode = Regex.Replace(htmlCode, @"<img\s+(?!src=(['""])[^'""]+\1\s*\/?>)[^>]*>", string.Empty);
        }

        /// <summary>
        /// Convert any special tags to readable html
        /// Replace any spoiler tags with span tags
        /// </summary>
        /// <param name="htmlCode">Html code as text</param>
        private void ConvertSpecialTagsToHtml(ref string htmlCode)
        {
            htmlCode = htmlCode.Replace("<spoiler>", "<span class=\"spoiler\" onclick=\"this.classList.toggle('revealed')\">");
            htmlCode = htmlCode.Replace("</spoiler>", "</span>");
            Stack<int> indicesOfUnclosedSpans = new();

            foreach (Match match in Regex.Matches(htmlCode, @"</?((span|/span)\b)[^>]*>"))
            {
                if (match.Value.StartsWith("</"))
                {
                    if (indicesOfUnclosedSpans.Count == 0)
                    {
                        htmlCode = htmlCode.Remove(match.Index, match.Value.Length);
                    }
                    else
                    {
                        indicesOfUnclosedSpans.Pop();
                    }
                }
                else
                {
                    indicesOfUnclosedSpans.Push(match.Index);
                }
            }

            while (indicesOfUnclosedSpans.Count > 0)
            {
                int matchIndex = indicesOfUnclosedSpans.Pop();
                htmlCode = htmlCode.Remove(matchIndex, "<span class=\"spoiler\" onclick=\"this.classList.toggle('revealed')\">".Length);
            }
        }

        /// <summary>
        /// Check if the repository's methods were successful by checking how many rows they affected in the database
        /// </summary>
        /// <param name="rowsAffected">Rows affected on execution</param>
        /// <returns>True if at least one row was affected, False if no rows were affected</returns>
        private bool CheckRowsAffected(int rowsAffected)
        {
            if (rowsAffected > DEFAULT_ROWS_AFFECTED_VALUE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
