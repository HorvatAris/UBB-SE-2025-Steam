using System.ComponentModel.DataAnnotations;
using BusinessLayer.Models;

namespace BusinessLayer.Validators
{
    public static class CollectionValidator
    {
        // Validation constants
        private const int MinimumUserId = 1;
        private const int MaximumNameLength = 100;
        private const int MaximumCoverPictureLength = 255;

        // Validation error message constants
        private const string ErrorInvalidUserId = "User ID must be greater than 0";
        private const string ErrorInvalidName = "Name cannot be empty or longer than 100 characters";
        private const string ErrorInvalidCoverPicture = "Cover picture URL cannot exceed 255 characters";

        public static bool IsUserIdValid(int userId)
        {
            return userId >= MinimumUserId;
        }

        public static bool IsNameValid(string collectionName)
        {
            return !string.IsNullOrWhiteSpace(collectionName) && collectionName.Length <= MaximumNameLength;
        }

        public static bool IsCoverPictureValid(string? coverPicture)
        {
            return coverPicture == null || coverPicture.Length <= MaximumCoverPictureLength;
        }

        public static void ValidateCollection(Collection collection)
        {
            if (!IsUserIdValid(collection.UserId))
            {
                throw new ValidationException(ErrorInvalidUserId);
            }

            if (!IsNameValid(collection.CollectionName))
            {
                throw new ValidationException(ErrorInvalidName);
            }

            if (!IsCoverPictureValid(collection.CoverPicture))
            {
                throw new ValidationException(ErrorInvalidCoverPicture);
            }
        }
    }
}