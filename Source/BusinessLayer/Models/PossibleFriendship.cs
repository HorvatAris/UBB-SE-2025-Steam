using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class PossibleFriendship
    {
        public User User { get; set; }
        public bool IsFriend { get; set; }
        public Friendship Friendship { get; set; }
    }
}
