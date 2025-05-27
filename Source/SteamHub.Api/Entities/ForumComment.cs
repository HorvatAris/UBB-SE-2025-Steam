using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Entities
{
    public class ForumComment
    {
        public int Id { get; set; }
        public string Body { get; set; } = string.Empty;
        public int Score { get; set; }
        public DateTime TimeStamp { get; set; }
        public int AuthorId { get; set; }
        public int PostId { get; set; }

        // Navigation properties
        public User Author { get; set; }
        public ForumPost Post { get; set; }
    }
}
