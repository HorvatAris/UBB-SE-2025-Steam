using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Repositories
{
    public class CollectionsRepository : ICollectionsRepository
    {
        // SQL Parameter Names
        private const string ParameterUserIdentifier = "@user_id";
        private const string ParameterCollectionIdentifierCamel = "@collectionId";   // used in GetCollectionById
        private const string ParameterCollectionIdentifierUnderscore = "@collection_id"; // used in other methods
        private const string ParameterGameIdentifier = "@game_id";
        private const string ParameterName = "@name";
        private const string ParameterCoverPicture = "@cover_picture";
        private const string ParameterIsPublic = "@is_public";
        private const string ParameterCreatedAt = "@created_at";

        // Stored Procedure Names
        private const string StoredProcedure_GetAllCollectionsForUser = "GetAllCollectionsForUser";
        private const string StoredProcedure_GetCollectionByIdentifier = "GetCollectionById";
        private const string StoredProcedure_GetGamesInCollection = "GetGamesInCollection";
        private const string StoredProcedure_GetAllGamesForUser = "GetAllGamesForUser";
        private const string StoredProcedure_AddGameToCollection = "AddGameToCollection";
        private const string StoredProcedure_RemoveGameFromCollection = "RemoveGameFromCollection";
        private const string StoredProcedure_MakeCollectionPrivate = "MakeCollectionPrivate";
        private const string StoredProcedure_MakeCollectionPublic = "MakeCollectionPublic";
        private const string StoredProcedure_DeleteCollection = "DeleteCollection";
        private const string StoredProcedure_CreateCollection = "CreateCollection";
        private const string StoredProcedure_UpdateCollection = "UpdateCollection";
        private const string StoredProcedure_GetPublicCollectionsForUser = "GetPublicCollectionsForUser";
        private const string StoredProcedure_GetGamesNotInCollection = "GetGamesNotInCollection";

        // Error messages
        private const string Error_GetCollections_DataBase = "Database error while retrieving collections.";
        private const string Error_GetCollections_Unexpected = "An unexpected error occurred while retrieving collections.";
        private const string Error_GetCollectionById_DataBase = "Database error while retrieving collection by ID.";
        private const string Error_GetCollectionById_Unexpected = "An unexpected error occurred while retrieving collection by ID.";
        private const string Error_GetGamesInCollection_DataBase = "Database error while retrieving games in collection.";
        private const string Error_GetGamesInCollection_Unexpected = "An unexpected error occurred while retrieving games in collection.";
        private const string Error_AddGameToCollection_DataBase = "Database error while adding game to collection.";
        private const string Error_AddGameToCollection_Unexpected = "An unexpected error occurred while adding game to collection.";
        private const string Error_RemoveGameFromCollection_DataBase = "Database error while removing game from collection.";
        private const string Error_RemoveGameFromCollection_Unexpected = "An unexpected error occurred while removing game from collection.";
        private const string Error_MakeCollectionPrivate = "Failed to make collection {0} private for user {1}.";
        private const string Error_MakeCollectionPublic = "Failed to make collection {0} public for user {1}.";
        private const string Error_RemoveCollection = "Failed to remove collection {0} for user {1}.";
        private const string Error_SaveCollection = "Failed to save collection for user {0}.";
        private const string Error_DeleteCollection_DataBase = "Database error while deleting collection.";
        private const string Error_DeleteCollection_Unexpected = "An unexpected error occurred while deleting collection.";
        private const string Error_CreateCollection_DataBase = "Database error while creating collection.";
        private const string Error_CreateCollection_Unexpected = "An unexpected error occurred while creating collection.";
        private const string Error_UpdateCollection_DataBase = "Database error while updating collection.";
        private const string Error_UpdateCollection_Unexpected = "An unexpected error occurred while updating collection.";
        private const string Error_GetPublicCollections_DataBase = "Database error while retrieving public collections.";
        private const string Error_GetPublicCollections_Unexpected = "An unexpected error occurred while retrieving public collections.";
        private const string Error_GetGamesNotInCollection_DataBase = "Database error while getting games not in collection.";
        private const string Error_GetGamesNotInCollection_Unexpected = "An unexpected error occurred while getting games not in collection.";

        // Column Names in DataRows
        private const string ColumnUserIdentifier = "user_id";
        private const string ColumnName = "name";
        private const string ColumnCreatedAt = "created_at";
        private const string ColumnCoverPicture = "cover_picture";
        private const string ColumnIsPublic = "is_public";
        private const string ColumnCollectionId = "collection_id";
        private const string ColumnTitle = "title";
        private const string ColumnDescription = "description";
        private const string ColumnGameId = "game_id";

        private readonly IDataLink dataLink;

        public CollectionsRepository(IDataLink dataLink)
        {
            this.dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public List<Collection> GetAllCollections(int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            SELECT collection_id, user_id, name, cover_picture, is_public, created_at
            FROM Collections
            WHERE user_id = @user_id
            ORDER BY created_at ASC;";

                var sqlParameters = new SqlParameter[]
                {
            new SqlParameter("@user_id", userIdentifier)
                };

                var resultTable = dataLink.ExecuteReaderSql(sqlCommand, sqlParameters);

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    return new List<Collection>();
                }

                var collectionsList = MapDataTableToCollections(resultTable);
                return collectionsList;
            }
            catch (DatabaseOperationException dbException)
            {
                throw new RepositoryException(Error_GetCollections_DataBase, dbException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetCollections_Unexpected, generalException);
            }
        }

        public List<Collection> GetLastThreeCollectionsForUser(int userIdentifier)
        {
            try
            {
                var allUserCollections = GetAllCollections(userIdentifier);

                var lastThreeCollections = allUserCollections
                    .OrderByDescending(collection => collection.CreatedAt)
                    .Take(3)
                    .ToList();

                return lastThreeCollections;
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving the last three collections.", generalException);
            }
        }

        public Collection GetCollectionById(int collectionIdentifier, int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            SELECT
                collection_id,
                user_id,
                name,
                cover_picture,
                is_public,
                created_at
            FROM Collections
            WHERE collection_id = @collectionId
              AND user_id = @user_id;";

                var sqlParameters = new SqlParameter[]
                {
            new SqlParameter("@collectionId", collectionIdentifier),
            new SqlParameter("@user_id", userIdentifier)
                };

                var resultTable = dataLink.ExecuteReaderSql(sqlCommand, sqlParameters);

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    return null;
                }

                return MapDataRowToCollection(resultTable.Rows[0]);
            }
            catch (DatabaseOperationException dbException)
            {
                throw new RepositoryException(Error_GetCollectionById_DataBase, dbException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetCollectionById_Unexpected, generalException);
            }
        }

        public List<OwnedGame> GetGamesInCollection(int collectionIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            IF NOT EXISTS (SELECT 1 FROM Collections WHERE collection_id = @collection_id)
            BEGIN
                RAISERROR('Collection not found', 16, 1);
                RETURN;
            END

            SELECT og.game_id, og.user_id, og.title, og.description, og.cover_picture
            FROM OwnedGames og
            INNER JOIN CollectionGames cg ON og.game_id = cg.game_id
            WHERE cg.collection_id = @collection_id;";

                var sqlParameters = new SqlParameter[]
                {
            new SqlParameter("@collection_id", collectionIdentifier)
                };

                var resultTable = dataLink.ExecuteReaderSql(sqlCommand, sqlParameters);

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    return new List<OwnedGame>();
                }

                return resultTable.AsEnumerable().Select(row =>
                {
                    var ownedGame = new OwnedGame(
                        Convert.ToInt32(row[ColumnUserIdentifier]),
                        row[ColumnTitle].ToString(),
                        row[ColumnDescription]?.ToString(),
                        row[ColumnCoverPicture]?.ToString());

                    ownedGame.GameId = Convert.ToInt32(row[ColumnGameId]);
                    return ownedGame;
                }).ToList();
            }
            catch (DatabaseOperationException dbException)
            {
                throw new RepositoryException(Error_GetGamesInCollection_DataBase, dbException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetGamesInCollection_Unexpected, generalException);
            }
        }

        public List<OwnedGame> GetGamesInCollection(int collectionIdentifier, int userIdentifier)
        {
            try
            {
                if (collectionIdentifier == 1)
                {
                    const string sqlCommand = @"
                SELECT game_id, user_id, title, description, cover_picture
                FROM OwnedGames
                WHERE user_id = @user_id
                ORDER BY title;";

                    var sqlParameters = new SqlParameter[]
                    {
                new SqlParameter("@user_id", userIdentifier)
                    };

                    var resultTable = dataLink.ExecuteReaderSql(sqlCommand, sqlParameters);

                    if (resultTable == null || resultTable.Rows.Count == 0)
                    {
                        return new List<OwnedGame>();
                    }

                    var userOwnedGames = resultTable.AsEnumerable().Select(row =>
                    {
                        var ownedGame = new OwnedGame(
                            Convert.ToInt32(row[ColumnUserIdentifier]),
                            row[ColumnTitle].ToString(),
                            row[ColumnDescription]?.ToString(),
                            row[ColumnCoverPicture]?.ToString());

                        ownedGame.GameId = Convert.ToInt32(row[ColumnGameId]);
                        return ownedGame;
                    }).ToList();

                    return userOwnedGames;
                }
                else
                {
                    return GetGamesInCollection(collectionIdentifier);
                }
            }
            catch (DatabaseOperationException dbException)
            {
                throw new RepositoryException(Error_GetGamesInCollection_DataBase, dbException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetGamesInCollection_Unexpected, generalException);
            }
        }
        public void AddGameToCollection(int collectionIdentifier, int gameIdentifier, int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
        IF NOT EXISTS (SELECT 1 FROM Collections WHERE collection_id = @collection_id)
            RAISERROR('Collection not found', 16, 1);

        IF NOT EXISTS (SELECT 1 FROM OwnedGames WHERE game_id = @game_id)
            RAISERROR('Game not found', 16, 1);

        IF EXISTS (SELECT 1 FROM OwnedGames_Collection WHERE collection_id = @collection_id AND game_id = @game_id)
            RAISERROR('Game is already in collection', 16, 1);

        INSERT INTO OwnedGames_Collection (collection_id, game_id)
        VALUES (@collection_id, @game_id);";

                var sqlParameters = new[]
                {
            new SqlParameter("@collection_id", collectionIdentifier),
            new SqlParameter("@game_id", gameIdentifier)
        };

                dataLink.ExecuteNonQuerySql(sqlCommand, sqlParameters);
            }
            catch (DatabaseOperationException dbEx)
            {
                throw new RepositoryException(Error_AddGameToCollection_DataBase, dbEx);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(Error_AddGameToCollection_Unexpected, ex);
            }
        }

        public void RemoveGameFromCollection(int collectionIdentifier, int gameIdentifier)
        {
            try
            {
                const string sqlCommand = @"
        DELETE FROM OwnedGames_Collection 
        WHERE collection_id = @collection_id AND game_id = @game_id;";

                var sqlParameters = new[]
                {
            new SqlParameter("@collection_id", collectionIdentifier),
            new SqlParameter("@game_id", gameIdentifier)
        };

                dataLink.ExecuteNonQuerySql(sqlCommand, sqlParameters);
            }
            catch (DatabaseOperationException dbEx)
            {
                throw new RepositoryException(Error_RemoveGameFromCollection_DataBase, dbEx);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(Error_RemoveGameFromCollection_Unexpected, ex);
            }
        }

        public void MakeCollectionPrivateForUser(string userIdentifier, string collectionIdentifier)
        {
            try
            {
                const string sqlCommand = @"
        UPDATE Collections
        SET is_public = 0
        WHERE collection_id = @collection_id AND user_id = @user_id;";

                var sqlParameters = new[]
                {
            new SqlParameter("@user_id", userIdentifier),
            new SqlParameter("@collection_id", collectionIdentifier)
        };

                dataLink.ExecuteNonQuerySql(sqlCommand, sqlParameters);
            }
            catch (DatabaseOperationException dbEx)
            {
                throw new RepositoryException(string.Format(Error_MakeCollectionPrivate, collectionIdentifier, userIdentifier), dbEx);
            }
        }

        public void MakeCollectionPublicForUser(string userIdentifier, string collectionIdentifier)
        {
            try
            {
                const string sqlCommand = @"
        UPDATE Collections
        SET is_public = 1
        WHERE collection_id = @collection_id AND user_id = @user_id;";

                var sqlParameters = new[]
                {
            new SqlParameter("@user_id", userIdentifier),
            new SqlParameter("@collection_id", collectionIdentifier)
        };

                dataLink.ExecuteNonQuerySql(sqlCommand, sqlParameters);
            }
            catch (DatabaseOperationException dbEx)
            {
                throw new RepositoryException(string.Format(Error_MakeCollectionPublic, collectionIdentifier, userIdentifier), dbEx);
            }
        }

        public void RemoveCollectionForUser(string userIdentifier, string collectionIdentifier)
        {
            try
            {
                const string sqlCommand = @"
        DELETE FROM OwnedGames_Collection WHERE collection_id = @collection_id;

        DELETE FROM Collections WHERE collection_id = @collection_id AND user_id = @user_id;";

                var sqlParameters = new[]
                {
            new SqlParameter("@user_id", userIdentifier),
            new SqlParameter("@collection_id", collectionIdentifier)
        };

                dataLink.ExecuteNonQuerySql(sqlCommand, sqlParameters);
            }
            catch (DatabaseOperationException dbEx)
            {
                throw new RepositoryException(string.Format(Error_RemoveCollection, collectionIdentifier, userIdentifier), dbEx);
            }
        }

        public void SaveCollection(string userIdentifier, Collection collection)
        {
            try
            {
                if (collection.CollectionId == 0)
                {
                    const string createSql = @"
                IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @user_id)
                    RAISERROR('User not found', 16, 1);

                INSERT INTO Collections (user_id, name, cover_picture, is_public, created_at)
                VALUES (@user_id, @name, @cover_picture, @is_public, @created_at);

                SELECT collection_id, user_id, name, cover_picture, is_public, created_at
                FROM Collections WHERE collection_id = SCOPE_IDENTITY();";

                    var sqlParameters = new[]
                    {
                new SqlParameter("@user_id", userIdentifier),
                new SqlParameter("@name", collection.CollectionName),
                new SqlParameter("@cover_picture", collection.CoverPicture),
                new SqlParameter("@is_public", collection.IsPublic),
                new SqlParameter("@created_at", collection.CreatedAt.ToDateTime(TimeOnly.MinValue))
            };

                    dataLink.ExecuteReaderSql(createSql, sqlParameters);
                }
                else
                {
                    const string updateSql = @"
                UPDATE Collections SET
                    name = @name,
                    cover_picture = @cover_picture,
                    is_public = @is_public,
                    created_at = @created_at
                WHERE collection_id = @collection_id AND user_id = @user_id;";

                    var sqlParameters = new[]
                    {
                new SqlParameter("@collection_id", collection.CollectionId),
                new SqlParameter("@user_id", userIdentifier),
                new SqlParameter("@name", collection.CollectionName),
                new SqlParameter("@cover_picture", collection.CoverPicture),
                new SqlParameter("@is_public", collection.IsPublic),
                new SqlParameter("@created_at", collection.CreatedAt.ToDateTime(TimeOnly.MinValue))
            };

                    dataLink.ExecuteNonQuerySql(updateSql, sqlParameters);
                }
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException(string.Format(Error_SaveCollection, userIdentifier), ex);
            }
        }

        public void DeleteCollection(int collectionIdentifier, int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            DELETE FROM OwnedGames_Collection WHERE collection_id = @collection_id;
            DELETE FROM Collections WHERE collection_id = @collection_id AND user_id = @user_id;";

                var sqlParameters = new[]
                {
            new SqlParameter("@collection_id", collectionIdentifier),
            new SqlParameter("@user_id", userIdentifier)
        };

                dataLink.ExecuteNonQuerySql(sqlCommand, sqlParameters);
            }
            catch (DatabaseOperationException dbEx)
            {
                throw new RepositoryException(Error_DeleteCollection_DataBase, dbEx);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(Error_DeleteCollection_Unexpected, ex);
            }
        }

        public void CreateCollection(int userIdentifier, string collectionName, string coverPicture, bool isPublic, DateOnly createdAt)
        {
            try
            {
                const string sqlCommand = @"
            IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @user_id)
                RAISERROR('User not found', 16, 1);

            INSERT INTO Collections (user_id, name, cover_picture, is_public, created_at)
            VALUES (@user_id, @name, @cover_picture, @is_public, @created_at);

            SELECT collection_id, user_id, name, cover_picture, is_public, created_at
            FROM Collections WHERE collection_id = SCOPE_IDENTITY();";

                var sqlParameters = new[]
                {
            new SqlParameter("@user_id", userIdentifier),
            new SqlParameter("@name", collectionName),
            new SqlParameter("@cover_picture", coverPicture),
            new SqlParameter("@is_public", isPublic),
            new SqlParameter("@created_at", createdAt.ToDateTime(TimeOnly.MinValue))
        };

                dataLink.ExecuteReaderSql(sqlCommand, sqlParameters);
            }
            catch (DatabaseOperationException dbEx)
            {
                throw new RepositoryException(Error_CreateCollection_DataBase, dbEx);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(Error_CreateCollection_Unexpected, ex);
            }
        }
        public void UpdateCollection(int collectionIdentifier, int userIdentifier, string collectionName, string coverPicture, bool isPublic)
        {
            try
            {
                const string sqlCommand = @"
            UPDATE Collections SET
                name = @name,
                cover_picture = @cover_picture,
                is_public = @is_public,
                created_at = @created_at
            WHERE collection_id = @collection_id AND user_id = @user_id;";

                var sqlParameters = new[]
                {
            new SqlParameter("@collection_id", collectionIdentifier),
            new SqlParameter("@user_id", userIdentifier),
            new SqlParameter("@name", collectionName),
            new SqlParameter("@cover_picture", coverPicture),
            new SqlParameter("@is_public", isPublic),
            new SqlParameter("@created_at", DateTime.Now.Date)
        };

                dataLink.ExecuteNonQuerySql(sqlCommand, sqlParameters);
            }
            catch (DatabaseOperationException dbEx)
            {
                throw new RepositoryException(Error_UpdateCollection_DataBase, dbEx);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(Error_UpdateCollection_Unexpected, ex);
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            SELECT collection_id, user_id, name, cover_picture, is_public, created_at
            FROM Collections
            WHERE user_id = @user_id AND is_public = 1
            ORDER BY name;";

                var sqlParameters = new[]
                {
            new SqlParameter("@user_id", userIdentifier)
        };

                var resultTable = dataLink.ExecuteReaderSql(sqlCommand, sqlParameters);

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    return new List<Collection>();
                }

                return MapDataTableToCollections(resultTable);
            }
            catch (DatabaseOperationException dbEx)
            {
                throw new RepositoryException(Error_GetPublicCollections_DataBase, dbEx);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(Error_GetPublicCollections_Unexpected, ex);
            }
        }

        public List<OwnedGame> GetGamesNotInCollection(int collectionIdentifier, int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            SELECT og.game_id, og.user_id, og.title, og.description, og.cover_picture
            FROM OwnedGames og
            WHERE og.user_id = @user_id
              AND NOT EXISTS (
                  SELECT 1 FROM OwnedGames_Collection ogc
                  WHERE ogc.game_id = og.game_id AND ogc.collection_id = @collection_id
              )
            ORDER BY og.title;";

                var sqlParameters = new[]
                {
            new SqlParameter("@collection_id", collectionIdentifier),
            new SqlParameter("@user_id", userIdentifier)
        };

                var resultTable = dataLink.ExecuteReaderSql(sqlCommand, sqlParameters);

                if (resultTable == null || resultTable.Rows.Count == 0)
                {
                    return new List<OwnedGame>();
                }

                return resultTable.AsEnumerable().Select(row => new OwnedGame(
                    Convert.ToInt32(row[ColumnUserIdentifier]),
                    row[ColumnTitle].ToString(),
                    row[ColumnDescription]?.ToString(),
                    row[ColumnCoverPicture]?.ToString())
                {
                    GameId = Convert.ToInt32(row[ColumnGameId])
                }).ToList();
            }
            catch (DatabaseOperationException dbEx)
            {
                throw new RepositoryException(Error_GetGamesNotInCollection_DataBase, dbEx);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(Error_GetGamesNotInCollection_Unexpected, ex);
            }
        }

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
    }
}
