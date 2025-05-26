using System;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI;
using BusinessLayer.Models;

namespace SteamProfile.Views
{
    public sealed partial class PostPreviewControl : UserControl
    {
        public event RoutedEventHandler? PostClicked;

        public Post PostData { get; private set; }

        public PostPreviewControl()
        {
            this.InitializeComponent();
            this.Loaded += PostPreviewControl_Loaded;
            this.PointerPressed += PostPreviewControl_PointerPressed;
        }

        public void SetPostData(Post post)
        {
            User? user = App.UserService.GetUserByIdentifier(post.AuthorId);
            if (user == null)
            {
                System.Diagnostics.Debug.WriteLine($"[WARNING] User not found for ID: {post.AuthorId}");
                Username.Text = "Unknown";
                UploadDate.Text = post.UploadDate.ToString("MMM d, yyyy");
                LikesCount.Text = post.NrLikes.ToString();
                DislikesCount.Text = post.NrDislikes.ToString();
                CommentsCount.Text = post.NrComments.ToString();
                ProfilePicture.ImageSource = null;
                return;
            }

            PostData = post;
            Username.Text = user.Username;
            UploadDate.Text = post.UploadDate.ToString("MMM d, yyyy");
            LikesCount.Text = post.NrLikes.ToString();
            DislikesCount.Text = post.NrDislikes.ToString();
            CommentsCount.Text = post.NrComments.ToString();

            var image = new BitmapImage();
            image.SetSource(new MemoryStream(user.ProfilePicture).AsRandomAccessStream());
            ProfilePicture.ImageSource = image;

            UpdateWebViewContent();
        }

        private void PostPreviewControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            PostClicked?.Invoke(this, new RoutedEventArgs());
        }

        private async void PostPreviewControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await ContentWebView.EnsureCoreWebView2Async();
                UpdateWebViewContent();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebView2 initialization error: {ex.Message}");
            }
        }

        private void UpdateWebViewContent()
        {
            if (ContentWebView.CoreWebView2 != null && PostData != null && PostData.Content != null)
            {
                ContentWebView.CoreWebView2.NavigateToString(PostData.Content);
            }
        }
    }
}