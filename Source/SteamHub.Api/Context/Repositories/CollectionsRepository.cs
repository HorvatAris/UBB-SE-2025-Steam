﻿using System;
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

        public async Task<List<ModelCollection>> GetAllCollectionsAsync(int userIdentifier)
        {
            return await context.Collections
                .Where(c => c.UserId == userIdentifier)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new ModelCollection(c.UserId, c.CollectionName, c.CreatedAt, c.CoverPicture, c.IsPublic))
                .ToListAsync();
        }

        public async Task<List<ModelCollection>> GetLastThreeCollectionsForUserAsync(int userIdentifier)
        {
            return await context.Collections
                .Where(c => c.UserId == userIdentifier)
                .OrderByDescending(c => c.CreatedAt)
                .Take(3)
                .Select(c => new ModelCollection(c.UserId, c.CollectionName, c.CreatedAt, c.CoverPicture, c.IsPublic))
                .ToListAsync();
        }

        public async Task<ModelCollection?> GetCollectionByIdAsync(int collectionIdentifier, int userIdentifier)
        {
            var c = await context.Collections
                .Include(col => col.CollectionGames)
                    .ThenInclude(cg => cg.OwnedGame)
                .FirstOrDefaultAsync(col => col.CollectionId == collectionIdentifier && col.UserId == userIdentifier);

            return c == null ? null : new ModelCollection(c.UserId, c.CollectionName, c.CreatedAt, c.CoverPicture, c.IsPublic);
        }


        public async Task<List<ModelOwnedGame>> GetGamesInCollectionAsync(int collectionIdentifier)
        {
            return await context.CollectionGames
                .Where(cg => cg.CollectionId == collectionIdentifier)
                .Select(cg => new ModelOwnedGame
                {
                    GameId = cg.OwnedGame.GameId,
                    UserId = cg.OwnedGame.UserId,
                    GameTitle = cg.OwnedGame.GameTitle
                })
                .ToListAsync();
        }

        public async Task<List<ModelOwnedGame>> GetGamesInCollectionAsync(int collectionId, int userId)
        {
            return await context.CollectionGames
                .Where(cg => cg.CollectionId == collectionId && cg.OwnedGame.UserId == userId)
                .Select(cg => new ModelOwnedGame
                {
                    GameId = cg.OwnedGame.GameId,
                    UserId = cg.OwnedGame.UserId,
                    GameTitle = cg.OwnedGame.GameTitle
                })
                .ToListAsync();
        }

        public async Task AddGameToCollectionAsync(int collectionIdentifier, int gameIdentifier)
        {
            context.CollectionGames.Add(new EntityCollectionGame
            {
                CollectionId = collectionIdentifier,
                GameId = gameIdentifier
            });
            await context.SaveChangesAsync();
        }

        public async Task RemoveGameFromCollectionAsync(int collectionIdentifier, int gameIdentifier)
        {
            var link = await context.CollectionGames.FindAsync(collectionIdentifier, gameIdentifier);
            if (link != null)
            {
                context.CollectionGames.Remove(link);
                await context.SaveChangesAsync();
            }
        }

        public async Task CreateCollectionAsync(int userIdentifier, string collectionName, string? coverPicture, bool isPublic, DateOnly createdAt)
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
            await context.SaveChangesAsync();
        }

        public async Task UpdateCollectionAsync(int collectionIdentifier, int userIdentifier, string collectionName, string coverPicture, bool isPublic)
        {
            var collection = await context.Collections.FindAsync(collectionIdentifier);
            if (collection != null && collection.UserId == userIdentifier)
            {
                collection.CollectionName = collectionName;
                collection.CoverPicture = coverPicture;
                collection.IsPublic = isPublic;
                collection.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteCollectionAsync(int collectionIdentifier, int userIdentifier)
        {
            var collection = await context.Collections
                .FirstOrDefaultAsync(c => c.CollectionId == collectionIdentifier && c.UserId == userIdentifier);
            if (collection != null)
            {
                context.Collections.Remove(collection);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<ModelCollection>> GetPublicCollectionsForUserAsync(int userIdentifier)
        {
            return await context.Collections
                .Where(c => c.UserId == userIdentifier && c.IsPublic)
                .OrderBy(c => c.CollectionName)
                .Select(c => new ModelCollection(c.UserId, c.CollectionName, c.CreatedAt, c.CoverPicture, c.IsPublic))
                .ToListAsync();
        }

        public async Task<List<ModelOwnedGame>> GetGamesNotInCollectionAsync(int collectionIdentifier, int userIdentifier)
        {
            var inCollection = context.CollectionGames
                .Where(cg => cg.CollectionId == collectionIdentifier)
                .Select(cg => cg.GameId);

            return await context.OwnedGames
                .Where(g => g.UserId == userIdentifier && !inCollection.Contains(g.GameId))
                .OrderBy(g => g.GameTitle)
                .Select(g => new ModelOwnedGame
                {
                    GameId = g.GameId,
                    UserId = g.UserId,
                    GameTitle = g.GameTitle
                })
                .ToListAsync();
        }

        // Not implemented methods stay the same for now
        public void MakeCollectionPrivateForUser(string userId, string collectionId) => throw new NotImplementedException();
        public void MakeCollectionPublicForUser(string userId, string collectionId) => throw new NotImplementedException();
        public void RemoveCollectionForUser(string userId, string collectionId) => throw new NotImplementedException();
        public void SaveCollection(string userId, ModelCollection collection) => throw new NotImplementedException();
    }
}