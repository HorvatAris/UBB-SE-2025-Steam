using System;
using System.Collections.Generic;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories.Fakes
{
    public class FakePointsOffersRepository : IPointsOffersRepository
    {
        private readonly List<PointsOffer> offers;

        public FakePointsOffersRepository()
        {
            // Initialize with test data
            offers = new List<PointsOffer>
            {
                new PointsOffer(2, 5),    // 5 points for $2
                new PointsOffer(8, 25),   // 25 points for $8
                new PointsOffer(15, 50),  // 50 points for $15
                new PointsOffer(20, 100), // 100 points for $20
                new PointsOffer(50, 500) // 500 points for $50
            };
        }

        public List<PointsOffer> PointsOffers
        {
            get { return offers; }
        }
    }
}