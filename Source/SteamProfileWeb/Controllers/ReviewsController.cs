using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamProfileWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace SteamProfileWeb.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserService _userService;

        public ReviewsController(IReviewService reviewService, IReviewRepository reviewRepository, IUserService userService)
        {
            _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
            _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet]
        public IActionResult Index(int gameId, string sortOption = "Newest", string recommendationFilter = "All")
        {
            var reviews = _reviewService.GetAllReviewsForAGame(gameId);
            
            // Map the recommendation filter
            switch (recommendationFilter)
            {
                case "Positive":
                    reviews = reviews.Where(r => r.IsRecommended).ToList();
                    break;
                case "Negative":
                    reviews = reviews.Where(r => !r.IsRecommended).ToList();
                    break;
                // "All" case is handled by not filtering
            }

            // Map the sort option
            switch (sortOption)
            {
                case "Newest":
                    reviews = reviews.OrderByDescending(r => r.DateAndTimeWhenReviewWasCreated).ToList();
                    break;
                case "Oldest":
                    reviews = reviews.OrderBy(r => r.DateAndTimeWhenReviewWasCreated).ToList();
                    break;
                case "Highest":
                    reviews = reviews.OrderByDescending(r => r.NumericRatingGivenByUser).ToList();
                    break;
                case "Helpful":
                    reviews = reviews.OrderByDescending(r => r.TotalHelpfulVotesReceived).ToList();
                    break;
                default:
                    reviews = reviews.OrderByDescending(r => r.DateAndTimeWhenReviewWasCreated).ToList();
                    break;
            }

            var (total, positive, avg) = _reviewService.GetReviewStatisticsForGame(gameId);
            int currentUserId = GetCurrentUserId();

            var model = new ReviewsViewModel
            {
                GameId = gameId,
                Reviews = reviews,
                SortOption = sortOption,
                RecommendationFilter = recommendationFilter,
                TotalReviews = total,
                PositiveReviewPercentage = positive,
                AverageRating = avg,
                CurrentUserId = currentUserId
            };
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create(int gameId)
        {
            var model = new CreateReviewViewModel { GameId = gameId };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int gameId, CreateReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var review = new Review
                {
                    GameIdentifier = gameId,
                    UserIdentifier = GetCurrentUserId(),
                    ReviewTitleText = model.Title,
                    ReviewContentText = model.Content,
                    NumericRatingGivenByUser = model.Rating,
                    IsRecommended = model.IsRecommended,
                    DateAndTimeWhenReviewWasCreated = DateTime.Now
                };

                var success = _reviewService.SubmitReview(review);
                TempData[success ? "SuccessMessage" : "ErrorMessage"] = success ? "Review submitted!" : "Failed to submit review.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating review: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new { gameId });
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit(int id)
        {
            var review = _reviewService.GetAllReviewsForAGame(0).FirstOrDefault(r => r.ReviewIdentifier == id);
            if (review == null || review.UserIdentifier != GetCurrentUserId())
                return NotFound();

            var model = new EditReviewViewModel
            {
                ReviewId = review.ReviewIdentifier,
                Title = review.ReviewTitleText,
                Content = review.ReviewContentText,
                Rating = review.NumericRatingGivenByUser,
                IsRecommended = review.IsRecommended,
                GameId = review.GameIdentifier
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EditReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var review = new Review
                {
                    ReviewIdentifier = id,
                    ReviewTitleText = model.Title,
                    ReviewContentText = model.Content,
                    NumericRatingGivenByUser = model.Rating,
                    IsRecommended = model.IsRecommended,
                    GameIdentifier = model.GameId,
                    UserIdentifier = GetCurrentUserId(),
                    DateAndTimeWhenReviewWasCreated = DateTime.Now
                };

                var success = _reviewService.EditReview(review);
                TempData[success ? "SuccessMessage" : "ErrorMessage"] = success ? "Review updated!" : "Failed to update review.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating review: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new { gameId = model.GameId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            int gameId = 0;
            try
            {
                var review = _reviewService.GetAllReviewsForAGame(0).FirstOrDefault(r => r.ReviewIdentifier == id);
                if (review == null || review.UserIdentifier != GetCurrentUserId())
                    return NotFound();

                gameId = review.GameIdentifier;
                var success = _reviewService.DeleteReview(id);
                TempData[success ? "SuccessMessage" : "ErrorMessage"] = success ? "Review deleted!" : "Failed to delete review.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting review: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new { gameId });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Vote(int reviewId, string voteType)
        {
            try
            {
                var review = _reviewService.GetAllReviewsForAGame(0).FirstOrDefault(r => r.ReviewIdentifier == reviewId);
                if (review == null)
                    return NotFound();

                // Check if the user has already voted
                int currentVoteCount = voteType == "Funny" ? review.TotalFunnyVotesReceived : review.TotalHelpfulVotesReceived;
                bool shouldIncrementVoteCount = currentVoteCount == 0; // If count is 0, we're adding a vote

                var success = _reviewService.ToggleVote(reviewId, voteType, shouldIncrementVoteCount);

                // Get the updated vote count
                var updatedReview = _reviewService.GetAllReviewsForAGame(0).FirstOrDefault(r => r.ReviewIdentifier == reviewId);
                var count = voteType == "Funny" ? updatedReview.TotalFunnyVotesReceived : updatedReview.TotalHelpfulVotesReceived;

                return Json(new { success = success, count = count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                // First, try to get user from UserService
                try
                {
                    var currentUser = _userService.GetCurrentUser();
                    if (currentUser != null)
                    {
                        return (int)currentUser.UserId;
                    }
                }
                catch
                {
                    // Fall through to other methods if this fails
                }

                // Then, try to get from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }

                // Try to get from name claim
                var nameClaim = User.FindFirst(ClaimTypes.Name);
                if (nameClaim != null)
                {
                    // Look up user by username
                    try
                    {
                        var user = _userService.GetUserByUsername(nameClaim.Value);
                        if (user != null)
                        {
                            return (int)user.UserId;
                        }
                    }
                    catch
                    {
                        // Continue to fallback
                    }
                }
            }

            // If we get here and can't determine user ID, log it for debugging
            Console.WriteLine("Warning: Unable to determine current user ID, defaulting to 1");
            return 1; // Default to user ID 1 if not authenticated
        }
    }
} 