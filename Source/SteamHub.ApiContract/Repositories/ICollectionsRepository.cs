using SteamHub.ApiContract.Models.Collections;
using SteamHub.ApiContract.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Repositories
{
    public interface ICollectionsRepository
    {
        Task<List<Collection>> GetAllCollectionsAsync(int userIdentifier);

        Task<List<Collection>> GetLastThreeCollectionsForUserAsync(int userIdentifier);

        Task<Collection?> GetCollectionByIdAsync(int collectionIdentifier, int userIdentifier);

        Task<List<OwnedGame>> GetGamesInCollectionAsync(int collectionIdentifier);

        Task<List<OwnedGame>> GetGamesInCollectionAsync(int collectionId, int userId);

        Task AddGameToCollectionAsync(int collectionIdentifier, int gameIdentifier);

        Task RemoveGameFromCollectionAsync(int collectionIdentifier, int gameIdentifier);

        Task CreateCollectionAsync(int userIdentifier, string collectionName, string? coverPicture, bool isPublic, DateOnly createdAt);

        Task UpdateCollectionAsync(int collectionIdentifier, int userIdentifier, string collectionName, string coverPicture, bool isPublic);

        Task DeleteCollectionAsync(int collectionIdentifier, int userIdentifier);

        Task<List<Collection>> GetPublicCollectionsForUserAsync(int userIdentifier);

        Task<List<OwnedGame>> GetGamesNotInCollectionAsync(int collectionIdentifier, int userIdentifier);

        // Not implemented yet
        void MakeCollectionPrivateForUser(string userId, string collectionId);
        void MakeCollectionPublicForUser(string userId, string collectionId);
        void RemoveCollectionForUser(string userId, string collectionId);
        void SaveCollection(string userId, Collection collection);
    }
}
