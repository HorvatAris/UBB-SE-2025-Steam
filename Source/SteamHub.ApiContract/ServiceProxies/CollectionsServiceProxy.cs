using System;
using System.Collections.Generic;
using BusinessLayer.Models;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Models.Collections;
using SteamHub.ApiContract.Models.Game;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class CollectionsServiceProxy : ServiceProxy, ICollectionsService
    {
        public CollectionsServiceProxy(string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
        }

        public List<Collection> GetAllCollections(int userIdentifier)
        {
            try
            {
                return GetAsync<List<Collection>>($"Collection/{userIdentifier}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve collections from server", ex);
            }
        }

        public Collection GetCollectionByIdentifier(int collectionIdentifier, int userIdentifier)
        {
            try
            {
                return GetAsync<Collection>($"Collection/{collectionIdentifier}/user/{userIdentifier}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve collection from server", ex);
            }
        }

        public List<OwnedGame> GetGamesInCollection(int collectionIdentifier)
        {
            try
            {
                return GetAsync<List<OwnedGame>>($"Collection/{collectionIdentifier}/games").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve games from server", ex);
            }
        }

        public void AddGameToCollection(int collectionIdentifier, int gameIdentifier, int userIdentifier)
        {
            try
            {
                PostAsync("Collection/add-game", new
                {
                    CollectionId = collectionIdentifier,
                    GameId = gameIdentifier,
                    UserId = userIdentifier
                }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to add game to collection", ex);
            }
        }

        public void RemoveGameFromCollection(int collectionIdentifier, int gameIdentifier)
        {
            try
            {
                PostAsync("Collection/remove-game", new
                {
                    CollectionId = collectionIdentifier,
                    GameId = gameIdentifier
                }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to remove game from collection", ex);
            }
        }

        public void DeleteCollection(int collectionIdentifier, int userIdentifier)
        {
            try
            {
                DeleteAsync<object>($"Collection/{collectionIdentifier}/user/{userIdentifier}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to delete collection", ex);
            }
        }

        public void CreateCollection(int userIdentifier, string collectionName, string coverPicture, bool isPublic, DateOnly createdAt)
        {
            try
            {
                var collection = new Collection(
                    userIdentifier,   // for userId
                    collectionName,   // for collectionName
                    createdAt,        // for createdAt
                    coverPicture,     // for coverPicture
                    isPublic);

                PostAsync("Collection", collection).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to create collection", ex);
            }
        }

        public void UpdateCollection(int collectionIdentifier, int userIdentifier, string collectionName, string coverPicture, bool isPublic)
        {
            try
            {
                PutAsync<Collection>($"Collection/{collectionIdentifier}", new
                {
                    UserId = userIdentifier,
                    CollectionName = collectionName,
                    CoverPicture = coverPicture,
                    IsPublic = isPublic
                }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to update collection", ex);
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userIdentifier)
        {
            try
            {
                return GetAsync<List<Collection>>($"Collection/public/{userIdentifier}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve public collections from server", ex);
            }
        }

        public List<OwnedGame> GetGamesNotInCollection(int collectionIdentifier, int userIdentifier)
        {
            try
            {
                return GetAsync<List<OwnedGame>>($"Collection/{collectionIdentifier}/user/{userIdentifier}/games-not-in-collection").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve games not in collection from server", ex);
            }
        }

        public List<Collection> GetLastThreeCollectionsForUser(int userIdentifier)
        {
            // TEMPORARY: This method is not implemented yet.
            return new List<Collection> { };
        }
    }
}