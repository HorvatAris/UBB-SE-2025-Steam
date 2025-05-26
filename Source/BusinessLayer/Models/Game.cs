using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
   public class Game
    {
        public int GameIdentifier { get; set;  }
        public string TitleOfGame { get; set;  }
        public string DescriptionOfGame { get; set;  }
        public string CoverImagePathOrUrl { get; set;  }
        public DateTime DateWhenGameWasReleased { get; set;  }

        // optianal display helpers
        public int RetrieveGameId() => GameIdentifier;
        public string RetrieveGameTitle() => TitleOfGame;
    }
}
