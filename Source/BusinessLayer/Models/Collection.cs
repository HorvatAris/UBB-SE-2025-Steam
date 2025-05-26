using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessLayer.Models
{
    [Table("Collections")]
    public class Collection
    {
        // Constants for validation rules and error messages
        private const int NameMaximumLength = 100;
        private const int NameMinimumLength = 1;
        private const int CoverPictureMaxLength = 255;
        private const string NameLengthError = "Name must be between 1 and 100 characters";
        private const string CoverPictureLengthError = "Cover picture URL cannot exceed 255 characters";

        [Column("collection_id")]
        public int CollectionId { get; set; }

        [Column("user_id")]
        [Required]
        public int UserId { get; set; }

        [Column("name")]
        [Required]
        [StringLength(NameMaximumLength, MinimumLength = NameMinimumLength, ErrorMessage = NameLengthError)]
        public string CollectionName { get; set; } = string.Empty;

        [Column("cover_picture")]
        [StringLength(CoverPictureMaxLength, ErrorMessage = CoverPictureLengthError)]
        public string? CoverPicture { get; set; }

        [Column("is_public")]
        public bool IsPublic { get; set; }

        [Column("created_at")]
        [Required]
        public DateOnly CreatedAt { get; set; }

        [NotMapped] // Use this for properties that don't map to DB columns
        public List<OwnedGame> Games { get; set; } = new();

        [NotMapped] // Use this for properties that don't map to DB columns
        public List<CollectionGame> CollectionGames { get; set; } = new();

        [NotMapped] // This is a computed property
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