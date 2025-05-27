using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Entities
{
    public class ForumPost
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public int Score { get; set; }
        public DateTime TimeStamp { get; set; }
        public int AuthorId { get; set; }
        public int? GameId { get; set; }

        // Navigation properties
        public User Author { get; set; }
        public Game? Game { get; set; }
        public ICollection<ForumComment> Comments { get; set; }
    }
}
