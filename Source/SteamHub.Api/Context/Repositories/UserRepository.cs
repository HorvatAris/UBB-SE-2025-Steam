using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Utils;
using SteamHub.ApiContract.Models.User;
using UserAchievement = SteamHub.ApiContract.Models.UserAchievement;
using SoldGame = SteamHub.ApiContract.Models.SoldGame;
using User = SteamHub.ApiContract.Models.User.User;
using UserDTO = SteamHub.Api.Entities.User;

namespace SteamHub.Api.Context.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext dataContext;

        public UserRepository(DataContext dataContext)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        private static User MapEntityToUserDto(UserDTO userEntity)
        {
            var user = new User
            {
                UserId = userEntity.UserId,
                Username = userEntity.Username,
                Password = userEntity.Password,
                Email = userEntity.Email,
                UserRole = userEntity.UserRole,
                CreatedAt = userEntity.CreatedAt,
                LastLogin = userEntity.LastLogin,
                ProfilePicture = userEntity.ProfilePicture,
                Bio = userEntity.Bio ?? string.Empty,
                LastModified = userEntity.LastModified,
                WalletBalance = userEntity.WalletBalance,
                PointsBalance = userEntity.PointsBalance
            };

            return user;
        }

        private static void ApplyUserDtoToEntity(UserDTO userDTO, User userEntity)
        {
            userDTO.Username = userEntity.Username;
            userDTO.Email = userEntity.Email;
            userDTO.UserRole = userEntity.UserRole;
            userDTO.Password = userEntity.Password;
            userDTO.ProfilePicture = userEntity.ProfilePicture;
            userDTO.Bio = string.Empty;
            userDTO.CreatedAt = userEntity.CreatedAt;
            userDTO.PointsBalance = userEntity.PointsBalance;
            userDTO.WalletBalance = userEntity.WalletBalance;
        }

        public async Task<GetUsersResponse?> GetUsersAsync()
        {
            var userResponses = await dataContext.Users
                .AsNoTracking()
                .Select(userEntity => new UserResponse
                {
                    UserId = userEntity.UserId,
                    UserName = userEntity.Username,
                    Password = userEntity.Password,
                    Email = userEntity.Email,
                    UserRole = userEntity.UserRole,
                    CreatedAt = userEntity.CreatedAt,
                    LastLogin = userEntity.LastLogin,
                    ProfilePicture = userEntity.ProfilePicture,
                    Bio = userEntity.Bio ?? string.Empty,
                    LastChanged = userEntity.LastModified,
                    WalletBalance = userEntity.WalletBalance,
                    PointsBalance = userEntity.PointsBalance
                })
                .ToListAsync();

            return new GetUsersResponse { Users = userResponses };
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            var userEntity = await dataContext.Users
                .AsNoTracking()
                .Where(userEntity => userEntity.UserId == userId)
                .SingleOrDefaultAsync();

            return userEntity == null ? null : MapEntityToUserDto(userEntity);
        }

        public async Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request)
        {
            var newUserEntity = new UserDTO
            {
                Username = request.UserName,
                Email = request.Email,
                Password = PasswordHasher.HashPassword(request.Password),
                UserRole = request.UserRole,
                ProfilePicture = request.ProfilePicture,
                CreatedAt = DateTime.UtcNow
            };

            await dataContext.Users.AddAsync(newUserEntity);
            await dataContext.SaveChangesAsync();

            return new CreateUserResponse { UserId = newUserEntity.UserId };
        }

        public async Task UpdateUserAsync(int userId, UpdateUserRequest request)
        {
            var existingUserEntity = await dataContext.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");

            existingUserEntity.Username = request.UserName;
            existingUserEntity.Email = request.Email;
            existingUserEntity.UserRole = request.UserRole;
            existingUserEntity.WalletBalance = request.WalletBalance;
            existingUserEntity.PointsBalance = (int)request.PointsBalance;

            await dataContext.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = dataContext.Users
                .AsNoTracking()
                .OrderBy(userEntity => userEntity.Username)
                .Select(MapEntityToUserDto)
                .ToList();
            
            return await Task.FromResult(users);
        }

        public async Task<User> UpdateUserAsync(User userDto)
        {
            var existingUserEntity = await dataContext.Users.FindAsync(userDto.UserId)
                ?? throw new KeyNotFoundException($"User {userDto.UserId} not found.");

            ApplyUserDtoToEntity(existingUserEntity, userDto);
            await dataContext.SaveChangesAsync();

            return MapEntityToUserDto(existingUserEntity);
        }

        public async Task<User> CreateUserAsync(User userDto)
        {
            var newUserEntity = new UserDTO();
            ApplyUserDtoToEntity(newUserEntity, userDto);

            await dataContext.Users.AddAsync(newUserEntity);
            await dataContext.SaveChangesAsync();

            return MapEntityToUserDto(newUserEntity);
        }

        public async Task DeleteUserAsync(int userId)
        {
            var userEntity = await dataContext.Users.FindAsync(userId);
            if (userEntity != null)
            {
                dataContext.Users.Remove(userEntity);
                await dataContext.SaveChangesAsync();
            }
        }

        public async Task<User?> VerifyCredentialsAsync(string emailOrUsername)
        {
            var userEntity = await dataContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(user => user.Username == emailOrUsername || user.Email == emailOrUsername);

            return userEntity == null ? null : MapEntityToUserDto(userEntity);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var userEntity = await dataContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(user => user.Email == email);
            return userEntity == null ? null : MapEntityToUserDto(userEntity);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var userEntity = await dataContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(user => user.Username == username);
            return userEntity == null ? null : MapEntityToUserDto(userEntity);
        }

        public async Task<string> CheckUserExistsAsync(string email, string username)
        {
            if (await dataContext.Users.AnyAsync(user => user.Email == email))
            {
                return "EMAIL_EXISTS";
            }

            if (await dataContext.Users.AnyAsync(user => user.Username == username))
            {
                return "USERNAME_EXISTS";
            }

            return null;
        }

        public async Task ChangeEmailAsync(int userId, string newEmail)
        {
            var userEntity = await dataContext.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");
            userEntity.Email = newEmail;
            await dataContext.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(int userId, string newPassword)
        {
            var userEntity = await dataContext.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");
            userEntity.Password = PasswordHasher.HashPassword(newPassword);
            await dataContext.SaveChangesAsync();
        }

        public async Task ChangeUsernameAsync(int userId, string newUsername)
        {
            var userEntity = await dataContext.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");
            userEntity.Username = newUsername;
            await dataContext.SaveChangesAsync();
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var userEntity = await dataContext.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");
            userEntity.LastLogin = DateTime.UtcNow;
            await dataContext.SaveChangesAsync();
        }

        public async Task UpdateProfileBioAsync(int userId, string bio)
        {
            var userEntity = await dataContext.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");
            userEntity.Bio = bio;
            await dataContext.SaveChangesAsync();
        }

        public async Task UpdateProfilePictureAsync(int userId, string picturePath)
        {
            var userEntity = await dataContext.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException($"User {userId} not found.");
            userEntity.ProfilePicture = picturePath;
            await dataContext.SaveChangesAsync();
        }
    }
}