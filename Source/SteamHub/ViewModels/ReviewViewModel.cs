﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.IO;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models;
using System.Threading.Tasks;

namespace SteamHub.ViewModels
{
    public partial class ReviewViewModel : INotifyPropertyChanged
    {
        private readonly IReviewService reviewService;
        private readonly IGameService gameService;
        private int currentGameIdentifier;
        public int CurrentGameId => currentGameIdentifier;

        private string gameImagePath;
        public string GameImagePath
        {
            get => gameImagePath;
            set
            {
                gameImagePath = value;
                OnPropertyChanged();
            }
        }

        public Action<string>? OnValidationFailed;

        private const int CurrentUserId = 1;
        private bool isEditingReview = false;
        private int? editingReviewId = null;

        public ObservableCollection<Review> CollectionOfGameReviews { get; set; } = new();
        public Review ReviewCurrentlyBeingWritten { get; set; } = new();

        public string CurrentSortOption { get; set; } = "Newest First";
        public string CurrentRecommendationFilter { get; set; } = "All Reviews";

        private int totalReviews;
        public int TotalNumberOfReviews
        {
            get => totalReviews;
            set
            {
                totalReviews = value;
                OnPropertyChanged();
            }
        }

        private double positiveReviewPercentage;
        public double PercentageOfPositiveReviews
        {
            get => positiveReviewPercentage;
            set
            {
                positiveReviewPercentage = value;
                OnPropertyChanged();
            }
        }

        private double averageRatingScore;
        public double AverageRatingAcrossAllReviews
        {
            get => averageRatingScore;
            set
            {
                averageRatingScore = value;
                OnPropertyChanged();
            }
        }

        public ReviewViewModel(IReviewService reviewsService, IGameService gameService)
        {
            reviewService = reviewsService;
            this.gameService = gameService;
        }

        public async Task LoadReviewsForGame(int gameIdentifier)
        {
            try
            {
                currentGameIdentifier = gameIdentifier;

                var reviews = await reviewService.GetAllReviewsForAGame(gameIdentifier);
                reviews = await reviewService.FilterReviewsByRecommendation(reviews, CurrentRecommendationFilter);
                reviews = await reviewService.SortReviews(reviews, CurrentSortOption);

                CollectionOfGameReviews.Clear();
                foreach (var review in reviews)
                {
                    if (review.GameIdentifier == gameIdentifier)
                    {
                        CollectionOfGameReviews.Add(review);
                    }   
                }

                UpdateReviewStatistics();
            }
            catch (Exception ex)
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string path = Path.Combine(desktop, "load_reviews_internal_error.txt");
                File.WriteAllText(path, ex.ToString());
                throw;
            }
        }

        public async void SubmitNewReview()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(ReviewCurrentlyBeingWritten.ReviewContentText) ||
                ReviewCurrentlyBeingWritten.NumericRatingGivenByUser <= 0)
            {
                // Send signal to UI to show a validation message
                OnValidationFailed?.Invoke("Please fill in the required fields: Review Content and Rating.");
                return;
            }

            ReviewCurrentlyBeingWritten.GameIdentifier = currentGameIdentifier;
            ReviewCurrentlyBeingWritten.UserIdentifier = CurrentUserId;
            ReviewCurrentlyBeingWritten.DateAndTimeWhenReviewWasCreated = DateTime.Now;

            bool success;
            if (isEditingReview && editingReviewId.HasValue)
            {
                ReviewCurrentlyBeingWritten.ReviewIdentifier = editingReviewId.Value;
                success = await reviewService.EditReview(ReviewCurrentlyBeingWritten);
            }
            else
            {
                success = await reviewService.SubmitReview(ReviewCurrentlyBeingWritten);
            }

            if (success)
            {
                ReviewCurrentlyBeingWritten = new Review();
                OnPropertyChanged(nameof(ReviewCurrentlyBeingWritten));

                isEditingReview = false;
                editingReviewId = null;
                await LoadReviewsForGame(currentGameIdentifier);
            }
        }

        public void EditAReview(Review review)
        {
            if (review.UserIdentifier != CurrentUserId)
            {
                return;
            }

            isEditingReview = true;
            editingReviewId = review.ReviewIdentifier;
            ReviewCurrentlyBeingWritten = new Review
            {
                ReviewIdentifier = review.ReviewIdentifier,
                ReviewTitleText = review.ReviewTitleText,
                ReviewContentText = review.ReviewContentText,
                NumericRatingGivenByUser = review.NumericRatingGivenByUser,
                IsRecommended = review.IsRecommended,
                GameIdentifier = review.GameIdentifier,
                UserIdentifier = review.UserIdentifier,
                TitleOfGame = review.ReviewTitleText
            };

            OnPropertyChanged(nameof(ReviewCurrentlyBeingWritten));
        }

        public async void DeleteSelectedReview(int reviewIdentifier)
        {
            if (await reviewService.DeleteReview(reviewIdentifier))
            {
                await LoadReviewsForGame(currentGameIdentifier);
            }
        }

        public void ToggleVoteForReview(int reviewId, string voteType, Review review)
        {
            bool shouldIncrement;

            if (voteType == "Helpful")
            {
                shouldIncrement = !review.HasVotedHelpful;
                review.HasVotedHelpful = !review.HasVotedHelpful;
                review.TotalHelpfulVotesReceived += shouldIncrement ? 1 : -1;
            }
            else if (voteType == "Funny")
            {
                shouldIncrement = !review.HasVotedFunny;
                review.HasVotedFunny = !review.HasVotedFunny;
                review.TotalFunnyVotesReceived += shouldIncrement ? 1 : -1;
            }
            else
            {
                return;
            }

            reviewService.ToggleVote(reviewId, voteType, shouldIncrement);
            review.OnPropertyChanged(nameof(review.TotalHelpfulVotesReceived));
            review.OnPropertyChanged(nameof(review.HasVotedHelpful));
            review.OnPropertyChanged(nameof(review.TotalFunnyVotesReceived));
            review.OnPropertyChanged(nameof(review.HasVotedFunny));
        }

        public void ApplyReccomendationFilter(string filter)
        {
            CurrentRecommendationFilter = filter;
            _ = LoadReviewsForGame(currentGameIdentifier);
        }

        public void ApplySortinOption(string sortOption)
        {
            CurrentSortOption = sortOption;
            _ = LoadReviewsForGame(currentGameIdentifier);
        }

        private async void UpdateReviewStatistics()
        {
            var (totalReviews, positivePercentage, averageRating) = await reviewService.GetReviewStatisticsForGame(currentGameIdentifier);
            TotalNumberOfReviews = totalReviews;
            PercentageOfPositiveReviews = positivePercentage;
            AverageRatingAcrossAllReviews = averageRating;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
