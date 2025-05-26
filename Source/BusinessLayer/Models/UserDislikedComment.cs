using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class UserDislikedComment
    {
        public int UserId { get; set; }
        public int CommentId { get; set; }
    }
}
