using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Entities
{
    public class FeatureUser
    {
        public int UserId { get; set; }
        public int FeatureId { get; set; }

        public bool Equipped { get; set; } = false;

        // Navigation properties
        public User User { get; set; }
        public Feature Feature { get; set; }
    }
}
