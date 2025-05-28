using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.Collections;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Repositories;

// Type aliases to resolve ambiguity
using EntityCollection = SteamHub.Api.Entities.Collection;
using ModelCollection = SteamHub.ApiContract.Models.Collections.Collection;
using ModelOwnedGame = SteamHub.ApiContract.Models.Game.OwnedGame;
using EntityCollectionGame = SteamHub.Api.Entities.CollectionGame;


namespace SteamHub.Api.Context.Repositories
{
    public class CollectionsRepository : ICollectionsRepository
    {
        private readonly DataContext context;

        public CollectionsRepository(DataContext newContext)
        {
            this.context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public List<ModelCollection> GetAllCollections(int userIdentifier)
        {
            return context.Collections
                .Where(collection => collection.UserId == userIdentifier)
                .OrderBy(collection => collection.CreatedAt)
                .Select(collection => new ModelCollection(collection.UserId, collection.CollectionName, collection.CreatedAt, collection.CoverPicture, collection.IsPublic))
                .ToList();
        }

        public List<ModelCollection> GetLastThreeCollectionsForUser(int userIdentifier)
        {
            return context.Collections
                .Where(collection => collection.UserId == userIdentifier)
                .OrderByDescending(collection => collection.CreatedAt)
                .Take(3)
                .Select(collection => new ModelCollection(collection.UserId, collection.CollectionName, collection.CreatedAt, collection.CoverPicture, collection.IsPublic))
                .ToList();
        }

        public ModelCollection? GetCollectionById(int collectionIdentifier, int userIdentifier)
        {
            var collection = context.Collections
                .Include(collection => collection.CollectionGames)
                    .ThenInclude(collection_game => collection_game.OwnedGame)
                .FirstOrDefault(collection => collection.CollectionId == collectionIdentifier && collection.UserId == userIdentifier);

            return collection == null ? null : new ModelCollection(collection.UserId, collection.CollectionName, collection.CreatedAt, collection.CoverPicture, collection.IsPublic);
        }

        public List<ModelOwnedGame> GetGamesInCollection(int collectionIdentifier)
        {
            return context.CollectionGames
                .Where(collection_game => collection_game.CollectionId == collectionIdentifier)
                .Select(collection_game => new ModelOwnedGame
                {
                    GameId = collection_game.OwnedGame.GameId,
                    UserId = collection_game.OwnedGame.UserId,
                    GameTitle = collection_game.OwnedGame.GameTitle
                    // Add other properties as needed
                })
                .ToList();
        }

        public List<ModelOwnedGame> GetGamesInCollection(int collectionId, int userId)
        {
            return context.CollectionGames
                .Where(collection_game => collection_game.CollectionId == collectionId && collection_game.OwnedGame.UserId == userId)
                .Select(collection_game => new ModelOwnedGame
                {
                    GameId = collection_game.OwnedGame.GameId,
                    UserId = collection_game.OwnedGame.UserId,
                    GameTitle = collection_game.OwnedGame.GameTitle
                    // Add other properties as needed
                })
                .ToList();
        }

        public void AddGameToCollection(int collectionIdentifier, int gameIdentifier, int userIdentifier)
        {
            context.CollectionGames.Add(new EntityCollectionGame
            {
                CollectionId = collectionIdentifier,
                GameId = gameIdentifier
            });
            context.SaveChanges();
        }

        public void RemoveGameFromCollection(int collectionIdentifier, int gameIdentifier)
        {
            var link = context.CollectionGames.Find(collectionIdentifier, gameIdentifier);
            if (link != null)
            {
                context.CollectionGames.Remove(link);
                context.SaveChanges();
            }
        }

        public void CreateCollection(int userIdentifier, string collectionName, string? coverPicture, bool isPublic, DateOnly createdAt)
        {
            var collection = new EntityCollection
            {
                UserId = userIdentifier,
                CollectionName = collectionName,
                CreatedAt = createdAt,
                CoverPicture = coverPicture,
                IsPublic = isPublic
            };
            context.Collections.Add(collection);
            context.SaveChanges();
        }

        public void UpdateCollection(int collectionIdentifier, int userIdentifier, string collectionName, string coverPicture, bool isPublic)
        {
            var collection = context.Collections.Find(collectionIdentifier);
            if (collection != null && collection.UserId == userIdentifier)
            {
                collection.CollectionName = collectionName;
                collection.CoverPicture = coverPicture;
                collection.IsPublic = isPublic;
                collection.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
                context.SaveChanges();
            }
        }

        public void DeleteCollection(int collectionIdentifier, int userIdentifier)
        {
            var collection = context.Collections
                .FirstOrDefault(c => c.CollectionId == collectionIdentifier && c.UserId == userIdentifier);
            if (collection != null)
            {
                context.Collections.Remove(collection);
                context.SaveChanges();
            }
        }

        public List<ModelCollection> GetPublicCollectionsForUser(int userIdentifier)
        {
            return context.Collections
                .Where(collection => collection.UserId == userIdentifier && collection.IsPublic)
                .OrderBy(collection => collection.CollectionName)
      
                .Select(collection => new ModelCollection(collection.UserId, collection.CollectionName, collection.CreatedAt, collection.CoverPicture, collection.IsPublic))
                .ToList();
        }

        public List<ModelOwnedGame> GetGamesNotInCollection(int collectionIdentifier, int userIdentifier)
        {
            var inCollection = context.CollectionGames
                .Where(collection_game => collection_game.CollectionId == collectionIdentifier)
                .Select(collection_game => collection_game.GameId);

            return context.OwnedGames
                .Where(game => game.UserId == userIdentifier && !inCollection.Contains(game.GameId))
                .OrderBy(game => game.GameTitle)
                .Select(game => new ModelOwnedGame
                {
                    GameId = game.GameId,
                    UserId = game.UserId,
                    GameTitle = game.GameTitle
                    // Add other properties as needed
                })
                .ToList();
        }

        public void MakeCollectionPrivateForUser(string userId, string collectionId)
        {
            throw new NotImplementedException();
        }

        public void MakeCollectionPublicForUser(string userId, string collectionId)
        {
            throw new NotImplementedException();
        }

        public void RemoveCollectionForUser(string userId, string collectionId)
        {
            throw new NotImplementedException();
        }

        public void SaveCollection(string userId, ModelCollection collection)
        {
            throw new NotImplementedException();
        }
    }
}