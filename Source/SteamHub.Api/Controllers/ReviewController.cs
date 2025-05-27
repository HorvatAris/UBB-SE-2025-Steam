using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.Api.Controllers
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
        public async Task<IActionResult> GetReviewsForGame(int gameId)
        {
            var reviews = await reviewService.GetAllReviewsForAGame(gameId);
            return Ok(reviews);
        }

        [HttpGet("game/{gameId}/statistics")]
        public async Task<IActionResult> GetReviewStatistics(int gameId)
        {
            var (totalReviews, positivePercentage, averageRating) = await reviewService.GetReviewStatisticsForGame(gameId);
            return Ok(new
            {
                TotalReviews = totalReviews,
                PositivePercentage = positivePercentage,
                AverageRating = averageRating
            });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetReviewsByUser(int userId)
        {
            // Get all reviews and filter by user
            var allReviews = await reviewService.GetAllReviewsForAGame(0);
            var userReviews = allReviews.Where(review => review.UserIdentifier == userId).ToList();
            return Ok(userReviews);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReview(Review review)
        {
            var result = await reviewService.SubmitReview(review);
            if (result)
            {
                return Ok(true);
            }
            return BadRequest("Failed to submit review.");
        }

        [HttpPut("{reviewId}")]
        public async Task<IActionResult> EditReview(int reviewId, [FromBody] Review review)
        {
            var result = await reviewService.UpdateReview(reviewId, review);
            if (result)
            {
                return Ok(true);
            }
            return BadRequest("Failed to update review.");
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var result = await reviewService.DeleteReview(reviewId);
            if (result)
            {
                return Ok(true);
            }
            return BadRequest("Failed to delete review.");
        }

        [HttpPost("{reviewId}/vote")]
        public async Task<IActionResult> ToggleVote(int reviewId, [FromBody] VoteRequest request)
        {
            var result = await reviewService.ToggleVote(reviewId, request.VoteType, request.ShouldIncrement);
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