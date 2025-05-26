using System;
using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;
using BusinessLayer.DataContext;

namespace BusinessLayer.Repositories
{
    public class UserProfilesRepository : IUserProfilesRepository
    {
        private readonly ApplicationDbContext context;

        public UserProfilesRepository(ApplicationDbContext newContext)
        {
            this.context = newContext ?? throw new ArgumentNullException(nameof(context));
        }

        public UserProfile? GetUserProfileByUserId(int userId)
        {
            return context.UserProfiles.SingleOrDefault(up => up.UserId == userId);
        }

        public UserProfile? UpdateProfile(UserProfile profile)
        {
            var existing = context.UserProfiles.Find(profile.ProfileId)
                ?? throw new RepositoryException($"Profile with ID {profile.ProfileId} not found.");
            existing.ProfilePicture = profile.ProfilePicture;
            existing.Bio = profile.Bio;
            existing.LastModified = DateTime.UtcNow;
            context.SaveChanges();
            return existing;
        }

        public UserProfile? CreateProfile(int userId)
        {
            var profile = new UserProfile { UserId = userId };
            context.UserProfiles.Add(profile);
            context.SaveChanges();
            return profile;
        }

        public void UpdateProfileBio(int userId, string bio)
        {
            var existing = context.UserProfiles.SingleOrDefault(up => up.UserId == userId)
                ?? throw new RepositoryException($"Profile with user ID {userId} not found.");
            existing.Bio = bio;
            existing.LastModified = DateTime.UtcNow;
            context.SaveChanges();
        }

        public async Task UpdateProfilePicture(int userId, string localImagePath)
        {
            string imgurClientId = "bbf48913b385d7b";
            var existing = context.UserProfiles.SingleOrDefault(up => up.UserId == userId)
                ?? throw new RepositoryException($"Profile with user ID {userId} not found.");

            string imageUrl = await UploadImageToImgurAsync(localImagePath, imgurClientId);

            existing.ProfilePicture = imageUrl;
            existing.LastModified = DateTime.UtcNow;
            context.SaveChanges();
        }

        private async Task<string> UploadImageToImgurAsync(string imagePath, string clientId)
        {
            using var client = new HttpClient();
            using var form = new MultipartFormDataContent();
            using var image = new ByteArrayContent(File.ReadAllBytes(imagePath));

            image.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
            form.Add(image, "image");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Client-ID", clientId);

            var response = await client.PostAsync("https://api.imgur.com/3/image", form);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new RepositoryException("Imgur upload failed: " + json);
            }

            var link = System.Text.Json.JsonDocument.Parse(json)
                        .RootElement.GetProperty("data")
                        .GetProperty("link").GetString();

            return link ?? throw new RepositoryException("Imgur returned null link.");
        }
    }
}
