using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Models.Collections;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Services
{
    public class CollectionsService : ICollectionsService
    {
        // Error message constants
        private const string Error_RetrieveCollectionsDataBase = "Failed to retrieve collections from database";
        private const string Error_RetrieveCollectionsUnexpected = "An unexpected error occurred while retrieving collections";
        private const string Error_RetrieveCollection = "Failed to retrieve collection.";
        private const string Error_RetrieveCollectionUnexpected = "An unexpected error occurred while retrieving collection.";
        private const string Error_RetrieveGamesDataBase = "Failed to retrieve games from database";
        private const string Error_RetrieveGamesUnexpected = "An unexpected error occurred while retrieving games";
        private const string Error_AddGameToCollection = "Failed to add game to collection";
        private const string Error_Unexpected = "An unexpected error occurred";
        private const string Error_RemoveGameFromCollection = "Failed to remove game from collection.";
        private const string Error_RemoveGameFromCollectionUnexpected = "An unexpected error occurred while removing game from collection.";
        private const string Error_DeleteCollection = "Failed to delete collection";
        private const string Error_CreateCollectionDataBase = "Failed to create collection in database";
        private const string Error_CreateCollectionUnexpected = "An unexpected error occurred while creating collection";
        private const string Error_UpdateCollectionDataBase = "Failed to update collection in database";
        private const string Error_UpdateCollectionUnexpected = "An unexpected error occurred while updating collection";
        private const string Error_RetrievePublicCollectionsDataBase = "Failed to retrieve public collections from database";
        private const string Error_RetrievePublicCollectionsUnexpected = "An unexpected error occurred while retrieving public collections";

        private readonly ICollectionsRepository collectionsRepository;

        public CollectionsService(ICollectionsRepository collectionsRepository)
        {
            this.collectionsRepository = collectionsRepository ?? throw new ArgumentNullException(nameof(collectionsRepository));
        }

        public async Task<List<Collection>> GetAllCollections(int userId)
        {
            try
            {
                return await collectionsRepository.GetAllCollectionsAsync(userId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(Error_RetrieveCollectionsDataBase, ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException(Error_RetrieveCollectionsUnexpected, ex);
            }
        }

        public async Task<Collection> GetCollectionByIdentifier(int collectionId, int userId)
        {
            try
            {
                var collection = await collectionsRepository.GetCollectionByIdAsync(collectionId, userId);
                if (collection == null)
                {
                    throw new ServiceException(Error_RetrieveCollection);
                }

                collection.Games = await collectionsRepository.GetGamesInCollectionAsync(collectionId, userId);
                return collection;
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(Error_RetrieveCollection, ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException(Error_RetrieveCollectionUnexpected, ex);
            }
        }

        public async Task<List<Collection>> GetLastThreeCollectionsForUser(int userId)
        {
            return await collectionsRepository.GetLastThreeCollectionsForUserAsync(userId);
        }

        public async Task<List<OwnedGame>> GetGamesInCollection(int collectionId)
        {
            try
            {
                return await collectionsRepository.GetGamesInCollectionAsync(collectionId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(Error_RetrieveGamesDataBase, ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException(Error_RetrieveGamesUnexpected, ex);
            }
        }

        public async Task AddGameToCollection(int collectionId, int gameId, int userId)
        {
            try
            {
                await collectionsRepository.AddGameToCollectionAsync(collectionId, gameId, userId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(Error_AddGameToCollection, ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException(Error_Unexpected, ex);
            }
        }

        public async Task RemoveGameFromCollection(int collectionId, int gameId, int userId)
        {
            try
            {
                await collectionsRepository.RemoveGameFromCollectionAsync(collectionId, gameId, userId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(Error_RemoveGameFromCollection, ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException(Error_RemoveGameFromCollectionUnexpected, ex);
            }
        }

        public async Task DeleteCollection(int collectionId, int userId)
        {
            try
            {
                await collectionsRepository.DeleteCollectionAsync(collectionId, userId);
            }
            catch (Exception ex)
            {
                throw new Exception(Error_DeleteCollection, ex);
            }
        }

        public async Task CreateCollection(int userId, string collectionName, string coverPicture, bool isPublic, DateOnly createdAt)
        {
            try
            {
                await collectionsRepository.CreateCollectionAsync(userId, collectionName, coverPicture, isPublic, createdAt);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(Error_CreateCollectionDataBase, ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException(Error_CreateCollectionUnexpected, ex);
            }
        }

        public async Task UpdateCollection(int collectionId, int userId, string collectionName, string coverPicture, bool isPublic)
        {
            try
            {
                await collectionsRepository.UpdateCollectionAsync(collectionId, userId, collectionName, coverPicture, isPublic);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(Error_UpdateCollectionDataBase, ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException(Error_UpdateCollectionUnexpected, ex);
            }
        }

        public async Task<List<Collection>> GetPublicCollectionsForUser(int userId)
        {
            try
            {
                return await collectionsRepository.GetPublicCollectionsForUserAsync(userId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException(Error_RetrievePublicCollectionsDataBase, ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException(Error_RetrievePublicCollectionsUnexpected, ex);
            }
        }

        public async Task<List<OwnedGame>> GetGamesNotInCollection(int collectionId, int userId)
        {
            return await collectionsRepository.GetGamesNotInCollectionAsync(collectionId, userId);
        }
    }
}