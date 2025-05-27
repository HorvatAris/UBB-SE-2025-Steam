using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Models
{
    public class ChatMessageUI : ChatMessage
    {
        public string SenderUsername { get; set; }
        public string Time { get; set; }
        public string Aligment { get; set; }
    }
}