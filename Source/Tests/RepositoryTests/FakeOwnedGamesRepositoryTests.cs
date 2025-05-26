using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Fakes;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class FakeOwnedGamesRepositoryTests
    {
        private FakeOwnedGamesRepository fakeOwnedGamesRepository;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Instantiate the fake owned games repository.
            fakeOwnedGamesRepository = new FakeOwnedGamesRepository();
        }

        [Test]
        public void GetAllOwnedGames_WithValidUserId_ReturnsCorrectCount()
        {
            // Arrange
            int validUserId = 1;

            // Act: Retrieve all owned games for the valid user.
            var ownedGamesForUser = fakeOwnedGamesRepository.GetAllOwnedGames(validUserId);

            // Assert: Expect exactly two owned games.
            Assert.That(ownedGamesForUser.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetAllOwnedGames_WithNonExistingUser_ReturnsEmptyList()
        {
            // Arrange
            int nonExistingUserId = 999;

            // Act: Retrieve all owned games for the non-existing user.
            var ownedGamesForNonExistingUser = fakeOwnedGamesRepository.GetAllOwnedGames(nonExistingUserId);

            // Assert: Expect an empty list.
            Assert.That(ownedGamesForNonExistingUser, Is.Empty);
        }

        [Test]
        public void GetOwnedGameById_WithValidIdAndUser_ReturnsCorrectGame()
        {
            // Arrange
            int validUserId = 1;
            int validGameId = 1;

            // Act: Retrieve the owned game with the given game ID for the valid user.
            var ownedGame = fakeOwnedGamesRepository.GetOwnedGameById(validGameId, validUserId);

            // Assert: The returned game's GameId should match the valid game ID.
            Assert.That(ownedGame?.GameId, Is.EqualTo(validGameId));
        }

        [Test]
        public void GetOwnedGameById_WithInvalidUser_ReturnsNull()
        {
            // Arrange
            int validGameId = 1;
            int invalidUserId = 999;

            // Act: Retrieve the owned game with the given game ID for an invalid user.
            var ownedGame = fakeOwnedGamesRepository.GetOwnedGameById(validGameId, invalidUserId);

            // Assert: Expect the result to be null.
            Assert.That(ownedGame, Is.Null);
        }

        [Test]
        public void RemoveOwnedGame_WithValidGame_RemovesGameSuccessfully()
        {
            // Arrange
            int validUserId = 1;
            int validGameId = 1;

            // Act: Remove the owned game for the valid game ID and valid user.
            fakeOwnedGamesRepository.RemoveOwnedGame(validGameId, validUserId);
            var removedGame = fakeOwnedGamesRepository.GetOwnedGameById(validGameId, validUserId);

            // Assert: Expect the removed game to be null.
            Assert.That(removedGame, Is.Null);
        }

        [Test]
        public void RemoveOwnedGame_WithInvalidGame_DoesNotChangeOwnedGamesCount()
        {
            // Arrange
            int validUserId = 1;
            int invalidGameId = 999;

            // Act: Attempt to remove a non-existing game for the valid user.
            fakeOwnedGamesRepository.RemoveOwnedGame(invalidGameId, validUserId);
            var remainingOwnedGames = fakeOwnedGamesRepository.GetAllOwnedGames(validUserId);

            // Assert: Expect the count of owned games to remain unchanged (2).
            Assert.That(remainingOwnedGames.Count, Is.EqualTo(2));
        }
    }
}
