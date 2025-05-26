using System;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using BusinessLayer.Services;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.ViewModels
{
    public partial class CommentViewModel : ObservableObject
    {
        private readonly NewsService service;
        private readonly Users users = Users.Instance;

        public event EventHandler? CommentUpdated;
        public event EventHandler? CommentDeleted;

        [ObservableProperty]
        private Comment commentData;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string commentDate;

        [ObservableProperty]
        private BitmapImage profilePicture;

        [ObservableProperty]
        private string contentHtml;

        [ObservableProperty]
        private bool isEditVisible;

        [ObservableProperty]
        private bool isDeleteVisible;

        [ObservableProperty]
        private bool isInEditMode;

        public IRelayCommand EditCommand { get; }
        public IRelayCommand DeleteCommand { get; }

        public CommentViewModel()
        {
            service = (NewsService)App.GetService<INewsService>();
            EditCommand = new RelayCommand(ExecuteEdit);
            DeleteCommand = new RelayCommand(ExecuteDelete);
        }

        public void LoadComment(Comment comment)
        {
            CommentData = comment;
            var user = users.GetUserById(comment.AuthorId);

            Username = user?.Username ?? "Unknown";
            CommentDate = comment.CommentDate.ToString("MMM d, yyyy");
            ContentHtml = comment.Content;

            var image = new BitmapImage();
            image.SetSource(new MemoryStream(user.ProfilePicture).AsRandomAccessStream());
            ProfilePicture = image;

            bool isOwnComment = user.UserId == service.ActiveUser.UserId;
            IsEditVisible = isOwnComment;
            IsDeleteVisible = isOwnComment;
        }

        private void ExecuteEdit()
        {
            IsInEditMode = true;
            IsEditVisible = false;
            IsDeleteVisible = false;
        }

        private void ExecuteDelete()
        {
            bool success = service.DeleteComment(CommentData.CommentId);
            if (success)
            {
                CommentDeleted?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SubmitEdit(string rawText)
        {
            CommentData.Content = service.FormatAsPost(rawText);
            ContentHtml = CommentData.Content;

            IsInEditMode = false;
            IsEditVisible = true;
            IsDeleteVisible = true;

            CommentUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
