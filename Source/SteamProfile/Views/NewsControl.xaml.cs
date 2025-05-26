using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.ViewModels;

namespace SteamProfile.Views
{
    public sealed partial class NewsControl : UserControl
    {
        public NewsViewModel ViewModel { get; } = new();

        public NewsControl()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;

            ViewModel.PostClicked += (_, post) => News_PostControl.SetPostData(post);
            ViewModel.PostEditRequested += (_, post) => News_PostEditorPanel.SetPostToEdit(post);
            ViewModel.PostUploaded += (_, __) => { };

            News_PostControl.PanelClosed += (_, __) => ViewModel.CloseOverlays();
            News_PostEditorPanel.PostUploaded += (_, __) => ViewModel.HandlePostUploaded();
        }

        private void SearchBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ViewModel.LoadPosts(true, News_SearchBox.Text.Trim());
                RefreshPostGrid();
            }
        }

        private void PostsScroller_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (News_PostsScroller.VerticalOffset >= News_PostsScroller.ScrollableHeight - 50)
            {
                ViewModel.LoadPosts(false);
            }
        }

        private void OverlayBackground_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ViewModel.CloseOverlays();
            e.Handled = true;
        }

        private void News_PostsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshPostGrid();
        }

        public void RefreshPostGrid()
        {
            News_PostsGrid.Children.Clear();
            News_PostsGrid.RowDefinitions.Clear();

            foreach (var post in ViewModel.PositionedPosts)
            {
                while (News_PostsGrid.RowDefinitions.Count <= post.Row)
                {
                    News_PostsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                }

                var preview = new PostPreviewControl();
                preview.SetPostData(post.Post);
                preview.PostClicked += (_, __) => ViewModel.HandlePostClicked(post.Post);

                Grid.SetRow(preview, post.Row);
                Grid.SetColumn(preview, post.Column);
                News_PostsGrid.Children.Add(preview);
            }
        }
    }
}
