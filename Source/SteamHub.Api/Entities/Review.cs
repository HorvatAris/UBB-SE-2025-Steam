using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace SteamHub.Api.Entities
{
    public class Review
    {
        public int ReviewIdentifier { get; set; }
        public string ReviewTitleText { get; set; } = string.Empty;
        public string ReviewContentText { get; set; } = string.Empty;
        public bool IsRecommended { get; set; }
        public double NumericRatingGivenByUser { get; set; }
        public int TotalHelpfulVotesReceived { get; set; }
        public int TotalFunnyVotesReceived { get; set; }
        public int TotalHoursPlayedByReviewer { get; set; }
        public DateTime DateAndTimeWhenReviewWasCreated { get; set; }
        public int UserIdentifier { get; set; }
        public int GameIdentifier { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Game Game { get; set; }
    }
}
