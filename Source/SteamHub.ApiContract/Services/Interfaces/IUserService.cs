// <copyright file="IUserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.User;

    public interface IUserService
    {
        Task<User> GetUserByIdentifierAsync(int userIdentifier);

        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserByUsernameAsync(string username);

        Task ValidateUserAndEmailAsync(string email, string username);

        Task<User> CreateUserAsync(User user);
        
        Task<User> UpdateUserAsync(User user);

        Task DeleteUserAsync(int userIdentifier);

        Task<bool> AcceptChangesAsync(int userIdentifier, string givenPassword);

        Task UpdateUserEmailAsync(int userIdentifier, string newEmail);

        Task UpdateUserPasswordAsync(int userIdentifier, string newPassword);

        Task UpdateUserUsernameAsync(int userIdentifier, string newUsername);

        Task<User?> LoginAsync(string emailOrUsername, string password);

        Task LogoutAsync();

        Task<User?> GetCurrentUserAsync();

        Task<bool> IsUserLoggedInAsync();

        Task<bool> UpdateUserUsernameAsync(string username, string currentPassword);

        Task<bool> UpdateUserPasswordAsync(string password, string currentPassword);

        Task<bool> UpdateUserEmailAsync(string email, string currentPassword);

        Task<bool> VerifyUserPasswordAsync(string password);

        Task<List<User>> GetAllUsersAsync();

        Task UpdateProfilePictureAsync(int userId, string profilePicturePath);
        Task UpdateProfileBioAsync(int userId, string profileBio);
    }
}
