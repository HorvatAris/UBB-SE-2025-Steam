using System.ComponentModel.DataAnnotations;
using SteamHub.ApiContract.Models.Game;

namespace SteamHub.ApiContract.Validators
{
    /// <summary>
    /// Provides validation methods for OwnedGame entities.
    /// </summary>
    public static class OwnedGameValidator
    {
        private const int MinimumUserId = 1;
        private const int MaximumTitleLength = 100;
        private const int MaximumCoverPictureLength = 255;

        private const string ErrorInvalidUserId = "User ID must be greater than 0";
        private const string ErrorInvalidTitle = "Title cannot be empty or longer than 100 characters";
        private const string ErrorInvalidCoverPicture = "Cover picture URL cannot exceed 255 characters";

        /// <summary>
        /// Checks if the user ID is valid (>= 1).
        /// </summary>
        public static bool IsUserIdValid(int userId) => userId >= MinimumUserId;

        /// <summary>
        /// Checks if the game title meets length and non-empty requirements.
        /// </summary>
        public static bool IsTitleValid(string title)
            => !string.IsNullOrWhiteSpace(title) && title.Length <= MaximumTitleLength;

        /// <summary>
        /// Checks if the cover picture URL meets length requirements or is null.
        /// </summary>
        public static bool IsCoverPictureValid(string? coverPicture)
            => coverPicture == null || coverPicture.Length <= MaximumCoverPictureLength;

        /// <summary>
        /// Validates all properties of an <see cref="OwnedGame"/> and throws <see cref="ValidationException"/> on failure.
        /// </summary>
        public static void ValidateOwnedGame(OwnedGame ownedGame)
        {
            if (!IsUserIdValid(ownedGame.UserId))
                throw new ValidationException(ErrorInvalidUserId);

            if (!IsTitleValid(ownedGame.GameTitle))
                throw new ValidationException(ErrorInvalidTitle);

            if (!IsCoverPictureValid(ownedGame.CoverPicture))
                throw new ValidationException(ErrorInvalidCoverPicture);
        }
    }
}