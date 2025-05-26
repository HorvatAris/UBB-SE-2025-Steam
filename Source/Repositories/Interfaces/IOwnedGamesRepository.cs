using BusinessLayer.Models;

namespace BusinessLayer.Repositories.Interfaces
{
    public interface IOwnedGamesRepository
    {
        List<OwnedGame> GetAllOwnedGames(int userId);
        OwnedGame GetOwnedGameById(int gameId, int userId);
        void RemoveOwnedGame(int gameId, int userId);
    }
}