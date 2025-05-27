using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Content { get; set; }
        public DateTime UploadDate { get; set; }
        public int NrLikes { get; set; }
        public int NrDislikes { get; set; }
        public int NrComments { get; set; }

        // Navigation properties
        public User Author { get; set; }
        public ICollection<Comment> Comments { get; set; }

        // Not mapped property
        public bool? ActiveUserRating { get; set; }
    }
}