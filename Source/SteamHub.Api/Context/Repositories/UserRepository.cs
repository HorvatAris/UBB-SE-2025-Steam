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
                WalletBalance = (float)userEntity.WalletBalance,
                PointsBalance = userEntity.PointsBalance,
                
                // This for later
                //UserAchievements = userEntity.UserAchievements.Select(ua => new UserAchievement
                //{
                //    UserId = ua.UserId,
                //    AchievementId = ua.AchievementId,
                //    UnlockedAt = ua.UnlockedAt
                //}).ToList(),
                //SoldGames = userEntity.SoldGames.Select(sg => new SoldGame
                //{
                //    SoldGameId = sg.SoldGameId,
                //    GameId = sg.GameId,
                //    UserId = sg.UserId,
                //    SoldDate = sg.SoldDate,
                //}).ToList()
            };

            return user;
        }

        private static void ApplyUserDtoToEntity(UserDTO userEntity, User userDto)
        {
            userEntity.Username = userDto.Username;
            userEntity.Email = userDto.Email;
            userEntity.UserRole = userDto.UserRole;
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
                    WalletBalance = (float)userEntity.WalletBalance,
                    PointsBalance = userEntity.PointsBalance,
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
            return dataContext.Users
                .AsNoTracking()
                .OrderBy(userEntity => userEntity.Username)
                .Select(MapEntityToUserDto)
                .ToList();
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
            var userEntity = await dataContext.Users.SingleOrDefaultAsync(
                user => user.Username == emailOrUsername || user.Email == emailOrUsername);

            return userEntity == null ? null : MapEntityToUserDto(userEntity);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var userEntity = await dataContext.Users.SingleOrDefaultAsync(user => user.Email == email);
            return userEntity == null ? null : MapEntityToUserDto(userEntity);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var userEntity = await dataContext.Users.SingleOrDefaultAsync(user => user.Username == username);
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

        // From UserProfile, WORK IN PROGRESS
        public async Task UpdateProfileBioAsync(int userId, string bio)
        {
            var existing = await dataContext.Users.SingleOrDefaultAsync(user => user.UserId == userId)
                ?? throw new Exception($"Profile with user ID {userId} not found.");
            existing.Bio = bio;
            existing.LastModified = DateTime.UtcNow;
            await dataContext.SaveChangesAsync();
        }

        public async Task UpdateProfilePictureAsync(int userId, string localImagePath)
        {
            string imgurClientId = "bbf48913b385d7b";
            var existing = await dataContext.Users.SingleOrDefaultAsync(up => up.UserId == userId)
                ?? throw new Exception($"Profile with user ID {userId} not found.");

            string imageUrl = await UploadImageToImgurAsync(localImagePath, imgurClientId);

            existing.ProfilePicture = imageUrl;
            existing.LastModified = DateTime.UtcNow;
            await dataContext.SaveChangesAsync();
        }

        private async Task<string> UploadImageToImgurAsync(string imagePath, string clientId)
        {
            using var client = new HttpClient();
            using var form = new MultipartFormDataContent();
            using var image = new ByteArrayContent(await File.ReadAllBytesAsync(imagePath));

            image.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
            form.Add(image, "image");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Client-ID", clientId);

            var response = await client.PostAsync("https://api.imgur.com/3/image", form);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Imgur upload failed: " + json);
            }

            var link = System.Text.Json.JsonDocument.Parse(json)
                        .RootElement.GetProperty("data")
                        .GetProperty("link").GetString();

            return link ?? throw new Exception("Imgur returned null link.");
        }
    }
}