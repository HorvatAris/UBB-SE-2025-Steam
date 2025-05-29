using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ViewModels;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SteamHub.ApiContract.Models;
using SteamHub.Pages.Converters;
using System.Resources;

namespace SteamHub.Pages
{
    public sealed partial class NewsPage : Page
    {
        public NewsPageViewModel ViewModel { get; }
        


        public NewsPage(INewsService newsService, IUserService userService, User currentUser)
        {
            this.InitializeComponent();
            ViewModel = new NewsPageViewModel(newsService, userService, currentUser);
            this.DataContext = ViewModel;
            //(this.Resources["PostIdToCommentsConverter"] as PostIdToCommentsConverter).ViewModel = ViewModel;
            Loaded += NewsPage_Loaded;
        }


        private async void NewsPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadPosts();
        }

        private async void CreatePostButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Create New Post",
                PrimaryButtonText = "Post",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var stackPanel = new StackPanel { Spacing = 10 };
            var titleBox = new TextBox { PlaceholderText = "Title" };
            var contentBox = new TextBox 
            { 
                PlaceholderText = "Content",
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                Height = 200
            };

            stackPanel.Children.Add(titleBox);
            stackPanel.Children.Add(contentBox);
            dialog.Content = stackPanel;

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await ViewModel.CreatePost(titleBox.Text, contentBox.Text);
            }
        }

        private async void LikePost_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int postId)
            {
                await ViewModel.LikePost(postId);
            }
        }

        private async void DislikePost_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int postId)
            {
                await ViewModel.DislikePost(postId);
            }
        }

        private async void AddComment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int postId)
            {
                var parent = button.Parent as Grid;
                var textBox = parent?.Children[0] as TextBox;
                if (textBox != null)
                {
                    await ViewModel.AddComment(postId, textBox.Text);
                    textBox.Text = string.Empty;
                }
            }
        }

        //private async void LikeComment_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is Button button && button.Tag is int commentId)
        //    {
        //        await ViewModel.LikeComment(commentId);
        //    }
        //}

        //private async void DislikeComment_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is Button button && button.Tag is int commentId)
        //    {
        //        await ViewModel.DislikeComment(commentId);
        //    }
        //}
    }
} 
 