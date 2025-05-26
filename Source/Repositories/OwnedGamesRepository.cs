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
    public class OwnedGamesRepository : IOwnedGamesRepository
    {
        // SQL Parameter Names
        private const string ParameterUserId = "@user_id";
        private const string ParameterGameIdCamel = "@gameId";
        private const string ParameterUserIdCamel = "@userId";
        private const string ParameterGameIdUnderscore = "@game_id";

        // Error messages
        private const string Error_GetOwnedGamesDataBase = "Database error while retrieving owned games.";
        private const string Error_GetOwnedGamesUnexpected = "An unexpected error occurred while retrieving owned games.";
        private const string Error_GetOwnedGameByIdDataBase = "Database error while retrieving owned game by ID.";
        private const string Error_GetOwnedGameByIdUnexpected = "An unexpected error occurred while retrieving owned game by ID.";
        private const string Error_RemoveOwnedGameDataBase = "Database error while removing owned game.";
        private const string Error_RemoveOwnedGameUnexpected = "An unexpected error occurred while removing owned game.";

        // Column Names
        private const string ColumnUserId = "user_id";
        private const string ColumnTitle = "title";
        private const string ColumnDescription = "description";
        private const string ColumnCoverPicture = "cover_picture";
        private const string ColumnGameId = "game_id";

        private readonly IDataLink dataLink;

        public OwnedGamesRepository(IDataLink dataLink)
        {
            this.dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public List<OwnedGame> GetAllOwnedGames(int userId)
        {
            try
            {
                const string sqlCommand = @"
            SELECT game_id, user_id, title, description, cover_picture
            FROM OwnedGames
            WHERE user_id = @user_id
            ORDER BY title;";

                var sqlParameters = new SqlParameter[]
                {
            new SqlParameter("@user_id", userId)
                };

                var ownedGamesDataTable = dataLink.ExecuteReaderSql(sqlCommand, sqlParameters);
                var ownedGamesList = MapDataTableToOwnedGames(ownedGamesDataTable);

                return ownedGamesList;
            }
            catch (DatabaseOperationException dbException)
            {
                throw new RepositoryException(Error_GetOwnedGamesDataBase, dbException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetOwnedGamesUnexpected, generalException);
            }
        }

        public OwnedGame GetOwnedGameById(int gameId, int userId)
        {
            try
            {
                const string sqlCommand = @"
            SELECT game_id, user_id, title, description, cover_picture
            FROM OwnedGames
            WHERE game_id = @game_id AND user_id = @user_id;";

                var sqlParameters = new SqlParameter[]
                {
            new SqlParameter("@game_id", gameId),
            new SqlParameter("@user_id", userId)
                };

                var ownedGameDataTable = dataLink.ExecuteReaderSql(sqlCommand, sqlParameters);

                if (ownedGameDataTable.Rows.Count == 0)
                {
                    return null;
                }

                return MapDataRowToOwnedGame(ownedGameDataTable.Rows[0]);
            }
            catch (DatabaseOperationException dbException)
            {
                throw new RepositoryException(Error_GetOwnedGameByIdDataBase, dbException);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(Error_GetOwnedGameByIdUnexpected, ex);
            }
        }

        public void RemoveOwnedGame(int gameId, int userId)
        {
            try
            {
                const string sqlCommand = @"
            DELETE FROM OwnedGames
            WHERE game_id = @game_id AND user_id = @user_id;";

                var sqlParameters = new SqlParameter[]
                {
            new SqlParameter("@game_id", gameId),
            new SqlParameter("@user_id", userId)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, sqlParameters);
            }
            catch (DatabaseOperationException dbException)
            {
                throw new RepositoryException(Error_RemoveOwnedGameDataBase, dbException);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(Error_RemoveOwnedGameUnexpected, ex);
            }
        }

        private static List<OwnedGame> MapDataTableToOwnedGames(DataTable dataTable)
        {
            var ownedGamesList = dataTable.AsEnumerable().Select(row =>
            {
                var ownedGame = new OwnedGame(
                    Convert.ToInt32(row[ColumnUserId]),
                    row[ColumnTitle].ToString(),
                    row[ColumnDescription]?.ToString(),
                    row[ColumnCoverPicture]?.ToString());

                ownedGame.GameId = Convert.ToInt32(row[ColumnGameId]);
                return ownedGame;
            }).ToList();

            return ownedGamesList;
        }

        private static OwnedGame MapDataRowToOwnedGame(DataRow dataRow)
        {
            var ownedGame = new OwnedGame(
                Convert.ToInt32(dataRow[ColumnUserId]),
                dataRow[ColumnTitle].ToString(),
                dataRow[ColumnDescription]?.ToString(),
                dataRow[ColumnCoverPicture]?.ToString());

            ownedGame.GameId = Convert.ToInt32(dataRow[ColumnGameId]);
            return ownedGame;
        }
    }
}
