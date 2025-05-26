using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface ICollectionsService
    {
        List<Collection> GetAllCollections(int userIdentifier);
        Collection GetCollectionByIdentifier(int collectionIdentifier, int userIdentifier);
        List<OwnedGame> GetGamesInCollection(int collectionIdentifier);
        void AddGameToCollection(int collectionIdentifier, int gameIdentifier, int userIdentifier);
        void RemoveGameFromCollection(int collectionIdentifier, int gameIdentifier);
        void DeleteCollection(int collectionIdentifier, int userIdentifier);
        void CreateCollection(int userIdentifier, string collectionName, string coverPicture, bool isPublic, DateOnly createdAt);
        void UpdateCollection(int collectionIdentifier, int userIdentifier, string collectionName, string coverPicture, bool isPublic);
        List<Collection> GetPublicCollectionsForUser(int userIdentifier);
        List<OwnedGame> GetGamesNotInCollection(int collectionIdentifier, int userIdentifier);
    }
}