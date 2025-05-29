﻿using BusinessLayer.Models;
using SteamHub.ApiContract.Models.Collections;
using SteamHub.ApiContract.Models;

namespace SteamHub.Web.ViewModels
{
    public class ProfileViewModel
    {
        public int UserIdentifier { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ProfilePhotoPath { get; set; }
        public string Biography { get; set; }
        public int FriendCount { get; set; }
        public decimal MoneyBalance { get; set; }
        public int PointsBalance { get; set; }
        public List<Collection> GameCollections { get; set; } = new();
        public bool IsFriend { get; set; }
        public string FriendButtonText { get; set; }
        public string ErrorMessage { get; set; }
        public AchievementWithStatus FriendshipsAchievement { get; set; }
        public AchievementWithStatus OwnedGamesAchievement { get; set; }
        public AchievementWithStatus SoldGamesAchievement { get; set; }
        public AchievementWithStatus NumberOfReviewsAchievement { get; set; }
        public AchievementWithStatus NumberOfReviewsReceived { get; set; }
        public AchievementWithStatus DeveloperAchievement { get; set; }
        public AchievementWithStatus YearsOfActivity { get; set; }
        public AchievementWithStatus NumberOfPostsGetTopAchievement { get; set; }
        // Add more achievement properties as needed
        public bool IsProfileOwner { get; set; }
        public bool IsDeveloper { get; set; }
        // Equipped features
        public string EquippedFrameSource { get; set; }
        public string EquippedHatSource { get; set; }
        public string EquippedPetSource { get; set; }
        public string EquippedEmojiSource { get; set; }
        public string EquippedBackgroundSource { get; set; }
        public bool HasEquippedFrame { get; set; }
        public bool HasEquippedHat { get; set; }
        public bool HasEquippedPet { get; set; }
        public bool HasEquippedEmoji { get; set; }
        public bool HasEquippedBackground { get; set; }
    }
}
