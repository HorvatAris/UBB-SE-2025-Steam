using System;

namespace SteamHub.ApiContract.Models
{
    public class PostDisplay
    {
        // Currently logged-in user ID for comparison
        private static readonly int CurrentUserId = 2; // Hard-coded to JaneSmith for demo
        // Original post data
        public ForumPost Post { get; private set; }
        // User information
        public SteamHub.ApiContract.Models.User.User Author { get; private set; }
        // Properties for easy binding
        public int Id => Post.Id;
        public string Title => Post.Title;
        public int Score => Post.Score;
        public string TimeStamp => Post.TimeStamp.ToString();
        public int AuthorId => (int)Post.AuthorId;
        public int? GameId => Post.GameId;
        // User properties
        public string Username => Author.Username;
        public string ProfilePicturePath => Author.ProfilePicture;
        // Indicates if the post belongs to the current user (for delete button visibility)
        public bool IsCurrentUser => AuthorId == CurrentUserId;
        // Create a PostDisplay from a ForumPost
        public static PostDisplay FromPost(ForumPost post)
        {
            return new PostDisplay
            {
                Post = post,
                Author = SteamHub.ApiContract.Models.User.User.GetUserById(post.AuthorId)
            };
        }
    }
}