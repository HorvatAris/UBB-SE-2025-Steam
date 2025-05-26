using BusinessLayer.Models;

namespace BusinessLayer.Repositories.Interfaces
{
    public interface ICollectionsRepository
    {
        List<Collection> GetAllCollections(int userId);
        List<Collection> GetLastThreeCollectionsForUser(int userId);
        Collection GetCollectionById(int collectionId, int userId);
        List<OwnedGame> GetGamesInCollection(int collectionId);
        List<OwnedGame> GetGamesInCollection(int collectionId, int userId);
        void AddGameToCollection(int collectionId, int gameId, int userId);
        void RemoveGameFromCollection(int collectionId, int gameId);
        void MakeCollectionPrivateForUser(string userId, string collectionId);
        void MakeCollectionPublicForUser(string userId, string collectionId);
        void RemoveCollectionForUser(string userId, string collectionId);
        void SaveCollection(string userId, Collection collection);
        void DeleteCollection(int collectionId, int userId);
        void CreateCollection(int userId, string collectionName, string coverPicture, bool isPublic, DateOnly createdAt);
        void UpdateCollection(int collectionId, int userId, string collectionName, string coverPicture, bool isPublic);
        List<Collection> GetPublicCollectionsForUser(int userId);
        List<OwnedGame> GetGamesNotInCollection(int collectionId, int userId);
    }
}