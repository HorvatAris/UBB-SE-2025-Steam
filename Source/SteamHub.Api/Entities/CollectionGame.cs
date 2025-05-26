﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Entities
{
    public class CollectionGame
    {
        public int CollectionId { get; set; }

        public int GameId { get; set; }

        public Collection Collection { get; set; }
        public OwnedGame OwnedGame { get; set; }
    }
}
