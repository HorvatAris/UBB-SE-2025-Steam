using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Entities
{
    public class AchievementUnlockedData
    {
        public string AchievementName { get; set; } = string.Empty;
        public string AchievementDescription { get; set; } = string.Empty;
        public DateTime? UnlockDate { get; set; }
    }
}
