using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models
{
    public class Collection
    {
        // Constants for validation rules and error messages
        private const int NameMaximumLength = 100;
        private const int NameMinimumLength = 1;
        private const int CoverPictureMaxLength = 255;

        private const string NameLengthError = "Name must be between 1 and 100 characters";
        private const string CoverPictureLengthError = "Cover picture URL cannot exceed 255 characters";

        public int CollectionId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(NameMaximumLength, MinimumLength = NameMinimumLength, ErrorMessage = NameLengthError)]
        public string CollectionName { get; set; } = string.Empty;

        [StringLength(CoverPictureMaxLength, ErrorMessage = CoverPictureLengthError)]
        public string? CoverPicture { get; set; }

        public bool IsPublic { get; set; }

        [Required]
        public DateOnly CreatedAt { get; set; }

        public List<OwnedGame> Games { get; set; } = new();
        public bool IsAllOwnedGamesCollection { get; }

        public Collection(int userId, string collectionName, DateOnly createdAt, string? coverPicture = null, bool isPublic = false)
        {
            UserId = userId;
            CollectionName = collectionName;
            CreatedAt = createdAt;
            CoverPicture = coverPicture;
            IsPublic = isPublic;
            IsAllOwnedGamesCollection = false;
        }
    }
}