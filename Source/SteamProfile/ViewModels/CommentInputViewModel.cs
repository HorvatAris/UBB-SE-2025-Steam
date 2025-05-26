using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.ViewModels
{
    public partial class CommentInputViewModel : ObservableObject
    {
        private readonly INewsService service;

        [ObservableProperty]
        private string rawText = string.Empty;

        [ObservableProperty]
        private string postButtonText = "Post Comment";

        [ObservableProperty]
        private bool isInEditMode = false;

        [ObservableProperty]
        private Visibility rawEditorVisibility = Visibility.Visible;

        [ObservableProperty]
        private Visibility previewVisibility = Visibility.Collapsed;

        public int PostId { get; set; }
        public int CommentId { get; set; }

        public event EventHandler? CommentPosted;

        public IRelayCommand ToggleRawCommand { get; }
        public IRelayCommand TogglePreviewCommand { get; }
        public IRelayCommand PostCommand { get; }

        public CommentInputViewModel()
        {
            service = App.GetService<INewsService>();
            ToggleRawCommand = new RelayCommand(ShowRaw);
            TogglePreviewCommand = new RelayCommand(ShowPreview);
            PostCommand = new RelayCommand(PostComment);
        }

        public void SetEditMode(bool isEdit)
        {
            IsInEditMode = isEdit;
            PostButtonText = service.SetStringOnEditMode(IsInEditMode);
        }

        public void Reset()
        {
            RawText = string.Empty;
            IsInEditMode = false;
            PostButtonText = "Post Comment";
            ShowRaw();
        }

        private void ShowRaw()
        {
            RawEditorVisibility = Visibility.Visible;
            PreviewVisibility = Visibility.Collapsed;
        }

        private void ShowPreview()
        {
            RawEditorVisibility = Visibility.Collapsed;
            PreviewVisibility = Visibility.Visible;
        }

        private void PostComment()
        {
            if (string.IsNullOrWhiteSpace(RawText))
            {
                return;
            }

            bool success = service.SetCommentMethodOnEditMode(IsInEditMode, CommentId, PostId, RawText);

            if (success)
            {
                CommentPosted?.Invoke(this, EventArgs.Empty);
                Reset();
            }
        }

        public string GetFormattedPreview() => service.FormatAsPost(RawText);
    }
}
