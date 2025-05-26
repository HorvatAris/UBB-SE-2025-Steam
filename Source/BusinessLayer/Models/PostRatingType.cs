using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class PostRatingType
    {
        public int PostId { get; set; }

        public int AuthorId { get; set; }

        public bool RatingType { get; set; }

        public const bool LIKE = true;
        public const bool DISLIKE = false;
    }
}
