using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories.Fakes
{
    public class FakeCollectionsRepository : ICollectionsRepository
    {
        private readonly List<Collection> collectionList = new();

        public FakeCollectionsRepository()
        {
            // Seed with some dummy collections for testing.
            collectionList.Add(new Collection(userId: 1, collectionName: "Test Collection 1", createdAt: DateOnly.FromDateTime(DateTime.Now.AddDays(-5)), coverPicture: "pic1.jpg", isPublic: true) { CollectionId = 1 });
            collectionList.Add(new Collection(userId: 1, collectionName: "Test Collection 2", createdAt: DateOnly.FromDateTime(DateTime.Now.AddDays(-3)), coverPicture: "pic2.jpg", isPublic: false) { CollectionId = 2 });
            collectionList.Add(new Collection(userId: 1, collectionName: "Test Collection 3", createdAt: DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), coverPicture: "pic3.jpg", isPublic: true) { CollectionId = 3 });
            collectionList.Add(new Collection(userId: 2, collectionName: "Other User Collection", createdAt: DateOnly.FromDateTime(DateTime.Now.AddDays(-2)), coverPicture: "pic4.jpg", isPublic: true) { CollectionId = 4 });
        }

        public List<Collection> GetAllCollections(int userId)
        {
            return collectionList.Where(collection => collection.UserId == userId).ToList();
        }

        public List<Collection> GetLastThreeCollectionsForUser(int userId)
        {
            return GetAllCollections(userId)
                .OrderByDescending(collection => collection.CreatedAt)
                .Take(3)
                .ToList();
        }

        public Collection GetCollectionById(int collectionId, int userId)
        {
            return collectionList.FirstOrDefault(collection => collection.CollectionId == collectionId && collection.UserId == userId);
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId)
        {
            // For testing, return an empty list.
            return new List<OwnedGame>();
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId, int userId)
        {
            // If collectionId == 1, we mimic returning all games for the user.
            if (collectionId == 1)
            {
                return new List<OwnedGame>
                {
                    CreateOwnedGame(userId, 1, "Game A", "Desc A", "gameA.jpg"),
                    CreateOwnedGame(userId, 2, "Game B", "Desc B", "gameB.jpg")
                };
            }
            else
            {
                return GetGamesInCollection(collectionId);
            }
        }

        public void AddGameToCollection(int collectionId, int gameId, int userId)
        {
            // No-op for fake.
        }

        public void RemoveGameFromCollection(int collectionId, int gameId)
        {
            // No-op for fake.
        }

        public void MakeCollectionPrivateForUser(string userId, string collectionId)
        {
            // For fake, simply do nothing.
        }

        public void MakeCollectionPublicForUser(string userId, string collectionId)
        {
            // For fake, simply do nothing.
        }

        public void RemoveCollectionForUser(string userId, string collectionId)
        {
            int userIdInteger = int.Parse(userId);
            int collectionIdInteger = int.Parse(collectionId);
            var collection = collectionList.FirstOrDefault(c => c.CollectionId == collectionIdInteger && c.UserId == userIdInteger);
            if (collection != null)
            {
                collectionList.Remove(collection);
            }
        }

        public void SaveCollection(string userId, Collection collection)
        {
            int userIdInteger = int.Parse(userId);
            if (collection.CollectionId == 0)
            {
                // Mimic creating new collection.
                collection.CollectionId = collectionList.Any() ? collectionList.Max(c => c.CollectionId) + 1 : 1;
                collection.UserId = userIdInteger;
                collectionList.Add(collection);
            }
            else
            {
                // Mimic update: remove the old version and add new one.
                var existingCollection = collectionList.FirstOrDefault(c => c.CollectionId == collection.CollectionId && c.UserId == userIdInteger);
                if (existingCollection != null)
                {
                    collectionList.Remove(existingCollection);
                    collectionList.Add(collection);
                }
            }
        }

        public void DeleteCollection(int collectionId, int userId)
        {
            var collection = collectionList.FirstOrDefault(collection => collection.CollectionId == collectionId && collection.UserId == userId);
            if (collection != null)
            {
                collectionList.Remove(collection);
            }
        }

        public void CreateCollection(int userId, string name, string coverPicture, bool isPublic, DateOnly createdAt)
        {
            var collection = new Collection(userId, name, createdAt, coverPicture, isPublic)
            {
                CollectionId = collectionList.Any() ? collectionList.Max(collection => collection.CollectionId) + 1 : 1
            };
            collectionList.Add(collection);
        }

        public void UpdateCollection(int collectionId, int userId, string name, string coverPicture, bool isPublic)
        {
            var collection = collectionList.FirstOrDefault(collection => collection.CollectionId == collectionId && collection.UserId == userId);
            if (collection != null)
            {
                // For simplicity, we replace properties.
                collection.CollectionName = name;
                collection.CoverPicture = coverPicture;
                collection.IsPublic = isPublic;
                // Update CreatedAt to now (as in original implementation)
                collection.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userId)
        {
            return collectionList.Where(collection => collection.UserId == userId && collection.IsPublic).ToList();
        }

        public List<OwnedGame> GetGamesNotInCollection(int collectionId, int userId)
        {
            // Return dummy games not in the collection.
            return new List<OwnedGame>
            {
                CreateOwnedGame(userId, 3, "Game C", "Desc C", "gameC.jpg")
            };
        }

        // Helper method to create OwnedGame using the new constructor.
        private OwnedGame CreateOwnedGame(int userId, int gameId, string title, string description, string coverPicture)
        {
            var ownedGame = new OwnedGame(userId, title, description, coverPicture);
            ownedGame.GameId = gameId;
            return ownedGame;
        }
    }
}
