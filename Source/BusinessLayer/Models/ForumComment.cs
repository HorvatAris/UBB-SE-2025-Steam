using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class ForumComment
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public int Score { get; set; }
        public DateTime TimeStamp { get; set; }
        public int AuthorId { get; set; }

        public int PostId { get; set; }
    }
}
