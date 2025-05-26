using System;

namespace BusinessLayer.Models
{
    public class CommentDisplay
    {
        // Currently logged-in user ID for comparison
        private static readonly uint CurrentUserId = App.GetService<IForumService>().GetCurrentUserId();
        
        // Original comment data
        public ForumComment Comment { get; private set; }
        
        // User information
        public User Author { get; private set; }
        
        // Properties for easy binding
        public uint Id => Comment.Id;
        public string Body => Comment.Body;
        public int Score => Comment.Score;
        public string TimeStamp => Comment.TimeStamp;
        public uint AuthorId => Comment.AuthorId;
        
        // User properties
        public string Username => Author.Username;
        public string ProfilePicturePath => Author.ProfilePicturePath;
        
        // Indicates if the comment belongs to the current user (for delete button visibility)
        public bool IsCurrentUser => AuthorId == CurrentUserId;
        
        // Create a CommentDisplay from a ForumComment
        public static CommentDisplay FromComment(ForumComment comment)
        {
            return new CommentDisplay
            {
                Comment = comment,
                Author = User.GetUserById(comment.AuthorId)
            };
        }
    }
} 