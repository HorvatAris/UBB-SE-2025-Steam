using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ViewModels;

namespace SteamHub.Pages
{
    public sealed partial class ReviewExpandedView : Page
    {
        public ReviewViewModel ViewModel { get; private set; }

        public ReviewExpandedView(IReviewService reviewService)
        {
            this.InitializeComponent();
            ViewModel = new ReviewViewModel(reviewService);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is int gameId)
            {
                this.ViewModel.LoadReviewsForGame(gameId);
            }
        }

        private void OnBackClicked(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void OnSortChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e?.AddedItems.Count > 0 &&
                e.AddedItems[0] is ComboBoxItem { Content: string sortOption })
            {
                ViewModel.ApplySortinOption(sortOption);
            }
        }

        private void OnFilterChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e?.AddedItems.Count > 0 &&
                e.AddedItems[0] is ComboBoxItem { Content: string filterOption })
            {
                ViewModel.ApplyReccomendationFilter(filterOption);
            }
        }

        private void OnVoteHelpfulClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn &&
                btn.Tag is int reviewId &&
                btn.DataContext is Review review)
            {
                ViewModel.ToggleVoteForReview(reviewId, "Helpful", review);
            }
        }

        private void OnVoteFunnyClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn &&
                btn.Tag is int reviewId &&
                btn.DataContext is Review review)
            {
                ViewModel.ToggleVoteForReview(reviewId, "Funny", review);
            }
        }

        private void OnEditReviewClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn &&
                btn.DataContext is Review review)
            {
                ViewModel.EditAReview(review);
                if (Frame.CanGoBack)
                {
                    Frame.GoBack(); // go back to ReviewView to edit
                }
            }
        }

        private void OnDeleteReviewClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn &&
                btn.Tag is int reviewId)
            {
                ViewModel.DeleteSelectedReview(reviewId);
            }
        }
    }
}
