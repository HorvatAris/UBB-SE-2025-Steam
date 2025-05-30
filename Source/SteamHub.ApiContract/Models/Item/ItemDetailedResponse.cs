﻿namespace SteamHub.ApiContract.Models.Item
{
    public class ItemDetailedResponse
    {
        public int ItemId { get; set; }

        public string ItemName { get; set; } = default!;

        public int GameId { get; set; }

        public float Price { get; set; }

        public string Description { get; set; } = default!;

        public bool IsListed { get; set; }

        public string ImagePath { get; set; } = default!;
    }
}
