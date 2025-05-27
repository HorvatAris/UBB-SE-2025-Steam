using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Models
{
    public class UserLikedComment
    {
        public int UserId { get; set; }
        public int CommentId { get; set; }
    }
}
