using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Entities
{
    public class PostRatingType
    {
        public int PostId { get; set; }

        public int AuthorId { get; set; }

        public bool RatingType { get; set; }

        // Navigation properties
        public Post Post { get; set; }
        public User Author { get; set; }

        public const bool LIKE = true;
        public const bool DISLIKE = false;
    }
}
