using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.ViewModels;

namespace SteamProfile.Views
{
    public sealed partial class CommentInputControl : UserControl
    {
        public CommentInputViewModel ViewModel { get; } = new();

        public event RoutedEventHandler? CommentPosted;

        public int PostId
        {
            get => ViewModel.PostId;
            set => ViewModel.PostId = value;
        }

        public int CommentId
        {
            get => ViewModel.CommentId;
            set => ViewModel.CommentId = value;
        }

        public CommentInputControl()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
            ViewModel.CommentPosted += (_, __) => CommentPosted?.Invoke(this, new RoutedEventArgs());

            this.Loaded += async (s, e) =>
            {
                try
                {
                    await HtmlPreview.EnsureCoreWebView2Async().AsTask();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"WebView2 initialization error: {ex.Message}");
                }
            };
        }

        public void SetEditMode(bool isEdit) => ViewModel.SetEditMode(isEdit);
        public void ResetControl() => ViewModel.Reset();

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.TogglePreviewCommand.Execute(null);
            if (HtmlPreview.CoreWebView2 != null)
            {
                HtmlPreview.CoreWebView2.NavigateToString(ViewModel.GetFormattedPreview());
            }
        }
    }
}
