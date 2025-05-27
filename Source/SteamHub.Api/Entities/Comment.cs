using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Entities
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public int AuthorId { get; set; }
        public string Content { get; set; }
        public DateTime CommentDate { get; set; }
        public int NrLikes { get; set; }
        public int NrDislikes { get; set; }

        // Navigation properties
        public Post Post { get; set; }
        public User Author { get; set; }
    }
}
