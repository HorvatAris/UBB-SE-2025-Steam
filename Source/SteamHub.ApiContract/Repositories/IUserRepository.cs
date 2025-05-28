using SteamHub.ApiContract.Models.User;

namespace SteamHub.ApiContract.Repositories
{
    
    /// <summary>
    /// Defines data access operations for <see cref="User"/> entities.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Creates a new user asynchronously.
        /// </summary>
        /// <param name="request">The request details for creating a user.</param>
        /// <returns>The creation response containing the new user ID.</returns>
        Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);

        /// <summary>
        /// Retrieves a user by ID asynchronously.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>The user response or null if not found.</returns>
        Task<User?> GetUserByIdAsync(int id);

        /// <summary>
        /// Retrieves all users asynchronously.
        /// </summary>
        /// <returns>A response containing a list of users.</returns>
        Task<GetUsersResponse?> GetUsersAsync();

        /// <summary>
        /// Updates an existing user asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="request">The update request details.</param>
        Task UpdateUserAsync(int userId, UpdateUserRequest request);

        /// <summary>
        /// Retrieves all users asynchronously.
        /// </summary>
        /// <returns>A list of user DTOs.</returns>
        Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// Updates an existing user asynchronously.
        /// </summary>
        /// <param name="user">The user DTO with updated values.</param>
        /// <returns>The updated user DTO.</returns>
        Task<User> UpdateUserAsync(User user);

        /// <summary>
        /// Creates a new user asynchronously.
        /// </summary>
        /// <param name="user">The user DTO to create.</param>
        /// <returns>The created user DTO, including assigned ID.</returns>
        Task<User> CreateUserAsync(User user);

        /// <summary>
        /// Deletes a user by ID asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        Task DeleteUserAsync(int userId);

        /// <summary>
        /// Verifies credentials by email or username asynchronously.
        /// </summary>
        /// <param name="emailOrUsername">The email or username to verify.</param>
        /// <returns>The user DTO or null if not found.</returns>
        Task<User?> VerifyCredentialsAsync(string emailOrUsername);

        /// <summary>
        /// Retrieves a user by email asynchronously.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <returns>The user DTO or null if not found.</returns>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Retrieves a user by username asynchronously.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The user DTO or null if not found.</returns>
        Task<User?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Checks if an email or username already exists asynchronously.
        /// </summary>
        /// <param name="email">Email to check.</param>
        /// <param name="username">Username to check.</param>
        /// <returns>A code indicating which field exists or null if none.</returns>
        Task<string> CheckUserExistsAsync(string email, string username);

        /// <summary>
        /// Changes a user's email address asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="newEmail">The new email address.</param>
        Task ChangeEmailAsync(int userId, string newEmail);

        /// <summary>
        /// Changes a user's password asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="newPassword">The new password hash.</param>
        Task ChangePasswordAsync(int userId, string newPassword);

        /// <summary>
        /// Changes a user's username asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="newUsername">The new username.</param>
        Task ChangeUsernameAsync(int userId, string newUsername);

        /// <summary>
        /// Updates the last login timestamp for a user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        Task UpdateLastLoginAsync(int userId);

        /// <summary>
        /// Updates the bio of the user asynchronously.
        /// </summary>
        /// <param name="bio">Bio to update.</param>
        /// <param name="userId">The user ID.</param>
        Task UpdateProfileBioAsync(int userId, string bio);

        /// <summary>
        /// Updates the profile picture of the user asynchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="localImagePath">The local path to the image file.</param>
        Task UpdateProfilePictureAsync(int userId, string localImagePath);
    }
}