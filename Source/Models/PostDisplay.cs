using System;
using Forum_Lib;

namespace BusinessLayer.Models
{
    public class PostDisplay
    {
        // Currently logged-in user ID for comparison
        private static readonly uint CurrentUserId = 2; // Hard-coded to JaneSmith for demo
        
        // Original post data
        public ForumPost Post { get; private set; }
        
        // User information
        public User Author { get; private set; }
        
        // Properties for easy binding
        public uint Id => Post.Id;
        public string Title => Post.Title;
        public int Score => Post.Score;
        public string TimeStamp => Post.TimeStamp;
        public uint AuthorId => Post.AuthorId;
        public uint? GameId => Post.GameId;
        
        // User properties
        public string Username => Author.Username;
        public string ProfilePicturePath => Author.ProfilePicturePath;
        
        // Indicates if the post belongs to the current user (for delete button visibility)
        public bool IsCurrentUser => AuthorId == CurrentUserId;
        
        // Create a PostDisplay from a ForumPost
        public static PostDisplay FromPost(ForumPost post)
        {
            return new PostDisplay
            {
                Post = post,
                Author = User.GetUserById(post.AuthorId)
            };
        }
    }
} 