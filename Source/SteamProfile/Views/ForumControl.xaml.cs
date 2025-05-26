using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using SteamProfile.ViewModels;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Services;
using BusinessLayer.Models;

namespace SteamProfile.Views
{
    public sealed partial class ForumControl : UserControl
    {
        private readonly uint pageSize = 10;
        private string currentSearchFilter = null;
        public ForumViewModel ViewModel { get; private set; }

        public ForumControl()
        {
            this.InitializeComponent();

            try
            {
                // Get the service from App's service container
                IForumService forumService = App.GetService<IForumService>();
                ViewModel = new ForumViewModel(forumService);
                this.DataContext = ViewModel;

                // Subscribe to CreatePostRequested event
                ViewModel.CreatePostRequested += ViewModel_CreatePostRequested;

                // Set up post selection event handler
                if (PostsControl != null)
                {
                    PostsControl.PostSelected += PostsControl_PostSelected;
                }

                // Initialize the UI after all elements are loaded
                LoadPosts();
            }
            catch (Exception ex)
            {
                // Handle initialization error
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Failed to initialize forum service. Please try again later.",
                    CloseButtonText = "OK"
                };
                dialog.ShowAsync();
            }
        }

        // Keep LoadPosts for now as it interacts directly with PostsControl sorting/filtering
        private void LoadPosts()
        {
            try
            {
                // Ensure controls exist before using them
                if (SortComboBox == null || PositiveScoreToggle == null || PostsControl == null)
                {
                    return;
                }
                // Determine selected sort option
                int selectedIndex = SortComboBox.SelectedIndex;
                bool positiveScoreOnly = PositiveScoreToggle.IsChecked ?? false;
                if (selectedIndex == 0)
                {
                    // Load first page of posts - the PostsControl will handle paging
                    PostsControl.LoadPagedPosts(0, pageSize, positiveScoreOnly, null, currentSearchFilter);
                }
                else
                {
                    // Convert sort index to TimeSpanFilter
                    TimeSpanFilter filter;
                    switch (selectedIndex)
                    {
                        case 1: // Today
                            filter = TimeSpanFilter.Day;
                            break;
                        case 2: // Week
                            filter = TimeSpanFilter.Week;
                            break;
                        case 3: // Month
                            filter = TimeSpanFilter.Month;
                            break;
                        case 4: // Year
                            filter = TimeSpanFilter.Year;
                            break;
                        case 5: // All Time
                            filter = TimeSpanFilter.AllTime;
                            break;
                        default:
                            filter = TimeSpanFilter.AllTime;
                            break;
                    }
                    // Load top posts with selected filter
                    PostsControl.LoadTopPosts(filter);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadPosts();
        }
        private void PositiveScoreToggle_CheckedChanged(object sender, RoutedEventArgs e)
        {
            LoadPosts();
        }
        private void SearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                PerformSearch();
            }
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }
        private void PerformSearch()
        {
            if (SearchBox == null || PositiveScoreToggle == null || PostsControl == null)
            {
                return;
            }
            string searchTerm = SearchBox.Text?.Trim();
            currentSearchFilter = string.IsNullOrEmpty(searchTerm) ? null : searchTerm;
            // Reset to first page and apply search filter (PostsControl handles the paging)
            bool positiveScoreOnly = PositiveScoreToggle.IsChecked ?? false;
            PostsControl.LoadPagedPosts(0, pageSize, positiveScoreOnly, null, currentSearchFilter);
        }
        private async void ViewModel_CreatePostRequested(object sender, EventArgs e)
        {
            var dialog = new CreatePostDialog();
            dialog.XamlRoot = this.Content.XamlRoot;
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary && dialog.PostCreated)
            {
                ViewModel.LoadPosts();
            }
        }
        private async void PostsControl_PostSelected(object sender, ForumPost post)
        {
            try
            {
                // Create and show the post detail dialog
                PostDetailDialog dialog = new PostDetailDialog(post);
                // Set XamlRoot for proper display
                dialog.XamlRoot = this.Content.XamlRoot;

                // Handle post deleted event
                dialog.PostDeleted += (s, e) =>
                {
                    // Reload posts when a post is deleted
                    LoadPosts();
                };

                // Show the dialog
                ContentDialogResult result = await dialog.ShowAsync();

                // Check if changes were made to reload posts if needed
                if (dialog.ChangesWereMade)
                {
                    LoadPosts();
                }
            }
            catch (Exception ex)
            {
                // Handle any errors
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"An error occurred: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await errorDialog.ShowAsync();
            }
        }
    }
}