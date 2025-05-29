using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SteamHub.ViewModels
{
    public class NewsPageViewModel : BaseViewModel
    {
        private readonly INewsService newsService;
        private string searchText;
        private ObservableCollection<Post> posts;
        private Dictionary<int, ObservableCollection<Comment>> postComments;

        public NewsPageViewModel(INewsService newsService, IUserService userService, User currentUser)
            : base(userService, currentUser)
        {
            this.newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
            Posts = new ObservableCollection<Post>();
            postComments = new Dictionary<int, ObservableCollection<Comment>>();
        }

        public string SearchText
        {
            get => searchText;
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    _ = LoadPosts(); // Reload posts when search text changes
                }
            }
        }

        public ObservableCollection<Post> Posts
        {
            get => posts;
            set => SetProperty(ref posts, value);
        }

        public async Task LoadPosts()
        {
            try
            {
                var user = await this.userService.GetCurrentUserAsync();
                var loadedPosts = await newsService.LoadNextPostsAsync(0, "", user.UserId);
                Posts.Clear();
                postComments.Clear();
                foreach (var post in loadedPosts)
                {
                    Posts.Add(post);
                    // Load comments for each post
                    await LoadCommentsForPost(post.Id, user.UserId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading posts: {ex.Message}");
            }
        }

        public async Task LoadCommentsForPost(int postId, int userId)
        {
            try
            {
                var comments = await newsService.LoadNextCommentsAsync(postId, userId);
                var commentCollection = new ObservableCollection<Comment>(comments);
                postComments[postId] = commentCollection;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading comments for post {postId}: {ex.Message}");
            }
        }



        //public ObservableCollection<Comment> GetCommentsForPost(int postId)
        //{
        //    if (postComments.TryGetValue(postId, out var comments))
        //    {
        //        return comments;
        //    }
        //    return new ObservableCollection<Comment>();
        //}
        public ObservableCollection<Comment> GetCommentsForPost(int postId)
        {
            return postComments.ContainsKey(postId) ? postComments[postId] : new ObservableCollection<Comment>();
        }


        public async Task CreatePost(string title, string content)
        {
            try
            {
                var user = await this.userService.GetCurrentUserAsync();
                var formattedContent = await newsService.FormatAsPostAsync(content);
                var result = await newsService.SavePostAsync(formattedContent, user.UserId);
                if (result)
                {
                    await LoadPosts(); // Reload posts after creating a new one
                }
            }
            catch (Exception ex)
            {
                // TODO: Handle error appropriately
                System.Diagnostics.Debug.WriteLine($"Error creating post: {ex.Message}");
            }
        }

        public async Task LikePost(int postId)
        {
            try
            {
                var user = await this.userService.GetCurrentUserAsync();

                var result = await newsService.LikePostAsync(postId, user.UserId);
                if (result)
                {
                    await LoadPosts(); // Reload posts to update likes
                }
            }
            catch (Exception ex)
            {
                // TODO: Handle error appropriately
                System.Diagnostics.Debug.WriteLine($"Error liking post: {ex.Message}");
            }
        }

        public async Task DislikePost(int postId)
        {
            try
            {
                var user = await this.userService.GetCurrentUserAsync();
                var result = await newsService.DislikePostAsync(postId, user.UserId);
                if (result)
                {
                    await LoadPosts(); // Reload posts to update dislikes
                }
            }
            catch (Exception ex)
            {
                // TODO: Handle error appropriately
                System.Diagnostics.Debug.WriteLine($"Error disliking post: {ex.Message}");
            }
        }

        public async Task AddComment(int postId, string content)
        {
            try
            {
                var user = await this.userService.GetCurrentUserAsync();
                var result = await newsService.SaveCommentAsync(postId, content, user.UserId);
                if (result)
                {
                    await LoadCommentsForPost(postId, user.UserId); // Reload comments for this post
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding comment: {ex.Message}");
            }
        }

        //public async Task LikeComment(int commentId)
        //{
        //    try
        //    {
        //        var user = await this.userService.GetCurrentUserAsync();
        //        var result = await newsService.LikeCommentAsync(commentId, user.UserId);
        //        if (result)
        //        {
        //            // Find the post ID for this comment and reload its comments
        //            foreach (var post in Posts)
        //            {
        //                var comments = GetCommentsForPost(post.Id);
        //                if (comments.Any(c => c.CommentId == commentId))
        //                {
        //                    await LoadCommentsForPost(post.Id, user.UserId);
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error liking comment: {ex.Message}");
        //    }
        //}

        //    public async Task DislikeComment(int commentId)
        //    {
        //        try
        //        {
        //            var user = await this.userService.GetCurrentUserAsync();
        //            var result = await newsService.Di
        //            if (result)
        //            {
        //                // Find the post ID for this comment and reload its comments
        //                foreach (var post in Posts)
        //                {
        //                    var comments = GetCommentsForPost(post.Id);
        //                    if (comments.Any(c => c.CommentId == commentId))
        //                    {
        //                        await LoadCommentsForPost(post.Id, user.UserId);
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            System.Diagnostics.Debug.WriteLine($"Error disliking comment: {ex.Message}");
        //        }
        //    }
        //}
    }
}
