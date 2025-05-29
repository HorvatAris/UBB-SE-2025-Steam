using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Models.Collections;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class CollectionsServiceProxy : ServiceProxy, ICollectionsService
    {
        public CollectionsServiceProxy(string baseUrl = "https://localhost:7241/api/")
            : base(baseUrl)
        {
        }

        public async Task<List<Collection>> GetAllCollections(int userIdentifier)
        {
            try
            {
                return await GetAsync<List<Collection>>($"Collections/{userIdentifier}");
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve collections from server", ex);
            }
        }

        public async Task<Collection> GetCollectionByIdentifier(int collectionIdentifier, int userIdentifier)
        {
            try
            {
                return await GetAsync<Collection>($"Collection/{collectionIdentifier}/user/{userIdentifier}");
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve collection from server", ex);
            }
        }

        public async Task<List<OwnedGame>> GetGamesInCollection(int collectionIdentifier)
        {
            try
            {
                return await GetAsync<List<OwnedGame>>($"Collections/{collectionIdentifier}/games");
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve games from server", ex);
            }
        }

        public async Task AddGameToCollection(int collectionIdentifier, int gameIdentifier)
        {
            try
            {
                Debug.WriteLine($"Sending: CollectionId={collectionIdentifier}, GameId={gameIdentifier}");

                await PostAsync("Collections/add-game", new
                {
                    CollectionId = collectionIdentifier,
                    GameId = gameIdentifier
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AddGameToCollection] EXCEPTION: {ex.Message}");
                Debug.WriteLine($"[AddGameToCollection] STACK TRACE: {ex.StackTrace}");
                throw new ServiceException("Failed to add game to collection", ex);
            }
        }

        public async Task RemoveGameFromCollection(int collectionIdentifier, int gameIdentifier)
        {
            try
            {
                await PostAsync("Collections/remove-game", new
                {
                    CollectionId = collectionIdentifier,
                    GameId = gameIdentifier
                });
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to remove game from collection", ex);
            }
        }

        public async Task DeleteCollection(int collectionIdentifier, int userIdentifier)
        {
            try
            {
                await DeleteAsync<object>($"Collection/{collectionIdentifier}/user/{userIdentifier}");
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to delete collection", ex);
            }
        }

        public async Task CreateCollection(int userIdentifier, string collectionName, string coverPicture, bool isPublic, DateOnly createdAt)
        {
            try
            {
                var collection = new Collection(userIdentifier, collectionName, createdAt, coverPicture, isPublic);
                await PostAsync("Collection", collection);
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to create collection", ex);
            }
        }

        public async Task UpdateCollection(int collectionIdentifier, int userIdentifier, string collectionName, string coverPicture, bool isPublic)
        {
            try
            {
                await PutAsync<Collection>($"Collections/{collectionIdentifier}", new
                {
                    UserId = userIdentifier,
                    CollectionName = collectionName,
                    CoverPicture = coverPicture,
                    IsPublic = isPublic
                });
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to update collection", ex);
            }
        }

        public async Task<List<Collection>> GetPublicCollectionsForUser(int userIdentifier)
        {
            try
            {
                return await GetAsync<List<Collection>>($"Collections/public/{userIdentifier}");
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve public collections from server", ex);
            }
        }

        public async Task<List<OwnedGame>> GetGamesNotInCollection(int collectionIdentifier, int userIdentifier)
        {
            try
            {
                return await GetAsync<List<OwnedGame>>(
                    $"Collections/{collectionIdentifier}/user/{userIdentifier}/games-not-in-collection");
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve games not in collection from server", ex);
            }
        }

        public async Task<List<Collection>> GetLastThreeCollectionsForUser(int userIdentifier)
        {
            try
            {
                return await GetAsync<List<Collection>>(
                    $"Collections/user/{userIdentifier}/last-three");
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve last three collections from server", ex);
            }
        }
    }
}