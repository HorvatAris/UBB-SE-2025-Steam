using SteamHub.ApiContract.Models.Collections;
using SteamHub.ApiContract.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Services.Interfaces
{
    public interface ICollectionsService
    {
        Task<List<Collection>> GetAllCollections(int userId);

        Task<Collection> GetCollectionByIdentifier(int collectionId, int userId);

        Task<List<Collection>> GetLastThreeCollectionsForUser(int userId);

        Task<List<OwnedGame>> GetGamesInCollection(int collectionId);

        Task AddGameToCollection(int collectionId, int gameId);

        Task RemoveGameFromCollection(int collectionId, int gameId);

        Task DeleteCollection(int collectionId, int userId);

        Task CreateCollection(int userId, string collectionName, string coverPicture, bool isPublic, DateOnly createdAt);

        Task UpdateCollection(int collectionId, int userId, string collectionName, string coverPicture, bool isPublic);

        Task<List<Collection>> GetPublicCollectionsForUser(int userId);

        Task<List<OwnedGame>> GetGamesNotInCollection(int collectionId, int userId);
    }
}
