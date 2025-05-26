using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services.Fakes
{
    public class FakeOwnedGamesService : IOwnedGamesService
    {
        private readonly List<OwnedGame> ownedGameList;

        public FakeOwnedGamesService()
        {
            // Seed with some dummy owned games using the proper OwnedGame constructor.
            ownedGameList = new List<OwnedGame>
            {
                new OwnedGame(1, "Game A", "Description for Game A", "gamea.jpg") { GameId = 100 },
                new OwnedGame(1, "Game B", "Description for Game B", "gameb.jpg") { GameId = 101 },
                new OwnedGame(2, "Game C", "Description for Game C", "gamec.jpg") { GameId = 102 }
            };
        }

        public List<OwnedGame> GetAllOwnedGames(int userId)
        {
            return ownedGameList.Where(ownedGame => ownedGame.UserId == userId).ToList();
        }

        public OwnedGame GetOwnedGameByIdentifier(int gameId, int userId)
        {
            return ownedGameList.FirstOrDefault(ownedGame => ownedGame.GameId == gameId && ownedGame.UserId == userId);
        }

        public void RemoveOwnedGame(int gameId, int userId)
        {
            var ownedGameToRemove = ownedGameList.FirstOrDefault(ownedGame => ownedGame.GameId == gameId && ownedGame.UserId == userId);
            if (ownedGameToRemove != null)
            {
                ownedGameList.Remove(ownedGameToRemove);
            }
        }
    }
}