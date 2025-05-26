using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class FriendEntity
    {
        public int FriendshipId { get; set; }
        public string User1Username { get; set; }
        public string User2Username { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
