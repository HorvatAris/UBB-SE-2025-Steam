using System;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Models
{
    public class CommentDisplay
    {
        // Currently logged-in user ID for comparison (settable from UI)
        private static int currentUserId = 1;
        public static void SetCurrentUserId(int userId) => currentUserId = userId;
        // Original comment data
        public ForumComment Comment { get; private set; }
        // User information
        public User Author { get; private set; }
        // Properties for easy binding
        public int Id => Comment.Id;
        public string Body => Comment.Body;
        public int Score => Comment.Score;
        public string TimeStamp => Comment.TimeStamp.ToString();
        public int AuthorId => Comment.AuthorId;
        // User properties
        public string Username => Author.Username;
        public string ProfilePicturePath => Author.ProfilePicturePath;
        // Indicates if the comment belongs to the current user (for delete button visibility)
        public bool IsCurrentUser => AuthorId == currentUserId;
        // Create a CommentDisplay from a ForumComment
        public static CommentDisplay FromComment(ForumComment comment)
        {
            return new CommentDisplay
            {
                Comment = comment,
                Author = User.GetUserById((int)comment.AuthorId)
            };
        }
    }
}