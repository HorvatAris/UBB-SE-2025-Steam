using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Services;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using SteamProfile.Views;

namespace SteamProfile.ViewModels
{
    public class ViewModelActionCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        public ViewModelActionCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        // Event required by ICommand interface
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);

        public void Execute(object parameter) => execute(parameter);

        // Method to manually trigger CanExecuteChanged
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class ForumViewModel : INotifyPropertyChanged
    {
        private readonly IForumService forumService;

        private ObservableCollection<ForumPost> posts;
        public ObservableCollection<ForumPost> Posts
        {
            get => posts;
            set => SetProperty(ref posts, value);
        }

        private ForumPost selectedPost;
        public ForumPost SelectedPost
        {
            get => selectedPost;
            set
            {
                if (SetProperty(ref selectedPost, value))
                {
                    // Load comments when a post is selected and notify command state change
                    LoadCommentsForPost(); // Use synchronous method
                    // Manually raise CanExecuteChanged for the command
                    ((ViewModelActionCommand)AddCommentCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private ObservableCollection<ForumComment> comments;
        public ObservableCollection<ForumComment> Comments
        {
            get => comments;
            set => SetProperty(ref comments, value);
        }

        // Commands
        public ICommand CreatePostCommand { get; }
        public ICommand AddCommentCommand { get; }
        public ICommand LoadPostsCommand { get; }
        public event EventHandler CreatePostRequested;
        public ICommand RequestCreatePostCommand { get; }

        // Constructor
        public ForumViewModel(IForumService forumService)
        {
            this.forumService = forumService ?? throw new ArgumentNullException(nameof(forumService));

            // Set the current user ID for comment display logic
            CommentDisplay.SetCurrentUserId(forumService.GetCurrentUserId());

            Posts = new ObservableCollection<ForumPost>();
            Comments = new ObservableCollection<ForumComment>();

            // Initialize Commands
            LoadPostsCommand = new ViewModelActionCommand(parameter => LoadPosts());
            CreatePostCommand = new ViewModelActionCommand(async parameter => await CreateNewPostAsync());
            AddCommentCommand = new ViewModelActionCommand(async parameter => await AddNewCommentAsync(), parameter => SelectedPost != null);
            RequestCreatePostCommand = new ViewModelActionCommand(_ => OnCreatePostRequested());

            // Load initial posts
            LoadPosts(); // Call synchronous method directly
        }

        // Method to load posts (synchronous to match service)
        public void LoadPosts()
        {
            try
            {
                // Use correct service method GetPagedPosts (load first page for example)
                uint pageNumber = 0;
                uint pageSize = 20; // Example page size
                var posts = forumService.GetPagedPosts(pageNumber, pageSize);
                Posts.Clear();
                foreach (var post in posts)
                {
                    Posts.Add(post);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error loading posts: {exception.Message}");
            }
        }

        // Method to load comments for the selected post (synchronous)
        private void LoadCommentsForPost()
        {
            if (SelectedPost == null)
            {
                Comments.Clear();
                return;
            }

            try
            {
                var comments = forumService.GetComments((int)SelectedPost.Id);
                Comments.Clear();
                foreach (var comment in comments)
                {
                    Comments.Add(comment);
                }
            }
            catch (Exception exception)
            {
                 Console.WriteLine($"Error loading comments for post {SelectedPost.Id}: {exception.Message}");
            }
        }

        private void OnCreatePostRequested()
        {
            CreatePostRequested?.Invoke(this, EventArgs.Empty);
        }

        private async Task CreateNewPostAsync()
        {
            // This method is now unused for dialog display. You may keep post-creation logic here if needed.
        }

        private async Task AddNewCommentAsync()
        {
            if (SelectedPost == null)
            {
                return;
            }
            try
            {
                // NOTE: If you need to set XamlRoot, do it from the View (code-behind) before calling this command.
                var addCommentDialog = new AddCommentDialog((int)SelectedPost.Id);
                var result = await addCommentDialog.ShowAsync();
                if (result == ContentDialogResult.Primary && addCommentDialog.CommentCreated)
                {
                    LoadCommentsForPost(); // Reload comments for the current post
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding new comment: {ex.Message}");
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<Property>(ref Property storage, Property value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<Property>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
