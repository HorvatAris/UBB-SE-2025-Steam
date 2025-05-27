using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Entities
{
    public class Wallet
    {
        public int WalletId { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public int Points { get; set; }

        public User User { get; set; }
    }
}
