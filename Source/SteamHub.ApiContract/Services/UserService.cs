﻿using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.Session;
using SteamHub.ApiContract.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models.Common;

namespace SteamHub.ApiContract.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IWalletRepository walletRepository;
        private readonly ISessionRepository sessionRepository;

        public UserService(IUserRepository userRepository, ISessionRepository sessionRepository, IWalletRepository walletRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
            this.walletRepository = walletRepository;
        }

        public async Task<User> GetUserByIdentifierAsync(int userId)
        {
            return await userRepository.GetUserByIdAsync(userId);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await userRepository.GetUserByEmailAsync(email);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await userRepository.GetUserByUsernameAsync(username);
        }

        public async Task ValidateUserAndEmailAsync(string email, string username)
        {
            var errorType = await userRepository.CheckUserExistsAsync(email, username);
            if (errorType == "EMAIL_EXISTS")
            {
                throw new EmailAlreadyExistsException(email);
            }

            if (errorType == "USERNAME_EXISTS")
            {
                throw new UsernameAlreadyTakenException(username);
            }
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await ValidateUserAndEmailAsync(user.Email, user.Username);
            
            // Ensure required fields are set
            user.Password = PasswordHasher.HashPassword(user.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.LastModified = DateTime.UtcNow;
            user.UserRole = user.UserRole;
                        
            var createdUser = await userRepository.CreateUserAsync(user);
            await walletRepository.AddNewWallet(createdUser.UserId);

            return createdUser;
        }

        public async Task<User> UpdateUserAsync(User user)
            => await userRepository.UpdateUserAsync(user);

        public async Task DeleteUserAsync(int userId)
        {
            await sessionRepository.DeleteUserSessions(userId);
            await userRepository.DeleteUserAsync(userId);
        }

        public async Task<bool> AcceptChangesAsync(int userId, string givenPassword)
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            return PasswordHasher.VerifyPassword(givenPassword, user.Password);
        }

        public async Task UpdateUserEmailAsync(int userId, string newEmail)
        {
            await userRepository.ChangeEmailAsync(userId, newEmail);
        }

        public async Task UpdateUserPasswordAsync(int userId, string newPassword)
        {
            await userRepository.ChangePasswordAsync(userId, newPassword);
        }

        public async Task UpdateUserUsernameAsync(int userId, string newUsername)
        {
            await userRepository.ChangeUsernameAsync(userId, newUsername);
        }

        public async Task<User?> LoginAsync(string emailOrUsername, string password)
        {
            var user = await userRepository.VerifyCredentialsAsync(emailOrUsername);
            if (user != null && PasswordHasher.VerifyPassword(password, user.Password))
            {
                var sessionDetails = await sessionRepository.CreateSession(user.UserId);
                await userRepository.UpdateLastLoginAsync(user.UserId);
                return user;
            }
            return null;
        }

        public async Task LogoutAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser != null)
            {
                await sessionRepository.DeleteUserSessions(currentUser.UserId);
            }
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            var sessionId = UserSession.Instance.CurrentSessionId;
            if (!sessionId.HasValue)
                return null;

            var userWithSession = await sessionRepository.GetUserFromSession(sessionId.Value);
            if (userWithSession == null)
                return null;

            return new User
            {
                UserId = userWithSession.UserId,
                Username = userWithSession.Username,
                Email = userWithSession.Email,
                UserRole = userWithSession.Developer ? UserRole.Developer : UserRole.User
            };
        }

        public async Task<bool> IsUserLoggedInAsync()
        {
            var sessionId = UserSession.Instance.CurrentSessionId;
            if (!sessionId.HasValue)
                return false;

            var session = await sessionRepository.GetSessionById(sessionId.Value);
            return session != null && session.ExpiresAt > DateTime.UtcNow;
        }

        public async Task<bool> UpdateUserUsernameAsync(string username, string currentPassword)
        {
            if (!await VerifyUserPasswordAsync(currentPassword)) 
                return false;
            var currentUser = await GetCurrentUserAsync();
            await UpdateUserUsernameAsync(currentUser.UserId, username);
            return true;
        }

        public async Task<bool> UpdateUserPasswordAsync(string newPassword, string currentPassword)
        {
            if (!await VerifyUserPasswordAsync(currentPassword)) 
                return false;
            var currentUser = await GetCurrentUserAsync();
            await UpdateUserPasswordAsync(currentUser.UserId, newPassword);
            return true;
        }

        public async Task<bool> UpdateUserEmailAsync(string newEmail, string currentPassword)
        {
            if (!await VerifyUserPasswordAsync(currentPassword)) 
                return false;
            var currentUser = await GetCurrentUserAsync();
            await UpdateUserEmailAsync(currentUser.UserId, newEmail);
            return true;
        }

        public async Task<bool> VerifyUserPasswordAsync(string password)
        {
            var currentUser = await GetCurrentUserAsync();
            var user = await userRepository.VerifyCredentialsAsync(currentUser.Email);
            return user != null && PasswordHasher.VerifyPassword(password, user.Password);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await userRepository.GetAllUsersAsync();
        }

        public async Task<bool> UpdateProfilePictureAsync(int userId, string profilePicturePath)
        {
            var currentUser = await GetCurrentUserAsync();
            await userRepository.UpdateProfilePictureAsync(currentUser.UserId, profilePicturePath);
            return true;
        }

        public async Task<bool> UpdateProfileBioAsync(int userId, string profileBio)
        {
            var currentUser = await GetCurrentUserAsync();
            await userRepository.UpdateProfileBioAsync(currentUser.UserId, profileBio);
            return true;
        }
    }
}