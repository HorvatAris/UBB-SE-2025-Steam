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
        Task<UserResponse?> GetUserByIdAsync(int id);

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
        /// Retrieves all users synchronously.
        /// </summary>
        /// <returns>A list of user DTOs.</returns>
        List<User> GetAllUsers();

        /// <summary>
        /// Retrieves a user by ID synchronously.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>The user DTO or null if not found.</returns>
        User? GetUserById(int userId);

        /// <summary>
        /// Updates an existing user synchronously.
        /// </summary>
        /// <param name="user">The user DTO with updated values.</param>
        /// <returns>The updated user DTO.</returns>
        User UpdateUser(User user);

        /// <summary>
        /// Creates a new user synchronously.
        /// </summary>
        /// <param name="user">The user DTO to create.</param>
        /// <returns>The created user DTO, including assigned ID.</returns>
        User CreateUser(User user);

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        void DeleteUser(int userId);

        /// <summary>
        /// Verifies credentials by email or username.
        /// </summary>
        /// <param name="emailOrUsername">The email or username to verify.</param>
        /// <returns>The user DTO or null if not found.</returns>
        User? VerifyCredentials(string emailOrUsername);

        /// <summary>
        /// Retrieves a user by email.
        /// </summary>
        /// <param name="email">The email address.</param>
        /// <returns>The user DTO or null if not found.</returns>
        User? GetUserByEmail(string email);

        /// <summary>
        /// Retrieves a user by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The user DTO or null if not found.</returns>
        User? GetUserByUsername(string username);

        /// <summary>
        /// Checks if an email or username already exists.
        /// </summary>
        /// <param name="email">Email to check.</param>
        /// <param name="username">Username to check.</param>
        /// <returns>A code indicating which field exists or null if none.</returns>
        string CheckUserExists(string email, string username);

        /// <summary>
        /// Changes a user's email address.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="newEmail">The new email address.</param>
        void ChangeEmail(int userId, string newEmail);

        /// <summary>
        /// Changes a user's password.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="newPassword">The new password hash.</param>
        void ChangePassword(int userId, string newPassword);

        /// <summary>
        /// Changes a user's username.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="newUsername">The new username.</param>
        void ChangeUsername(int userId, string newUsername);

        /// <summary>
        /// Updates the last login timestamp for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        void UpdateLastLogin(int userId);
    }
}