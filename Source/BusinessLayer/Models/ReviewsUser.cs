using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class ReviewsUser
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public byte[]? ProfilePicture { get; set; }

        // Navigation property
        public ICollection<Review> Reviews { get; set; }
    }
}
