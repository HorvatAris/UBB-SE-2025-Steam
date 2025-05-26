using System.ComponentModel.DataAnnotations;
using BusinessLayer.Models;

namespace BusinessLayer.Validators
{
    public static class OwnedGameValidator
    {
        // Validation constants
        private const int MinimumUserId = 1;
        private const int MaximumTitleLength = 100;
        private const int MaximumCoverPictureLength = 255;

        // Validation error message constants
        private const string ErrorInvalidUserId = "User ID must be greater than 0";
        private const string ErrorInvalidTitle = "Title cannot be empty or longer than 100 characters";
        private const string ErrorInvalidCoverPicture = "Cover picture URL cannot exceed 255 characters";

        public static bool IsUserIdValid(int userId)
        {
            return userId >= MinimumUserId;
        }

        public static bool IsTitleValid(string title)
        {
            return !string.IsNullOrWhiteSpace(title) && title.Length <= MaximumTitleLength;
        }

        public static bool IsCoverPictureValid(string? coverPicture)
        {
            return coverPicture == null || coverPicture.Length <= MaximumCoverPictureLength;
        }

        public static void ValidateOwnedGame(OwnedGame ownedGame)
        {
            if (!IsUserIdValid(ownedGame.UserId))
            {
                throw new ValidationException(ErrorInvalidUserId);
            }

            if (!IsTitleValid(ownedGame.GameTitle))
            {
                throw new ValidationException(ErrorInvalidTitle);
            }

            if (!IsCoverPictureValid(ownedGame.CoverPicture))
            {
                throw new ValidationException(ErrorInvalidCoverPicture);
            }
        }
    }
}