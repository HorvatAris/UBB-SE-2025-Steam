using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.ViewModels;

namespace SteamProfile.Views
{
    public sealed partial class ReviewsPage : Page
    {
        private readonly ReviewViewModel reviewViewModel;

        public ReviewsPage()
        {
            reviewViewModel = new ReviewViewModel();
            DataContext = reviewViewModel;
            reviewViewModel.OnValidationFailed = ShowValidationMessage;

            InitializeComponent();
        }

        private void OnWriteReviewClicked(object sender, RoutedEventArgs e)
        {
            if (ReviewPanel == null)
            {
                return;
            }

            ReviewPanel.Visibility = ReviewPanel.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void OnSubmitReviewClicked(object sender, RoutedEventArgs e)
        {
            reviewViewModel.SubmitNewReview();
            ReviewPanel.Visibility = Visibility.Collapsed;
        }

        private void OnSortChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e?.AddedItems.Count > 0 &&
                e.AddedItems[0] is ComboBoxItem { Content: string sortOption } &&
                !string.IsNullOrWhiteSpace(sortOption))
            {
                reviewViewModel.ApplySortinOption(sortOption);
            }
        }

        private void OnFilterChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e?.AddedItems.Count > 0 &&
                e.AddedItems[0] is ComboBoxItem { Content: string filter } &&
                !string.IsNullOrWhiteSpace(filter))
            {
                reviewViewModel.ApplyReccomendationFilter(filter);
            }
        }

        private void OnEditReviewClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn &&
                btn.DataContext is BusinessLayer.Models.Review review)
            {
                reviewViewModel.EditAReview(review);

                // Ensure ReviewPanel is visible
                if (ReviewPanel != null)
                {
                    ReviewPanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void OnDeleteReviewClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn &&
                btn.Tag is int reviewId)
            {
                reviewViewModel.DeleteSelectedReview(reviewId);
            }
        }

        private void OnVoteHelpfulClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn &&
                btn.DataContext is BusinessLayer.Models.Review review)
            {
                reviewViewModel.ToggleVoteForReview(review.ReviewIdentifier, "Helpful", review);
            }
        }

        private void OnVoteFunnyClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn &&
                btn.DataContext is BusinessLayer.Models.Review review)
            {
                reviewViewModel.ToggleVoteForReview(review.ReviewIdentifier, "Funny", review);
            }
        }

        public string FormatHoursPlayed(int hours)
        {
            return $"Played {hours} hour{(hours == 1 ? string.Empty : "s")}";
        }

        private async void ShowValidationMessage(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Missing Information",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }
        private void GoBack(object sender, RoutedEventArgs eventArgs)
        {
            Frame.GoBack();
        }
    }
}