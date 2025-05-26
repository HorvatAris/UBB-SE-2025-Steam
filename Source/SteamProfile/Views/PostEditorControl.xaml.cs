using System;
using System.IO;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.System;
using BusinessLayer.Services;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.Views
{
    public sealed partial class PostEditorControl : UserControl
    {
        private const string EMPTY_STRING = "";

        public event RoutedEventHandler? PostUploaded;
        private INewsService service;
        private Post? postBeingEdited = null;
        private bool isEditMode = false;

        public PostEditorControl()
        {
            this.InitializeComponent();

            service = service = App.GetService<INewsService>();

            this.Loaded += PostEditorControl_Loaded;
        }

        private void RawButton_Click(object sender, RoutedEventArgs e)
        {
            if (RawHtmlEditor.Visibility == Visibility.Visible)
            {
                return;
            }
            HtmlPreview.Visibility = Visibility.Collapsed;
            RawHtmlEditor.Visibility = Visibility.Visible;
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (HtmlPreview.Visibility == Visibility.Visible)
            {
                return;
            }
            HtmlPreview.Visibility = Visibility.Visible;
            RawHtmlEditor.Visibility = Visibility.Collapsed;
            HtmlPreview.CoreWebView2.NavigateToString(service.FormatAsPost(RawHtmlEditor.Text));
        }

        public void SetPostToEdit(Post post)
        {
            isEditMode = true;
            postBeingEdited = post;

            string htmlContent = post.Content;

            int startIndex = htmlContent.IndexOf("<body>") + "<body>".Length;
            int endIndex = htmlContent.IndexOf("</body>");
            int bodyLength = endIndex - startIndex;
            string bodyContent = htmlContent.Substring(startIndex, bodyLength);
            RawHtmlEditor.Text = bodyContent;
        }

        public void ResetEditor()
        {
            isEditMode = false;
            postBeingEdited = null;
            RawHtmlEditor.Text = EMPTY_STRING;
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditMode)
            {
                if (RawHtmlEditor.Text == string.Empty)
                {
                    return;
                }
                string html = service.FormatAsPost(RawHtmlEditor.Text);
                service.UpdatePost(postBeingEdited.Id, html);
            }
            else
            {
                if (RawHtmlEditor.Text != string.Empty)
                {
                    string html = service.FormatAsPost(RawHtmlEditor.Text);
                    service.SavePost(html);
                }
            }
            ResetEditor();
            RawHtmlEditor.Text = EMPTY_STRING;
            PostUploaded?.Invoke(this, new RoutedEventArgs());
        }

        private async void PostEditorControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await HtmlPreview.EnsureCoreWebView2Async();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebView2 initialization error: {ex.Message}");
            }
            Username.Text = App.CurrentUser.Username;
            CurrentDate.Text = DateTime.Now.ToString("MMM d, yyyy");

            var image = new BitmapImage();
            image.SetSource(new MemoryStream(App.CurrentUser.ProfilePicture).AsRandomAccessStream());
            ProfilePicture.ImageSource = image;
        }
    }
}