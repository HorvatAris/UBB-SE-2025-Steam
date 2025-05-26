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

namespace BusinessLayer.Repositories
{
    public class OwnedGamesRepository : IOwnedGamesRepository
    {
        private readonly ApplicationDbContext context;

        public OwnedGamesRepository(ApplicationDbContext newContext)
        {
            this.context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public List<OwnedGame> GetAllOwnedGames(int userId)
        {
            return context.OwnedGames
                .Where(og => og.UserId == userId)
                .OrderBy(og => og.GameTitle)
                .ToList();
        }

        public OwnedGame GetOwnedGameById(int gameId, int userId)
        {
            return context.OwnedGames.SingleOrDefault(og => og.GameId == gameId && og.UserId == userId);
        }

        public void RemoveOwnedGame(int gameId, int userId)
        {
            var owned = context.OwnedGames.SingleOrDefault(og => og.GameId == gameId && og.UserId == userId);
            if (owned != null)
            {
                context.OwnedGames.Remove(owned);
                context.SaveChanges();
            }
        }

        /* This for testing purposes only */
        // TODO: Rework
        /*
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
        */
    }
}
