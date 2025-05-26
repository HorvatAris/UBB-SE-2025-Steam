using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services.Fakes
{
    public class FakeCollectionsService : ICollectionsService
    {
        private List<Collection> collectionList;
        private List<OwnedGame> ownedGameList;

        public FakeCollectionsService()
        {
            // Seed some fake collections.
            collectionList = new List<Collection>();
            collectionList.Add(new Collection(1, "Collection 1", DateOnly.FromDateTime(DateTime.Now.AddDays(-5)), "cover1.jpg", true) { CollectionId = 1 });
            collectionList.Add(new Collection(1, "Collection 2", DateOnly.FromDateTime(DateTime.Now.AddDays(-3)), "cover2.jpg", false) { CollectionId = 2 });
            collectionList.Add(new Collection(1, "Collection 3", DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), "cover3.jpg", true) { CollectionId = 3 });
            collectionList.Add(new Collection(2, "Other User Collection", DateOnly.FromDateTime(DateTime.Now.AddDays(-2)), "cover4.jpg", true) { CollectionId = 4 });

            // Seed some dummy owned games (only for collectionId == 1) using the new constructor.
            ownedGameList = new List<OwnedGame>();
            var gameA = new OwnedGame(1, "Game A", "Description for Game A", "gameA.jpg");
            gameA.GameId = 1;
            ownedGameList.Add(gameA);

            var gameB = new OwnedGame(1, "Game B", "Description for Game B", "gameB.jpg");
            gameB.GameId = 2;
            ownedGameList.Add(gameB);
        }

        public List<Collection> GetAllCollections(int userId)
        {
            List<Collection> result = new List<Collection>();
            foreach (Collection collection in collectionList)
            {
                if (collection.UserId == userId)
                {
                    result.Add(collection);
                }
            }
            return result;
        }

        public Collection GetCollectionByIdentifier(int collectionId, int userId)
        {
            Collection foundCollection = null;
            foreach (Collection collection in collectionList)
            {
                if (collection.CollectionId == collectionId && collection.UserId == userId)
                {
                    foundCollection = collection;
                    break;
                }
            }
            if (foundCollection != null && foundCollection.CollectionId == 1)
            {
                // For collection 1, attach owned games.
                foundCollection.Games = new List<OwnedGame>();
                foreach (OwnedGame ownedGame in ownedGameList)
                {
                    foundCollection.Games.Add(ownedGame);
                }
            }
            return foundCollection;
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId)
        {
            // When user is not specified, return an empty list.
            return new List<OwnedGame>();
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId, int userId)
        {
            if (collectionId == 1)
            {
                List<OwnedGame> result = new List<OwnedGame>();
                foreach (OwnedGame ownedGame in ownedGameList)
                {
                    result.Add(ownedGame);
                }
                return result;
            }
            return GetGamesInCollection(collectionId);
        }

        public void AddGameToCollection(int collectionId, int gameId, int userId)
        {
            // Fake implementation: No operation.
        }

        public void RemoveGameFromCollection(int collectionId, int gameId)
        {
            // Fake implementation: No operation.
        }

        public void MakeCollectionPrivateForUser(string userId, string collectionId)
        {
            // Fake implementation: No operation.
        }

        public void MakeCollectionPublicForUser(string userId, string collectionId)
        {
            // Fake implementation: No operation.
        }

        public void RemoveCollectionForUser(string userId, string collectionId)
        {
            int userIdentifier = int.Parse(userId);
            int collectionIdentifier = int.Parse(collectionId);
            Collection collectionToRemove = null;
            foreach (Collection collection in collectionList)
            {
                if (collection.UserId == userIdentifier && collection.CollectionId == collectionIdentifier)
                {
                    collectionToRemove = collection;
                    break;
                }
            }
            if (collectionToRemove != null)
            {
                collectionList.Remove(collectionToRemove);
            }
        }

        public void SaveCollection(string userId, Collection collection)
        {
            int userIdentifier = int.Parse(userId);
            if (collection.CollectionId == 0)
            {
                int newId = collectionList.Count > 0 ? collectionList[collectionList.Count - 1].CollectionId + 1 : 1;
                collection.CollectionId = newId;
                collection.UserId = userIdentifier;
                collectionList.Add(collection);
            }
            else
            {
                for (int i = 0; i < collectionList.Count; i++)
                {
                    if (collectionList[i].CollectionId == collection.CollectionId && collectionList[i].UserId == userIdentifier)
                    {
                        collectionList[i] = collection;
                        break;
                    }
                }
            }
        }

        public void DeleteCollection(int collectionId, int userId)
        {
            Collection collectionToDelete = null;
            foreach (Collection collection in collectionList)
            {
                if (collection.CollectionId == collectionId && collection.UserId == userId)
                {
                    collectionToDelete = collection;
                    break;
                }
            }
            if (collectionToDelete != null)
            {
                collectionList.Remove(collectionToDelete);
            }
        }

        public void CreateCollection(int userId, string name, string coverPicture, bool isPublic, DateOnly createdAt)
        {
            int newId = collectionList.Count > 0 ? collectionList[collectionList.Count - 1].CollectionId + 1 : 1;
            Collection newCollection = new Collection(userId, name, createdAt, coverPicture, isPublic)
            {
                CollectionId = newId
            };
            collectionList.Add(newCollection);
        }

        public void UpdateCollection(int collectionId, int userId, string name, string coverPicture, bool isPublic)
        {
            foreach (Collection collection in collectionList)
            {
                if (collection.CollectionId == collectionId && collection.UserId == userId)
                {
                    collection.CollectionName = name;
                    collection.CoverPicture = coverPicture;
                    collection.IsPublic = isPublic;
                    collection.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
                    break;
                }
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userId)
        {
            List<Collection> result = new List<Collection>();
            foreach (Collection collection in collectionList)
            {
                if (collection.UserId == userId && collection.IsPublic)
                {
                    result.Add(collection);
                }
            }
            return result;
        }

        public List<OwnedGame> GetGamesNotInCollection(int collectionId, int userId)
        {
            List<OwnedGame> result = new List<OwnedGame>();
            var gameC = new OwnedGame(userId, "Game C", "Description for Game C", "gameC.jpg");
            gameC.GameId = 3;
            result.Add(gameC);
            return result;
        }

        private int CompareByCreatedAtDescending(Collection firstCollection, Collection secondCollection)
        {
            return secondCollection.CreatedAt.CompareTo(firstCollection.CreatedAt);
        }

        public List<Collection> GetLastThreeCollectionsForUser(int userId)
        {
            List<Collection> userCollections = GetAllCollections(userId);
            userCollections.Sort(CompareByCreatedAtDescending);
            int count = userCollections.Count < 3 ? userCollections.Count : 3;
            List<Collection> result = new List<Collection>();
            for (int i = 0; i < count; i++)
            {
                result.Add(userCollections[i]);
            }
            return result;
        }
    }
}
