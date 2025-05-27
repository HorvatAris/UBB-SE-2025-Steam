// <copyright file="IUserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.User;

    /// <summary>
    /// Defines user management operations including CRUD, authentication, and session checks.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieves all users synchronously.
        /// </summary>
        List<User> GetAllUsers();

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userIdentifier">The user ID.</param>
        User GetUserByIdentifier(int userIdentifier);

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The user's email.</param>
        User GetUserByEmail(string email);

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The user's username.</param>
        User GetUserByUsername(string username);

        /// <summary>
        /// Validates that a new email and username are not already in use.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <param name="username">The username to check.</param>
        /// <exception cref="Exceptions.EmailAlreadyExistsException" />
        /// <exception cref="Exceptions.UsernameAlreadyTakenException" />
        void ValidateUserAndEmail(string email, string username);

        /// <summary>
        /// Creates a new user record.
        /// </summary>
        /// <param name="user">The user data to create.</param>
        User CreateUser(User user);

        /// <summary>
        /// Updates an existing user record.
        /// </summary>
        /// <param name="user">The user data to update.</param>
        User UpdateUser(User user);

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="userIdentifier">The user ID to delete.</param>
        void DeleteUser(int userIdentifier);

        /// <summary>
        /// Verifies the current password for a user before applying changes.
        /// </summary>
        /// <param name="userIdentifier">The user ID.</param>
        /// <param name="givenPassword">The plain-text password to verify.</param>
        bool AcceptChanges(int userIdentifier, string givenPassword);

        /// <summary>
        /// Updates a user's email.
        /// </summary>
        void UpdateUserEmail(int userIdentifier, string newEmail);

        /// <summary>
        /// Updates a user's password.
        /// </summary>
        void UpdateUserPassword(int userIdentifier, string newPassword);

        /// <summary>
        /// Updates a user's username.
        /// </summary>
        void UpdateUserUsername(int userIdentifier, string newUsername);

        /// <summary>
        /// Attempts to authenticate a user and start a session.
        /// </summary>
        /// <param name="emailOrUsername">Email or username.</param>
        /// <param name="password">Password.</param>
        User? Login(string emailOrUsername, string password);

        /// <summary>
        /// Ends the current user session.
        /// </summary>
        void Logout();

        /// <summary>
        /// Retrieves the currently logged-in user.
        /// </summary>
        User GetCurrentUser();

        /// <summary>
        /// Checks if a user session is active.
        /// </summary>
        bool IsUserLoggedIn();

        /// <summary>
        /// Verifies current password and updates username.
        /// </summary>
        bool UpdateUserUsername(string username, string currentPassword);

        /// <summary>
        /// Verifies current password and updates password.
        /// </summary>
        bool UpdateUserPassword(string password, string currentPassword);

        /// <summary>
        /// Verifies current password and updates email.
        /// </summary>
        bool UpdateUserEmail(string email, string currentPassword);

        /// <summary>
        /// Verifies a user's password.
        /// </summary>
        bool VerifyUserPassword(string password);

        /// <summary>
        /// Asynchronously retrieves all users.
        /// </summary>
        Task<List<User>> GetAllUsersAsync();
    }
}
