using BusinessLayer.Models;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SteamWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService reviewService;

        public ReviewController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        [HttpGet("game/{gameId}")]
        public IActionResult GetReviewsForGame(int gameId)
        {
            var reviews = reviewService.GetAllReviewsForAGame(gameId);
            return Ok(reviews);
        }

        [HttpGet("game/{gameId}/statistics")]
        public IActionResult GetReviewStatistics(int gameId)
        {
            var (totalReviews, positivePercentage, averageRating) = reviewService.GetReviewStatisticsForGame(gameId);
            return Ok(new
            {
                TotalReviews = totalReviews,
                PositivePercentage = positivePercentage,
                AverageRating = averageRating
            });
        }

        [HttpGet("{userId}")]
        public IActionResult GetReviewsByUser(int userId)
        {
            // Get all reviews and filter by user
            var allReviews = reviewService.GetAllReviewsForAGame(0);
            var userReviews = allReviews.Where(review => review.UserIdentifier == userId).ToList();
            return Ok(userReviews);
        }

        [HttpPost]
        public IActionResult SubmitReview(Review review)
        {
            var result = reviewService.SubmitReview(review);
            if (result)
            {
                return Ok(true);
            }
            return BadRequest("Failed to submit review.");
        }

        [HttpPut("{reviewId}")]
        public IActionResult EditReview(int reviewId, [FromBody] Review review)
        {
            var result = reviewService.UpdateReview(reviewId, review);
            if (result)
            {
                return Ok(true);
            }
            return BadRequest("Failed to update review.");
        }

        [HttpDelete("{reviewId}")]
        public IActionResult DeleteReview(int reviewId)
        {
            var result = reviewService.DeleteReview(reviewId);
            if (result)
            {
                return Ok(true);
            }
            return BadRequest("Failed to delete review.");
        }

        [HttpPost("{reviewId}/vote")]
        public IActionResult ToggleVote(int reviewId, [FromBody] VoteRequest request)
        {
            var result = reviewService.ToggleVote(reviewId, request.VoteType, request.ShouldIncrement);
            if (result)
            {
                return Ok(true);
            }
            return BadRequest("Failed to toggle vote.");
        }
    }

    public class VoteRequest
    {
        public string VoteType { get; set; }
        public bool ShouldIncrement { get; set; }
    }
}