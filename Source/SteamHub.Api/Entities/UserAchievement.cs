using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Entities
{
    public class UserAchievement
    {
        public int UserId { get; set; }
        public int AchievementId { get; set; }
        public DateTime UnlockedAt { get; set; }

        public User User { get; set; }
        public Achievement Achievement { get; set; }
    }
}
