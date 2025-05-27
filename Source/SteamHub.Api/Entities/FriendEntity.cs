using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Entities
{
    public class FriendEntity
    {
        public int FriendshipId { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public User User1 { get; set; }
        public User User2 { get; set; }
    }
}
