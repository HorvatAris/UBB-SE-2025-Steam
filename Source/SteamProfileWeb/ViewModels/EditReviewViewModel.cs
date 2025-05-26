using System.ComponentModel.DataAnnotations;

namespace SteamProfileWeb.ViewModels
{
    public class EditReviewViewModel
    {
        public int ReviewId { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Range(1, 5)]
        public double Rating { get; set; }
        public bool IsRecommended { get; set; }
        public int GameId { get; set; }
    }
} 