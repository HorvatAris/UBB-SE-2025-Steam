using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
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
        public bool? ActiveUserRating { get; set; }
    }
}