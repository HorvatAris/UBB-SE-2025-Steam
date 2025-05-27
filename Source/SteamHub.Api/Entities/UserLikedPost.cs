using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Entities
{
    public class UserLikedPost
    {
        public int UserId { get; set; }
        public int PostId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Post Post { get; set; }
    }
}
