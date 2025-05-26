using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface IOwnedGamesService
    {
        List<OwnedGame> GetAllOwnedGames(int userIdentifier);
        OwnedGame GetOwnedGameByIdentifier(int gameIdentifier, int userIdentifier);
        void RemoveOwnedGame(int gameIdentifier, int userIdentifier);
    }
}