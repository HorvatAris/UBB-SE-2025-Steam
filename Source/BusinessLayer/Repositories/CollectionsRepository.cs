using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;
using BusinessLayer.DataContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Repositories
{
    public class CollectionsRepository : ICollectionsRepository
    {
        private readonly ApplicationDbContext context;

        public CollectionsRepository(ApplicationDbContext newContext)
        {
            this.context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public List<Collection> GetAllCollections(int userIdentifier)
        {
            return context.Collections
                      .Where(c => c.UserId == userIdentifier)
                      .OrderBy(c => c.CreatedAt)
                      .ToList();
        }

        public List<Collection> GetLastThreeCollectionsForUser(int userIdentifier)
        {
            return GetAllCollections(userIdentifier)
                   .OrderByDescending(c => c.CreatedAt)
                   .Take(3)
                   .ToList();
        }

        public Collection? GetCollectionById(int collectionIdentifier, int userIdentifier)
        {
            return context.Collections
                      .Include(c => c.CollectionGames)
                        .ThenInclude(cg => cg.OwnedGame)
                      .FirstOrDefault(c => c.CollectionId == collectionIdentifier
                                        && c.UserId == userIdentifier);
        }

        public List<OwnedGame> GetGamesInCollection(int collectionIdentifier)
        {
            return context.CollectionGames
                      .Where(cg => cg.CollectionId == collectionIdentifier)
                      .Select(cg => cg.OwnedGame)
                      .ToList();
        }

        public void AddGameToCollection(int collectionIdentifier, int gameIdentifier, int userIdentifier)
        {
            // Optional: validate existence of both entities first
            context.CollectionGames.Add(new CollectionGame
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
            var collection = new Collection(userIdentifier, collectionName, createdAt, coverPicture, isPublic);
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
                                .FirstOrDefault(c => c.CollectionId == collectionIdentifier
                                            && c.UserId == userIdentifier);
            if (collection != null)
            {
                // remove all links first
                context.Collections.Remove(collection);
                context.SaveChanges();
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userIdentifier)
        {
            return context.Collections
                      .Where(c => c.UserId == userIdentifier && c.IsPublic)
                      .OrderBy(c => c.CollectionName)
                      .ToList();
        }

        public List<OwnedGame> GetGamesNotInCollection(int collectionIdentifier, int userIdentifier)
        {
            var inCollection = context.CollectionGames
                                  .Where(cg => cg.CollectionId == collectionIdentifier)
                                  .Select(cg => cg.GameId);

            return context.OwnedGames
                      .Where(g => g.UserId == userIdentifier && !inCollection.Contains(g.GameId))
                      .OrderBy(g => g.GameTitle)
                      .ToList();
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId, int userId)
        {
            throw new NotImplementedException();
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

        public void SaveCollection(string userId, Collection collection)
        {
            throw new NotImplementedException();
        }

        /* // Used for old tests
        private static List<Collection> MapDataTableToCollections(DataTable dataTable)
        {
            var collectionList = dataTable.AsEnumerable().Select(row =>
            {
                var collection = new Collection(
                    userId: Convert.ToInt32(row[ColumnUserIdentifier]),
                    collectionName: row[ColumnName].ToString(),
                    createdAt: DateOnly.FromDateTime(Convert.ToDateTime(row[ColumnCreatedAt])),
                    coverPicture: row[ColumnCoverPicture]?.ToString(),
                    isPublic: Convert.ToBoolean(row[ColumnIsPublic]));

                collection.CollectionId = Convert.ToInt32(row[ColumnCollectionId]);
                return collection;
            }).ToList();

            return collectionList;
        }

        private static Collection MapDataRowToCollection(DataRow dataRow)
        {
            var collection = new Collection(
                userId: Convert.ToInt32(dataRow[ColumnUserIdentifier]),
                collectionName: dataRow[ColumnName].ToString(),
                createdAt: DateOnly.FromDateTime(Convert.ToDateTime(dataRow[ColumnCreatedAt])),
                coverPicture: dataRow[ColumnCoverPicture]?.ToString(),
                isPublic: Convert.ToBoolean(dataRow[ColumnIsPublic]));

            collection.CollectionId = Convert.ToInt32(dataRow[ColumnCollectionId]);
            return collection;
        }
        */
    }
}
