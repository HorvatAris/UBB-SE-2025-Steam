using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models
{
    public class OwnedGame
    {
        // Constants for validation
        private const int TitleMaximumLength = 100;
        private const int TitleMinimumLength = 1;
        private const int CoverPictureMaximumLength = 255;

        private const string TitleLengthError = "Title must be between 1 and 100 characters";
        private const string CoverPictureLengthError = "Cover picture URL cannot exceed 255 characters";

        public int GameId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(TitleMaximumLength, MinimumLength = TitleMinimumLength, ErrorMessage = TitleLengthError)]
        public string GameTitle { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [StringLength(CoverPictureMaximumLength, ErrorMessage = CoverPictureLengthError)]
        public string? CoverPicture { get; set; }

        public OwnedGame(int userId, string gameTitile, string description, string? coverPicture = null)
        {
            UserId = userId;
            GameTitle = gameTitile;
            Description = description;
            CoverPicture = coverPicture;
        }
    }
}