using System.Collections.Generic;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Services.Fakes;
using BusinessLayer.Services.Interfaces;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class FakeOwnedGamesServiceTests
    {
        private IOwnedGamesService fakeOwnedGamesService;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Instantiate the fake owned games service.
            fakeOwnedGamesService = new FakeOwnedGamesService();
        }

        #region GetAllOwnedGames

        [Test]
        public void GetAllOwnedGames_UserExists_ReturnsCorrectGameCount()
        {
            // Arrange

            // Act: For userId 1, seeded data contains 2 games.
            List<OwnedGame> ownedGames = fakeOwnedGamesService.GetAllOwnedGames(1);

            // Assert: Expect the count to be exactly 2.
            Assert.That(ownedGames.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetAllOwnedGames_UserExists_AllReturnedGamesHaveUserId1()
        {
            // Arrange

            // Act: Retrieve owned games for userId 1.
            List<OwnedGame> ownedGames = fakeOwnedGamesService.GetAllOwnedGames(1);

            // Assert: Verify each game has UserId equal to 1.
            Assert.That(ownedGames.TrueForAll(game => game.UserId == 1), Is.True);
        }

        #endregion

        #region GetOwnedGameById

        [Test]
        public void GetOwnedGameById_GameExists_ReturnsNonNullGame()
        {
            // Arrange

            // Act: For userId 1, GameId 100 exists.
            OwnedGame ownedGame = fakeOwnedGamesService.GetOwnedGameByIdentifier(100, 1);

            // Assert: Expect the returned game to be non-null.
            Assert.That(ownedGame, Is.Not.Null);
        }

        [Test]
        public void GetOwnedGameById_GameExists_ReturnsGameWithCorrectGameId()
        {
            // Arrange

            // Act: Retrieve the owned game for GameId 100 for userId 1.
            OwnedGame ownedGame = fakeOwnedGamesService.GetOwnedGameByIdentifier(100, 1);

            // Assert: Verify that the game's GameId equals 100.
            Assert.That(ownedGame.GameId, Is.EqualTo(100));
        }

        [Test]
        public void GetOwnedGameById_GameDoesNotExist_ReturnsNull()
        {
            // Arrange

            // Act: For userId 1, GameId 999 does not exist.
            OwnedGame ownedGame = fakeOwnedGamesService.GetOwnedGameByIdentifier(999, 1);

            // Assert: Expect the returned value to be null.
            Assert.That(ownedGame, Is.Null);
        }

        #endregion

        #region RemoveOwnedGame

        [Test]
        public void RemoveOwnedGame_GameExists_DecreasesGameCount()
        {
            // Arrange: Retrieve the initial count of owned games for userId 1.
            int initialGameCount = fakeOwnedGamesService.GetAllOwnedGames(1).Count;

            // Act: Remove an existing game (GameId 100) for userId 1.
            fakeOwnedGamesService.RemoveOwnedGame(100, 1);
            int updatedGameCount = fakeOwnedGamesService.GetAllOwnedGames(1).Count;

            // Assert: Expect the count to decrease by one.
            Assert.That(updatedGameCount, Is.EqualTo(initialGameCount - 1));
        }

        [Test]
        public void RemoveOwnedGame_GameExists_GameIsNoLongerRetrievable()
        {
            // Arrange

            // Act: Remove game with GameId 100 for userId 1.
            fakeOwnedGamesService.RemoveOwnedGame(100, 1);
            OwnedGame removedGame = fakeOwnedGamesService.GetOwnedGameByIdentifier(100, 1);

            // Assert: The removed game should not be retrievable (should be null).
            Assert.That(removedGame, Is.Null);
        }

        [Test]
        public void RemoveOwnedGame_GameDoesNotExist_DoesNotChangeGameCount()
        {
            // Arrange: Retrieve the initial count of owned games for userId 1.
            int initialGameCount = fakeOwnedGamesService.GetAllOwnedGames(1).Count;

            // Act: Attempt to remove a non-existing game (GameId 999) for userId 1.
            fakeOwnedGamesService.RemoveOwnedGame(999, 1);
            int updatedGameCount = fakeOwnedGamesService.GetAllOwnedGames(1).Count;

            // Assert: Expect the game count to remain unchanged.
            Assert.That(updatedGameCount, Is.EqualTo(initialGameCount));
        }

        #endregion
    }
}
