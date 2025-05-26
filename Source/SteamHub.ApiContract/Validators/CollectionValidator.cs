using System.ComponentModel.DataAnnotations;
using SteamHub.ApiContract.Models.Collections;

namespace SteamHub.ApiContract.Validators
{
    /// <summary>
    /// Provides validation methods for Collection entities.
    /// </summary>
    public static class CollectionValidator
    {
        private const int MinimumUserId = 1;
        private const int MaximumNameLength = 100;
        private const int MaximumCoverPictureLength = 255;

        private const string ErrorInvalidUserId = "User ID must be greater than 0";
        private const string ErrorInvalidName = "Name cannot be empty or longer than 100 characters";
        private const string ErrorInvalidCoverPicture = "Cover picture URL cannot exceed 255 characters";

        /// <summary>
        /// Determines whether the specified user ID is valid (>= 1).
        /// </summary>
        public static bool IsUserIdValid(int userId) => userId >= MinimumUserId;

        /// <summary>
        /// Determines whether the collection name meets length and non-empty requirements.
        /// </summary>
        public static bool IsNameValid(string collectionName)
            => !string.IsNullOrWhiteSpace(collectionName) && collectionName.Length <= MaximumNameLength;

        /// <summary>
        /// Determines whether the cover picture URL meets length requirements or is null.
        /// </summary>
        public static bool IsCoverPictureValid(string? coverPicture)
            => coverPicture == null || coverPicture.Length <= MaximumCoverPictureLength;

        /// <summary>
        /// Validates all properties of a <see cref="Collection"/> and throws <see cref="ValidationException"/> on failure.
        /// </summary>
        public static void ValidateCollection(Collection collection)
        {
            if (!IsUserIdValid(collection.UserId))
                throw new ValidationException(ErrorInvalidUserId);

            if (!IsNameValid(collection.CollectionName))
                throw new ValidationException(ErrorInvalidName);

            if (!IsCoverPictureValid(collection.CoverPicture))
                throw new ValidationException(ErrorInvalidCoverPicture);
        }
    }
}