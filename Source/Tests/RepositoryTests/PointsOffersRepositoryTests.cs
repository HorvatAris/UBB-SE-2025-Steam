using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Repositories;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class PointsOffersRepositoryTests
    {
        private PointsOffersRepository pointsOffersRepository;

        [SetUp]
        public void SetUp()
        {
            pointsOffersRepository = new PointsOffersRepository();
        }

        [Test]
        public void Offers_ReturnsNonNullList()
        {
            // Arrange

            // Act
            var offers = pointsOffersRepository.PointsOffers;

            // Assert
            Assert.That(offers, Is.Not.Null);
        }

        [Test]
        public void Offers_ReturnsCorrectNumberOfOffers()
        {
            // Arrange

            // Act
            var offers = pointsOffersRepository.PointsOffers;

            // Assert
            Assert.That(offers.Count, Is.EqualTo(5));
        }

        [Test]
        public void Offers_FirstOffer_HasCorrectPrice()
        {
            // Arrange

            // Act
            var offers = pointsOffersRepository.PointsOffers;

            // Assert
            Assert.That(offers[0].Price, Is.EqualTo(2));
        }

        [Test]
        public void Offers_FirstOffer_HasCorrectPoints()
        {
            // Arrange

            // Act
            var offers = pointsOffersRepository.PointsOffers;

            // Assert
            Assert.That(offers[0].Points, Is.EqualTo(5));
        }

        [Test]
        public void Offers_SecondOffer_HasCorrectPrice()
        {
            // Arrange

            // Act
            var offers = pointsOffersRepository.PointsOffers;

            // Assert
            Assert.That(offers[1].Price, Is.EqualTo(8));
        }

        [Test]
        public void Offers_SecondOffer_HasCorrectPoints()
        {
            // Arrange

            // Act
            var offers = pointsOffersRepository.PointsOffers;

            // Assert
            Assert.That(offers[1].Points, Is.EqualTo(25));
        }

        [Test]
        public void Offers_ThirdOffer_HasCorrectPrice()
        {
            // Arrange

            // Act
            var offers = pointsOffersRepository.PointsOffers;

            // Assert
            Assert.That(offers[2].Price, Is.EqualTo(15));
        }

        [Test]
        public void Offers_ThirdOffer_HasCorrectPoints()
        {
            // Arrange

            // Act
            var offers = pointsOffersRepository.PointsOffers;

            // Assert
            Assert.That(offers[2].Points, Is.EqualTo(50));
        }

        [Test]
        public void Offers_FourthOffer_HasCorrectPrice()
        {
            // Arrange

            // Act
            var offers = pointsOffersRepository.PointsOffers;

            // Assert
            Assert.That(offers[3].Price, Is.EqualTo(20));
        }

        [Test]
        public void Offers_FourthOffer_HasCorrectPoints()
        {
            // Arrange

            // Act
            var offers = pointsOffersRepository.PointsOffers;

            // Assert
            Assert.That(offers[3].Points, Is.EqualTo(100));
        }

        [Test]
        public void Offers_FifthOffer_HasCorrectPrice()
        {
            // Arrange

            // Act
            var offers = pointsOffersRepository.PointsOffers;

            // Assert
            Assert.That(offers[4].Price, Is.EqualTo(50));
        }

        [Test]
        public void Offers_FifthOffer_HasCorrectPoints()
        {
            // Arrange

            // Act
            var offers = pointsOffersRepository.PointsOffers;

            // Assert
            Assert.That(offers[4].Points, Is.EqualTo(500));
        }
    }
}