using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Entities
{
    public class UserLikedComment
    {
        public int UserId { get; set; }
        public int CommentId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Comment Comment { get; set; }
    }
}
