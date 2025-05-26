using BusinessLayer.Models;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services
{
    public class OwnedGamesService : IOwnedGamesService
    {
        // Error message constants
        private const string Error_RetrieveOwnedGames = "Failed to retrieve owned games.";
        private const string Error_RetrieveOwnedGamesUnexpected = "An unexpected error occurred while retrieving owned games.";
        private const string Error_RetrieveOwnedGame = "Failed to retrieve owned game.";
        private const string Error_RetrieveOwnedGameUnexpected = "An unexpected error occurred while retrieving owned game.";
        private const string Error_RemoveOwnedGame = "Failed to remove owned game.";
        private const string Error_RemoveOwnedGameUnexpected = "An unexpected error occurred while removing owned game.";

        private readonly IOwnedGamesRepository ownedGamesRepository;

        public OwnedGamesService(IOwnedGamesRepository ownedGamesRepository)
        {
            this.ownedGamesRepository = ownedGamesRepository ?? throw new ArgumentNullException(nameof(ownedGamesRepository));
        }

        public List<OwnedGame> GetAllOwnedGames(int userId)
        {
            try
            {
                var ownedGamesList = ownedGamesRepository.GetAllOwnedGames(userId);
                return ownedGamesList;
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Error_RetrieveOwnedGames, repositoryException);
            }
            catch (Exception unexpectedException)
            {
                throw new ServiceException(Error_RetrieveOwnedGamesUnexpected, unexpectedException);
            }
        }

        public OwnedGame GetOwnedGameByIdentifier(int gameId, int userId)
        {
            try
            {
                var ownedGame = ownedGamesRepository.GetOwnedGameById(gameId, userId);

                if (ownedGame == null)
                {
                    return null;
                }

                return ownedGame;
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Error_RetrieveOwnedGame, repositoryException);
            }
            catch (Exception unexpectedException)
            {
                throw new ServiceException(Error_RetrieveOwnedGameUnexpected, unexpectedException);
            }
        }

        public void RemoveOwnedGame(int gameId, int userId)
        {
            try
            {
                ownedGamesRepository.RemoveOwnedGame(gameId, userId);
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Error_RemoveOwnedGame, repositoryException);
            }
            catch (Exception unexpectedException)
            {
                throw new ServiceException(Error_RemoveOwnedGameUnexpected, unexpectedException);
            }
        }
    }
}
