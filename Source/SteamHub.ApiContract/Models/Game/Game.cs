// <copyright file="Game.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace SteamHub.ApiContract.Models.Game;

using System;
using System.Runtime.Intrinsics.X86;

public class Game
{
    public const decimal NOTCOMPUTED = -111111;

    // Added from business layer
    //public int GameIdentifier { get; set; }
    //public string TitleOfGame { get; set; }
    //public string DescriptionOfGame { get; set; }
    //public string CoverImagePathOrUrl { get; set; }
    //public DateTime DateWhenGameWasReleased { get; set; }

    //// optianal display helpers
    //public int RetrieveGameId() => GameIdentifier;
    //public string RetrieveGameTitle() => TitleOfGame;

    public Game()
    {
    }

    public int GameId { get; set; }

    public string GameTitle { get; set; }

    public string GameDescription { get; set; }

    public string ImagePath { get; set; }

    public decimal Price { get; set; }

    public string MinimumRequirements { get; set; }

    public string RecommendedRequirements { get; set; }

    public string Status { get; set; }

    public string[] Tags { get; set; }

    public decimal Rating { get; set; }

    public int NumberOfRecentPurchases { get; set; }

    public decimal TrendingScore { get; set; }

    public string TrailerPath { get; set; }

    public string GameplayPath { get; set; }

    public decimal Discount { get; set; }

    public decimal TagScore { get; set; }

    public int PublisherIdentifier { get; set; }
}
