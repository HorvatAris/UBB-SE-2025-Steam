using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Models
{
    public class GroupedAchievementsResult
    {
        public List<AchievementWithStatus> AllAchievements { get; set; }
        public List<AchievementWithStatus> Friendships { get; set; }
        public List<AchievementWithStatus> OwnedGames { get; set; }
        public List<AchievementWithStatus> SoldGames { get; set; }
        public List<AchievementWithStatus> YearsOfActivity { get; set; }
        public List<AchievementWithStatus> NumberOfPosts { get; set; }
        public List<AchievementWithStatus> NumberOfReviewsGiven { get; set; }
        public List<AchievementWithStatus> NumberOfReviewsReceived { get; set; }
        public List<AchievementWithStatus> Developer { get; set; }
    }
}
