using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class AchievementWithStatus
    {
        public Achievement Achievement { get; set; }
        public bool IsUnlocked { get; set; }
        public double Opacity => IsUnlocked ? 1.0 : 0.5;

        public DateTime? UnlockedDate { get; set; }
    }
}
