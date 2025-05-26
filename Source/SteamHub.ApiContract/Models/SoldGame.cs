using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Models
{
    public class SoldGame
    {
        public int SoldGameId { get; set; }
        public int UserId { get; set; }
        public int? GameId { get; set; }
        public DateTime? SoldDate { get; set; }

        public SteamHub.ApiContract.Models.User.User User { get; set; }
    }
}
