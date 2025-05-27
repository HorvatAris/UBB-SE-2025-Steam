using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Models
{
    public class UserLikedPost
    {
        public int UserId { get; set; }
        public int PostId { get; set; }
    }
}
