using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using BusinessLayer.Models;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;

namespace SteamProfile.ViewModels
{
    public class PostWithPosition
    {
        public Post Post { get; }
        public int Row { get; }
        public int Column { get; }

        public PostWithPosition(Post post, int index, int columns)
        {
            Post = post;
            Row = index / columns;
            Column = index % columns;
        }
    }

    public partial class NewsViewModel : ObservableObject
    {
        private readonly INewsService service;
        private const int DIVISOR = 3;

        public ObservableCollection<PostWithPosition> PositionedPosts { get; } = new();
        public int CurrentPage { get; private set; } = 0;

        [ObservableProperty] private string searchText = string.Empty;
        [ObservableProperty] private Visibility postOverlayVisibility = Visibility.Collapsed;
        [ObservableProperty] private Visibility editorOverlayVisibility = Visibility.Collapsed;
        [ObservableProperty] private Visibility createPostButtonVisibility = Visibility.Collapsed;

        public IRelayCommand LoadMoreCommand { get; }
        public IRelayCommand ShowCreatePostPanelCommand { get; }

        public event EventHandler<Post>? PostClicked;
        public event EventHandler<Post>? PostEditRequested;
        public event EventHandler? PostUploaded;

        public NewsViewModel()
        {
            service = App.GetService<INewsService>();

            LoadMoreCommand = new RelayCommand(() => LoadPosts(false));
            ShowCreatePostPanelCommand = new RelayCommand(() => EditorOverlayVisibility = Visibility.Visible);

            createPostButtonVisibility = App.CurrentUser.IsDeveloper ? Visibility.Visible : Visibility.Collapsed;
            LoadPosts(true);
        }

        public void LoadPosts(bool reset, string searchOverride = "")
        {
            if (reset)
            {
                PositionedPosts.Clear();
                CurrentPage = 0;
            }

            ++CurrentPage;

            string search = string.IsNullOrEmpty(searchOverride) ? SearchText : searchOverride;
            var posts = service.LoadNextPosts(CurrentPage, search);

            int startIndex = PositionedPosts.Count;
            for (int i = 0; i < posts.Count; i++)
            {
                var positioned = new PostWithPosition(posts[i], startIndex + i, DIVISOR);
                PositionedPosts.Add(positioned);
            }

            SearchText = string.Empty;
        }

        public void HandlePostClicked(Post post)
        {
            PostOverlayVisibility = Visibility.Visible;
            PostClicked?.Invoke(this, post);
        }

        public void HandlePostUploaded()
        {
            EditorOverlayVisibility = Visibility.Collapsed;
            LoadPosts(true);
            PostUploaded?.Invoke(this, EventArgs.Empty);
        }

        public void HandlePostEditRequested(Post post)
        {
            EditorOverlayVisibility = Visibility.Visible;
            PostEditRequested?.Invoke(this, post);
        }

        public void CloseOverlays()
        {
            PostOverlayVisibility = Visibility.Collapsed;
            EditorOverlayVisibility = Visibility.Collapsed;
        }
    }
}
