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
        List<User> GetAllUsers();

        User GetUserByIdentifier(int userIdentifier);

        User GetUserByEmail(string email);

        User GetUserByUsername(string username);

        void ValidateUserAndEmail(string email, string username);

        User CreateUser(User user);
        
        User UpdateUser(User user);

        void DeleteUser(int userIdentifier);

        bool AcceptChanges(int userIdentifier, string givenPassword);

        void UpdateUserEmail(int userIdentifier, string newEmail);

        void UpdateUserPassword(int userIdentifier, string newPassword);

        void UpdateUserUsername(int userIdentifier, string newUsername);

        Task<User?> LoginAsync(string emailOrUsername, string password);

        Task LogoutAsync();

        User GetCurrentUser();

        bool IsUserLoggedIn();

        bool UpdateUserUsername(string username, string currentPassword);

        bool UpdateUserPassword(string password, string currentPassword);

        bool UpdateUserEmail(string email, string currentPassword);

        bool VerifyUserPassword(string password);

        Task<List<User>> GetAllUsersAsync();
    }
}
