using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.Models;

namespace SteamProfileWeb.ViewModels
{
    public class ReviewsViewModel
    {
        public int GameId { get; set; }
        public List<Review> Reviews { get; set; } = new();
        public string SortOption { get; set; }
        public string RecommendationFilter { get; set; }
        public int TotalReviews { get; set; }
        public double PositiveReviewPercentage { get; set; }
        public double AverageRating { get; set; }
        public int CurrentUserId { get; set; }
    }

    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public string UserName { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public double Rating { get; set; }
        public bool IsRecommended { get; set; }
        public int HelpfulVotes { get; set; }
        public int FunnyVotes { get; set; }
        public string CreatedAt { get; set; }
        public int UserId { get; set; }
        // Add more fields as needed
    }
} 