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
                .Where(c => c.UserId == userIdentifier)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new ModelCollection(c.UserId, c.CollectionName, c.CreatedAt, c.CoverPicture, c.IsPublic))
                .ToList();
        }

        public List<ModelCollection> GetLastThreeCollectionsForUser(int userIdentifier)
        {
            return context.Collections
                .Where(c => c.UserId == userIdentifier)
                .OrderByDescending(c => c.CreatedAt)
                .Take(3)
                .Select(c => new ModelCollection(c.UserId, c.CollectionName, c.CreatedAt, c.CoverPicture, c.IsPublic))
                .ToList();
        }

        public ModelCollection? GetCollectionById(int collectionIdentifier, int userIdentifier)
        {
            var c = context.Collections
                .Include(col => col.CollectionGames)
                    .ThenInclude(cg => cg.OwnedGame)
                .FirstOrDefault(col => col.CollectionId == collectionIdentifier && col.UserId == userIdentifier);

            return c == null ? null : new ModelCollection(c.UserId, c.CollectionName, c.CreatedAt, c.CoverPicture, c.IsPublic))
        }

        public List<ModelOwnedGame> GetGamesInCollection(int collectionIdentifier)
        {
            return context.CollectionGames
                .Where(cg => cg.CollectionId == collectionIdentifier)
                .Select(cg => new ModelOwnedGame
                {
                    GameId = cg.OwnedGame.GameId,
                    UserId = cg.OwnedGame.UserId,
                    GameTitle = cg.OwnedGame.GameTitle
                    // Add other properties as needed
                })
                .ToList();
        }

        public List<ModelOwnedGame> GetGamesInCollection(int collectionId, int userId)
        {
            return context.CollectionGames
                .Where(cg => cg.CollectionId == collectionId && cg.OwnedGame.UserId == userId)
                .Select(cg => new ModelOwnedGame
                {
                    GameId = cg.OwnedGame.GameId,
                    UserId = cg.OwnedGame.UserId,
                    GameTitle = cg.OwnedGame.GameTitle
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
                .Where(c => c.UserId == userIdentifier && c.IsPublic)
                .OrderBy(c => c.CollectionName)
      
                .Select(c => new ModelCollection(c.UserId, c.CollectionName, c.CreatedAt, c.CoverPicture, c.IsPublic))
                .ToList();
        }

        public List<ModelOwnedGame> GetGamesNotInCollection(int collectionIdentifier, int userIdentifier)
        {
            var inCollection = context.CollectionGames
                .Where(cg => cg.CollectionId == collectionIdentifier)
                .Select(cg => cg.GameId);

            return context.OwnedGames
                .Where(g => g.UserId == userIdentifier && !inCollection.Contains(g.GameId))
                .OrderBy(g => g.GameTitle)
                .Select(g => new ModelOwnedGame
                {
                    GameId = g.GameId,
                    UserId = g.UserId,
                    GameTitle = g.GameTitle
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