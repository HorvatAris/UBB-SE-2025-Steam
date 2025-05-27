using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SteamHub.ApiContract.Proxies;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ApiContract.Services
{
    /// <summary>
    /// Provides business logic for user management, wrapping repository calls and session handling.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly ISessionService sessionService;

        /// <summary>
        /// Initializes a new instance of <see cref="UserService"/>.
        /// </summary>
        /// <param name="userRepository">The underlying user repository.</param>
        /// <param name="sessionService">The session service for managing user sessions.</param>
        public UserService(IUserRepository userRepository, ISessionService sessionService)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        /// <summary>
        /// Retrieves all users from the repository.
        /// </summary>
        public List<User> GetAllUsers()
            => userRepository.GetAllUsers();

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The identifier of the user to retrieve.</param>
        public User GetUserByIdentifier(int userId)
            => userRepository.GetUserById(userId);

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        public User GetUserByEmail(string email)
            => userRepository.GetUserByEmail(email);

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        public User? GetUserByUsername(string username)
            => userRepository.GetUserByUsername(username);

        /// <summary>
        /// Validates that the given email and username are not already in use.
        /// </summary>
        /// <param name="email">The email to validate.</param>
        /// <param name="username">The username to validate.</param>
        /// <exception cref="EmailAlreadyExistsException">Thrown if the email is already registered.</exception>
        /// <exception cref="UsernameAlreadyTakenException">Thrown if the username is already taken.</exception>
        public void ValidateUserAndEmail(string email, string username)
        {
            var errorType = userRepository.CheckUserExists(email, username);
            if (errorType == "EMAIL_EXISTS") throw new EmailAlreadyExistsException(email);
            if (errorType == "USERNAME_EXISTS") throw new UsernameAlreadyTakenException(username);
        }

        /// <summary>
        /// Creates a new user after validating uniqueness and hashing the password.
        /// </summary>
        /// <param name="user">The user DTO containing new user details.</param>
        public User CreateUser(User user)
        {
            ValidateUserAndEmail(user.Email, user.Username);
            user.Password = PasswordHasher.HashPassword(user.Password);
            return userRepository.CreateUser(user);
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The user DTO with updated details.</param>
        public User UpdateUser(User user)
            => userRepository.UpdateUser(user);

        /// <summary>
        /// Deletes a user by their identifier.
        /// </summary>
        /// <param name="userId">The identifier of the user to delete.</param>
        public void DeleteUser(int userId)
            => userRepository.DeleteUser(userId);

        /// <summary>
        /// Verifies a user's current password.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="givenPassword">The password to verify.</param>
        public bool AcceptChanges(int userId, string givenPassword)
        {
            var user = userRepository.GetUserById(userId);
            return PasswordHasher.VerifyPassword(givenPassword, user.Password);
        }

        /// <summary>
        /// Updates a user's email address.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="newEmail">The new email address.</param>
        public void UpdateUserEmail(int userId, string newEmail)
            => userRepository.ChangeEmail(userId, newEmail);

        /// <summary>
        /// Updates a user's password.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="newPassword">The new password.</param>
        public void UpdateUserPassword(int userId, string newPassword)
            => userRepository.ChangePassword(userId, newPassword);

        /// <summary>
        /// Updates a user's username.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="newUsername">The new username.</param>
        public void UpdateUserUsername(int userId, string newUsername)
            => userRepository.ChangeUsername(userId, newUsername);

        /// <summary>
        /// Attempts to log in a user by verifying credentials and creating a session.
        /// </summary>
        /// <param name="emailOrUsername">The email or username.</param>
        /// <param name="password">The password.</param>
        public User? Login(string emailOrUsername, string password)
        {
            var user = userRepository.VerifyCredentials(emailOrUsername);
            if (user != null && PasswordHasher.VerifyPassword(password, user.Password))
            {
                sessionService.CreateNewSession(user);
                userRepository.UpdateLastLogin(user.UserId);
                return user;
            }
            return null;
        }

        /// <summary>
        /// Ends the current user session.
        /// </summary>
        public void Logout()
            => sessionService.EndSession();

        /// <summary>
        /// Retrieves the currently logged-in user from session.
        /// </summary>
        public User GetCurrentUser()
            => sessionService.GetCurrentUser();

        /// <summary>
        /// Checks if a user is currently logged in.
        /// </summary>
        public bool IsUserLoggedIn()
            => sessionService.IsUserLoggedIn();

        /// <summary>
        /// Verifies the current user's password and updates their username.
        /// </summary>
        public bool UpdateUserUsername(string username, string currentPassword)
        {
            if (!VerifyUserPassword(currentPassword)) return false;
            userRepository.ChangeUsername(GetCurrentUser().UserId, username);
            return true;
        }

        /// <summary>
        /// Verifies the current user's password and updates their password.
        /// </summary>
        public bool UpdateUserPassword(string newPassword, string currentPassword)
        {
            if (!VerifyUserPassword(currentPassword)) return false;
            userRepository.ChangePassword(GetCurrentUser().UserId, newPassword);
            return true;
        }

        /// <summary>
        /// Verifies the current user's password and updates their email.
        /// </summary>
        public bool UpdateUserEmail(string newEmail, string currentPassword)
        {
            if (!VerifyUserPassword(currentPassword)) return false;
            userRepository.ChangeEmail(GetCurrentUser().UserId, newEmail);
            return true;
        }

        /// <summary>
        /// Verifies a user's password against the stored hash.
        /// </summary>
        public bool VerifyUserPassword(string password)
        {
            var currentEmail = GetCurrentUser().Email;
            var user = userRepository.VerifyCredentials(currentEmail);
            return user != null && PasswordHasher.VerifyPassword(password, user.Password);
        }

        /// <summary>
        /// Asynchronously retrieves all users via the repository proxy and maps to DTOs.
        /// </summary>
        public async Task<List<User>> GetAllUsersAsync()
        {
            var dto = await this.userRepository.GetUsersAsync();
            var result = new List<User>();
            foreach (var u in dto.Users)
            {
                result.Add(new User
                {
                    UserId = u.UserId,
                    Username = u.UserName,
                    Password = u.Password,
                    Email = u.Email,
                    WalletBalance = u.WalletBalance,
                    PointsBalance = u.PointsBalance,
                    UserRole = u.UserRole,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin,
                    ProfilePicture = u.ProfilePicture,
                });   
            }
            return result;
        }
    }
}