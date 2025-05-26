using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Utils;
using RoleEnum = SteamHub.Api.Entities.RoleEnum;

namespace SteamHub.Api.Context.Repositories
{
    /// <summary>
    /// Repository implementation for managing <see cref="SteamHub.ApiContract.Models.User.User"/> entities and mapping to DTOs.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly DataContext dataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="dataContext">The EF Core data context.</param>
        public UserRepository(DataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        /// <summary>
        /// Maps a <see cref="Entities.User"/> entity to a <see cref="SteamHub.ApiContract.Models.User.User"/> DTO.
        /// </summary>
        private static SteamHub.ApiContract.Models.User.User MapEntityToUserDto(Entities.User userEntity)
        {
            var user = new SteamHub.ApiContract.Models.User.User
            {
                UserId = userEntity.UserId,
                Username = userEntity.Username,
                Password = userEntity.Password,
                Email = userEntity.Email,
                WalletBalance = userEntity.WalletBalance,
                PointsBalance = userEntity.PointsBalance,
                UserRole = userEntity.UserRole.Id == RoleEnum.Developer ? UserRole.Developer : UserRole.User,
            };

            return user;
        }

        /// <summary>
        /// Applies values from a <see cref="SteamHub.ApiContract.Models.User.User"/> DTO to an <see cref="Entities.User"/> entity.
        /// </summary>
        private static void ApplyUserDtoToEntity(Entities.User userEntity, SteamHub.ApiContract.Models.User.User userDto)
        {
            userEntity.Username = userDto.Username;
            userEntity.Email = userDto.Email;
            userEntity.WalletBalance = userDto.WalletBalance;
            userEntity.PointsBalance = userDto.PointsBalance;
            userEntity.RoleId = (RoleEnum)userDto.UserRole;
        }

        /// <inheritdoc />
        public async Task<GetUsersResponse?> GetUsersAsync()
        {
            var userResponses = await dataContext.Users
                .AsNoTracking()
                .Select(userEntity => new UserResponse
                {
                    UserId = userEntity.UserId,
                    UserName = userEntity.Username,
                    Email = userEntity.Email,
                    Role = (ApiContract.Models.User.RoleEnum)userEntity.RoleId,
                    WalletBalance = userEntity.WalletBalance,
                    PointsBalance = userEntity.PointsBalance
                })
                .ToListAsync();

            return new GetUsersResponse { Users = userResponses };
        }

        /// <inheritdoc />
        public async Task<UserResponse?> GetUserByIdAsync(int userId)
        {
            var userResponse = await dataContext.Users
                .AsNoTracking()
                .Where(userEntity => userEntity.UserId == userId)
                .Select(userEntity => new UserResponse
                {
                    UserId = userEntity.UserId,
                    UserName = userEntity.Username,
                    Email = userEntity.Email,
                    Role = (ApiContract.Models.User.RoleEnum)userEntity.RoleId,
                    WalletBalance = userEntity.WalletBalance,
                    PointsBalance = userEntity.PointsBalance
                })
                .SingleOrDefaultAsync();

            return userResponse;
        }

        /// <inheritdoc />
        public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
        {
            var newUserEntity = new Entities.User
            {
                Username = request.UserName,
                Email = request.Email,
                RoleId = (Entities.RoleEnum)request.Role,
                WalletBalance = request.WalletBalance,
                PointsBalance = request.PointsBalance
            };

            await dataContext.Users.AddAsync(newUserEntity);
            await dataContext.SaveChangesAsync();

            return new CreateUserResponse { UserId = newUserEntity.UserId };
        }

        /// <inheritdoc />
        public async Task UpdateUserAsync(int userId, UpdateUserRequest request)
        {
            var existingUserEntity = await dataContext.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            existingUserEntity.Username = request.UserName;
            existingUserEntity.Email = request.Email;
            existingUserEntity.RoleId = (Entities.RoleEnum)request.Role;
            existingUserEntity.WalletBalance = request.WalletBalance;
            existingUserEntity.PointsBalance = request.PointsBalance;

            await dataContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public List<SteamHub.ApiContract.Models.User.User> GetAllUsers()
        {
            return dataContext.Users
                .AsNoTracking()
                .OrderBy(userEntity => userEntity.Username)
                .Select(MapEntityToUserDto)
                .ToList();
        }

        /// <inheritdoc />
        public SteamHub.ApiContract.Models.User.User? GetUserById(int userId)
        {
            var userEntity = dataContext.Users.Find(userId);
            return userEntity == null ? null : MapEntityToUserDto(userEntity);
        }

        /// <inheritdoc />
        public SteamHub.ApiContract.Models.User.User UpdateUser(SteamHub.ApiContract.Models.User.User userDto)
        {
            var existingUserEntity = dataContext.Users.Find(userDto.UserId)
                ?? throw new KeyNotFoundException($"User {userDto.UserId} not found.");

            ApplyUserDtoToEntity(existingUserEntity, userDto);
            dataContext.SaveChanges();

            return MapEntityToUserDto(existingUserEntity);
        }

        /// <inheritdoc />
        public SteamHub.ApiContract.Models.User.User CreateUser(SteamHub.ApiContract.Models.User.User userDto)
        {
            var newUserEntity = new Entities.User();
            ApplyUserDtoToEntity(newUserEntity, userDto);

            dataContext.Users.Add(newUserEntity);
            dataContext.SaveChanges();

            return MapEntityToUserDto(newUserEntity);
        }

        /// <inheritdoc />
        public void DeleteUser(int userId)
        {
            var userEntity = dataContext.Users.Find(userId);
            if (userEntity != null)
            {
                dataContext.Users.Remove(userEntity);
                dataContext.SaveChanges();
            }
        }

        /// <inheritdoc />
        public SteamHub.ApiContract.Models.User.User? VerifyCredentials(string emailOrUsername)
        {
            var userEntity = dataContext.Users.SingleOrDefault(
                user => user.Username == emailOrUsername || user.Email == emailOrUsername);

            return userEntity == null ? null : MapEntityToUserDto(userEntity);
        }

        /// <inheritdoc />
        public SteamHub.ApiContract.Models.User.User? GetUserByEmail(string email)
        {
            var userEntity = dataContext.Users.SingleOrDefault(user => user.Email == email);
            return userEntity == null ? null : MapEntityToUserDto(userEntity);
        }

        /// <inheritdoc />
        public SteamHub.ApiContract.Models.User.User? GetUserByUsername(string username)
        {
            var userEntity = dataContext.Users.SingleOrDefault(user => user.Username == username);
            return userEntity == null ? null : MapEntityToUserDto(userEntity);
        }

        /// <inheritdoc />
        public string CheckUserExists(string email, string username)
        {
            if (dataContext.Users.Any(user => user.Email == email))
            {
                return "EMAIL_EXISTS";
            }

            if (dataContext.Users.Any(user => user.Username == username))
            {
                return "USERNAME_EXISTS";
            }

            return null;
        }

        /// <inheritdoc />
        public void ChangeEmail(int userId, string newEmail)
        {
            var userEntity = dataContext.Users.Find(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            userEntity.Email = newEmail;
            dataContext.SaveChanges();
        }

        /// <inheritdoc />
        public void ChangePassword(int userId, string newPassword)
        {
            var userEntity = dataContext.Users.Find(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            userEntity.Password = PasswordHasher.HashPassword(newPassword);
            dataContext.SaveChanges();
        }

        /// <inheritdoc />
        public void ChangeUsername(int userId, string newUsername)
        {
            var userEntity = dataContext.Users.Find(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            userEntity.Username = newUsername;
            dataContext.SaveChanges();
        }

        /// <inheritdoc />
        public void UpdateLastLogin(int userId)
        {
            var userEntity = dataContext.Users.Find(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            userEntity.LastLogin = DateTime.UtcNow;
            dataContext.SaveChanges();
        }
    }
}