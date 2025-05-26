using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories.Fakes
{
    public class FakeOwnedGamesRepository : IOwnedGamesRepository
    {
        private readonly List<OwnedGame> ownedGamesList;

        public FakeOwnedGamesRepository()
        {
            // Seed with some dummy owned games for testing.
            ownedGamesList = new List<OwnedGame>
            {
                // Creating OwnedGame using the new constructor, then assigning GameId.
                CreateOwnedGame(1, 1, "Game A", "Description A", "gameA.jpg"),
                CreateOwnedGame(1, 2, "Game B", "Description B", "gameB.jpg"),
                CreateOwnedGame(2, 3, "Game C", "Description C", "gameC.jpg")
            };
        }

        public List<OwnedGame> GetAllOwnedGames(int userId)
        {
            return ownedGamesList.Where(ownedGame => ownedGame.UserId == userId).ToList();
        }

        public OwnedGame GetOwnedGameById(int gameId, int userId)
        {
            return ownedGamesList.FirstOrDefault(ownedGame => ownedGame.GameId == gameId && ownedGame.UserId == userId);
        }

        public void RemoveOwnedGame(int gameId, int userId)
        {
            var ownedGame = ownedGamesList.FirstOrDefault(ownedGame => ownedGame.GameId == gameId && ownedGame.UserId == userId);
            if (ownedGame != null)
            {
                ownedGamesList.Remove(ownedGame);
            }
        }

        // Helper method to create an OwnedGame using the new constructor.
        private OwnedGame CreateOwnedGame(int userId, int gameId, string gameTitle, string description, string coverPicture)
        {
            var ownedGame = new OwnedGame(userId, gameTitle, description, coverPicture);
            ownedGame.GameId = gameId;
            return ownedGame;
        }
    }
}