using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BusinessLayer.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Services;

namespace SteamProfile.Views
{
    public sealed partial class PostsControl : UserControl
    {
        private ObservableCollection<PostDisplay> posts;
        private uint currentPage = 0;
        private uint pageSize = 10;
        private bool isLoadingMore = false;
        private bool hasMorePosts = true;
        private bool positiveScoreOnly = false;
        private int? gameId = null;
        private string filter = null;
        private TimeSpanFilter timeSpanFilter = TimeSpanFilter.AllTime;
        private bool isTopPostsMode = false;
        // Event for when a post is selected
        public event EventHandler<ForumPost> PostSelected;
        public PostsControl()
        {
            this.InitializeComponent();
            posts = new ObservableCollection<PostDisplay>();
            PostsListView.ItemsSource = posts;
            // Add handler for when container is generated to set delete button visibility
            PostsListView.ContainerContentChanging += PostsListView_ContainerContentChanging;
        }

        // Load top posts using time filter
        public void LoadTopPosts(TimeSpanFilter filter)
        {
            try
            {
                // Reset state
                currentPage = 0;
                isTopPostsMode = true;
                timeSpanFilter = filter;
                hasMorePosts = true;
                ShowLoading(true);
                // Clear existing posts
                posts.Clear();
                // Get posts from repository
                List<ForumPost> forumPosts = App.GetService<IForumService>().GetTopPosts(filter);
                // Update the UI on the UI thread
                this.DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (var post in forumPosts)
                    {
                        posts.Add(PostDisplay.FromPost(post));
                    }
                    // If we got fewer posts than the page size, there are no more posts to load
                    if (forumPosts.Count < pageSize)
                    {
                        hasMorePosts = false;
                    }
                    // Show the no posts message if no posts were found
                    UpdateVisibility();
                    ShowLoading(false);
                });
            }
            catch (Exception ex)
            {
                // Handle errors
                ShowLoading(false);
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Failed to load posts. Please try again later.",
                    CloseButtonText = "OK"
                };
                dialog.ShowAsync();
            }
        }
        // Load paged posts with optional filters
        public void LoadPagedPosts(uint pageNumber, uint pageSize, bool positiveScoreOnly = false, int? gameId = null, string filter = null)
        {
            try
            {
                // Save filter parameters for loading more later
                currentPage = pageNumber;
                pageSize = pageSize;
                positiveScoreOnly = positiveScoreOnly;
                gameId = gameId;
                filter = filter;
                isTopPostsMode = false;
                hasMorePosts = true;
                ShowLoading(true);
                // Clear existing posts if loading first page
                if (pageNumber == 0)
                {
                    posts.Clear();
                }
                // Get posts from repository
                List<ForumPost> forumPosts = App.GetService<IForumService>().GetPagedPosts(pageNumber, pageSize, positiveScoreOnly, gameId, filter);
                // Update the UI on the UI thread
                this.DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (var post in forumPosts)
                    {
                        posts.Add(PostDisplay.FromPost(post));
                    }
                    // If we got fewer posts than the page size, there are no more posts to load
                    if (forumPosts.Count < pageSize)
                    {
                        hasMorePosts = false;
                    }
                    // Show the no posts message if no posts were found
                    UpdateVisibility();
                    ShowLoading(false);
                });
            }
            catch (Exception ex)
            {
                // Handle errors
                ShowLoading(false);
                // Could show error message here
            }
        }
        // Load more posts when user scrolls to the bottom
        private void LoadMorePosts()
        {
            if (isLoadingMore || !hasMorePosts)
            {
                return; // Already loading or no more posts to load
            }
            try
            {
                isLoadingMore = true;
                ShowLoadMoreIndicator(true);
                // Increment page number
                currentPage++;
                // Get more posts based on the current mode
                List<ForumPost> morePosts;
                if (isTopPostsMode)
                {
                    // In top posts mode, we can't really load "more" top posts
                    // This is just for demonstration, in a real app you might
                    // implement a different strategy for "more" top posts
                    morePosts = new List<ForumPost>();
                    hasMorePosts = false;
                }
                else
                {
                    // Get next page of posts
                    morePosts = App.GetService<IForumService>().GetPagedPosts(currentPage, pageSize, positiveScoreOnly, gameId, filter);
                    // If we got fewer posts than the page size, there are no more posts to load
                    if (morePosts.Count < pageSize)
                    {
                        hasMorePosts = false;
                    }
                }
                // Update the UI on the UI thread
                this.DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (var post in morePosts)
                    {
                        posts.Add(PostDisplay.FromPost(post));
                    }
                    isLoadingMore = false;
                    ShowLoadMoreIndicator(false);
                });
            }
            catch (Exception ex)
            {
                isLoadingMore = false;
                ShowLoadMoreIndicator(false);
                // Could show error message here
            }
        }
        // ScrollViewer event handler to detect when the user has scrolled to the bottom
        private void PostsScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            // Check if we've scrolled near the bottom
            if (PostsScrollViewer.VerticalOffset >= PostsScrollViewer.ScrollableHeight - 100)
            {
                LoadMorePosts();
            }
        }
        // Handle post selection
        private void PostsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PostsListView.SelectedItem is PostDisplay postDisplay)
            {
                PostSelected?.Invoke(this, postDisplay.Post);
            }
        }
        // Handle upvote button click
        private void UpvoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int postId)
            {
                // Call the service with a positive vote value (1)
                App.GetService<IForumService>().VoteOnPost(postId, 1);
                // Refresh the specific post in the list to show updated score
                RefreshPost(postId);
            }
        }
        // Handle downvote button click
        private void DownvoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int postId)
            {
                // Call the service with a negative vote value (-1)
                App.GetService<IForumService>().VoteOnPost(postId, -1);
                // Refresh the specific post in the list to show updated score
                RefreshPost(postId);
            }
        }
        // Handle delete button click
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int postId)
            {
                try
                {
                    // Skip the confirmation dialog for now due to XamlRoot issues
                    // Just delete the post directly
                    App.GetService<IForumService>().DeletePost(postId);
                    // Remove the post from the UI
                    RemovePostFromUI((int)postId);
                }
                catch (Exception ex)
                {
                    // Just fail silently for now
                }
            }
        }
        // Helper method to refresh a specific post
        private void RefreshPost(int postId)
        {
            // Find the post in the collection
            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i].Id == postId)
                {
                    // In a real application, we would fetch the updated post from the service
                    // Instead of updating the score ourselves, let's get the current score from the service
                    try
                    {
                        // Get the actual updated score from the database
                        // First get a reference to the original ForumPost object
                        ForumPost originalPost = this.posts[i].Post;
                        // Create a new instance with the latest score from the service
                        // This could be a dedicated GetPostById method in a real application
                        List<ForumPost> posts;
                        if (isTopPostsMode)
                        {
                            posts = App.GetService<IForumService>().GetTopPosts(timeSpanFilter);
                        }
                        else
                        {
                            posts = App.GetService<IForumService>().GetPagedPosts(currentPage, pageSize, positiveScoreOnly, gameId, filter);
                        }
                        // Find the post with matching ID to get updated score
                        ForumPost updatedPost = posts.FirstOrDefault(p => p.Id == postId);
                        if (updatedPost != null)
                        {
                            // Create a new PostDisplay with the updated post
                            PostDisplay updatedPostDisplay = PostDisplay.FromPost(updatedPost);
                            // Replace the old post with the updated one
                            posts.RemoveAt(i);
                            this.posts.Insert(i, updatedPostDisplay);
                        }
                    }
                    catch (Exception)
                    {
                        // If there's an error getting the updated post, just leave as is
                    }
                    break;
                }
            }
        }
        // Helper method to update visibility of controls
        private void UpdateVisibility()
        {
            if (posts.Count == 0)
            {
                PostsListView.Visibility = Visibility.Collapsed;
                NoPostsMessage.Visibility = Visibility.Visible;
            }
            else
            {
                PostsListView.Visibility = Visibility.Visible;
                NoPostsMessage.Visibility = Visibility.Collapsed;
            }
        }
        // Helper method to show/hide loading indicator
        private void ShowLoading(bool isLoading)
        {
            LoadingIndicator.IsActive = isLoading;
            if (isLoading)
            {
                PostsListView.Visibility = Visibility.Collapsed;
                NoPostsMessage.Visibility = Visibility.Collapsed;
            }
        }
        // Helper method to show/hide load more indicator
        private void ShowLoadMoreIndicator(bool isLoading)
        {
            if (isLoading)
            {
                LoadMoreIndicator.IsActive = true;
                LoadMoreIndicator.Visibility = Visibility.Visible;
            }
            else
            {
                LoadMoreIndicator.IsActive = false;
                LoadMoreIndicator.Visibility = Visibility.Collapsed;
            }
        }
        // Helper method to remove a post from the UI
        private void RemovePostFromUI(int postId)
        {
            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i].Id == postId)
                {
                    posts.RemoveAt(i);
                    break;
                }
            }
            // Update visibility in case we removed the last post
            UpdateVisibility();
        }
        // Show brief delete confirmation
        private void ShowDeleteConfirmation()
        {
            // In a real application, you might want to show a temporary message or toast notification
            // For this example, we'll just keep it simple
        }

        // Handle container content changing to set delete button visibility
        private void PostsListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Item is PostDisplay postDisplay)
            {
                // Find the delete button in the container
                var container = args.ItemContainer;
                var deleteButton = FindChildByName(container, "DeleteButton") as Button;
                if (deleteButton != null)
                {
                    // Set visibility based on whether this is the current user's post
                    deleteButton.Visibility = postDisplay.IsCurrentUser ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        // Helper method to find child control by name
        private DependencyObject FindChildByName(DependencyObject parent, string name)
        {
            // Check if current element is what we're looking for
            if (parent is FrameworkElement element && element.Name == name)
            {
                return parent;
            }

            // Get number of children
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            // Search children
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                DependencyObject result = FindChildByName(child, name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}