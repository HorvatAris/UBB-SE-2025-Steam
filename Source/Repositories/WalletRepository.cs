using System;
using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly IDataLink dataLink;

        // Define parameter names as constants
        private static class ParameterNames
        {
            public const string WalletId = "@wallet_id";
            public const string UserId = "@user_id";
            public const string Amount = "@amount";
            public const string Price = "@price";
            public const string NumberOfPoints = "@numberOfPoints";
            public const string OfferId = "@offerId";
        }

        // Define column names as constants
        private static class ColumnNames
        {
            public const string WalletId = "wallet_id";
            public const string UserId = "user_id";
            public const string Points = "points";
            public const string MoneyForGames = "money_for_games";
            public const string NumberOfPoints = "numberOfPoints";
            public const string Value = "value";
        }

        // Define error message templates
        private static class ErrorMessages
        {
            public const string NoWalletFound = "No wallet found for user ID {0}.";
            public const string FailedToRetrieveWallet = "Failed to retrieve wallet with ID {0} from the database.";
            public const string FailedToRetrieveWalletId = "Failed to retrieve wallet ID for user ID {0} from the database.";
        }

        public WalletRepository(IDataLink datalink)
        {
            this.dataLink = datalink ?? throw new ArgumentNullException(nameof(datalink));
        }

        public Wallet GetWallet(int walletId)
        {
            try
            {
                const string sqlCommand = @"
                    SELECT * 
                    FROM Wallet 
                    WHERE wallet_id = @wallet_id";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterNames.WalletId, walletId)
                };
                var dataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);

                if (dataTable.Rows.Count == 0)
                {
                    throw new RepositoryException($"Wallet with ID {walletId} not found.");
                }

                return MapDataRowToWallet(dataTable.Rows[0]);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException(
                    string.Format(ErrorMessages.FailedToRetrieveWallet, walletId),
                    exception);
            }
        }

        public int GetWalletIdByUserId(int userId)
        {
            try
            {
                const string sqlCommand = @"
                    SELECT wallet_id 
                    FROM Wallet 
                    WHERE user_id = @user_id";

                var parameters = new SqlParameter[]
                {
                     new SqlParameter(ParameterNames.UserId, userId)
                };
                var dataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);

                if (dataTable.Rows.Count > 0)
                {
                    return Convert.ToInt32(dataTable.Rows[0][ColumnNames.WalletId]);
                }
                throw new RepositoryException(string.Format(ErrorMessages.NoWalletFound, userId));
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException(
                    string.Format(ErrorMessages.FailedToRetrieveWalletId, userId),
                    exception);
            }
        }

        private Wallet MapDataRowToWallet(DataRow dataRow)
        {
            return new Wallet
            {
                WalletId = Convert.ToInt32(dataRow[ColumnNames.WalletId]),
                UserId = Convert.ToInt32(dataRow[ColumnNames.UserId]),
                Balance = Convert.ToDecimal(dataRow[ColumnNames.MoneyForGames]),
                Points = Convert.ToInt32(dataRow[ColumnNames.Points]),
            };
        }

        public void AddMoneyToWallet(decimal moneyToAdd, int userId)
        {
            try
            {
                const string sqlCommand = @"
                    UPDATE Wallet
                    SET money_for_games = money_for_games + @amount
                    WHERE user_id = @user_id";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterNames.Amount, moneyToAdd),
                    new SqlParameter(ParameterNames.UserId, userId)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to add money to wallet for user {userId}.", exception);
            }
        }

        public void AddPointsToWallet(int pointsToAdd, int userId)
        {
            try
            {
                const string sqlCommand = @"
                    UPDATE Wallet
                    SET points = points + @amount
                    WHERE user_id = @user_id";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterNames.Amount, pointsToAdd),
                    new SqlParameter(ParameterNames.UserId, userId)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to add points to wallet for user {userId}.", exception);
            }
        }

        public decimal GetMoneyFromWallet(int walletId)
        {
            return GetWallet(walletId).Balance;
        }

        public int GetPointsFromWallet(int walletId)
        {
            return GetWallet(walletId).Points;
        }

        public void PurchasePoints(PointsOffer offer, int userId)
        {
            try
            {
                const string sqlCommand = @"
                    -- Add points to the wallet
                    UPDATE Wallet
                    SET points = points + @numberOfPoints
                    WHERE user_id = @user_id;

                    -- Deduct money from the wallet
                    UPDATE Wallet
                    SET money_for_games = money_for_games - @price
                    WHERE user_id = @user_id";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterNames.Price, offer.Price),
                    new SqlParameter(ParameterNames.NumberOfPoints, offer.Points),
                    new SqlParameter(ParameterNames.UserId, userId)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to purchase points for user {userId}.", exception);
            }
        }

        public void BuyWithMoney(decimal amount, int userId)
        {
            try
            {
                const string sqlCommand = @"
                    UPDATE Wallet
                    SET money_for_games = money_for_games - @amount
                    WHERE user_id = @user_id";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterNames.Amount, amount),
                    new SqlParameter(ParameterNames.UserId, userId)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to purchase with money for user {userId}.", exception);
            }
        }

        public void BuyWithPoints(int amount, int userId)
        {
            try
            {
                const string sqlCommand = @"
                    UPDATE Wallet
                    SET points = points - @amount
                    WHERE user_id = @user_id";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterNames.Amount, amount),
                    new SqlParameter(ParameterNames.UserId, userId)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to purchase with points for user {userId}.", exception);
            }
        }

        public void AddNewWallet(int userId)
        {
            try
            {
                const string sqlCommand = @"
                    INSERT INTO Wallet (user_id, points, money_for_games)
                    VALUES (@user_id, 0, 0);
                    
                    -- Update user_id to match wallet_id for the newly created wallet
                    UPDATE Wallet
                    SET user_id = wallet_id
                    WHERE wallet_id = (SELECT MAX(wallet_id) FROM Wallet)";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterNames.UserId, userId)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public void RemoveWallet(int userId)
        {
            try
            {
                const string sqlCommand = @"
                    DELETE FROM Wallet 
                    WHERE user_id = @user_id";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterNames.UserId, userId)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public PointsOffer[] GetAllPointsOffers()
        {
            try
            {
                const string sqlCommand = "SELECT numberOfPoints, value FROM PointsOffers";

                DataTable result = dataLink.ExecuteReaderSql(sqlCommand);
                PointsOffer[] offers = new PointsOffer[result.Rows.Count];

                for (int i = 0; i < result.Rows.Count; i++)
                {
                    DataRow row = result.Rows[i];
                    offers[i] = new PointsOffer(
                        Convert.ToInt32(row[ColumnNames.Value]),
                        Convert.ToInt32(row[ColumnNames.NumberOfPoints]));
                }

                return offers;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("Failed to get points offers.", exception);
            }
        }

        public PointsOffer GetPointsOfferById(int offerId)
        {
            try
            {
                const string sqlCommand = @"
                    SELECT numberOfPoints, value 
                    FROM PointsOffers 
                    WHERE offerId = @offerId";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterNames.OfferId, offerId)
                };

                DataTable result = dataLink.ExecuteReaderSql(sqlCommand, parameters);

                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    return new PointsOffer(
                        Convert.ToInt32(row[ColumnNames.Value]),
                        Convert.ToInt32(row[ColumnNames.NumberOfPoints]));
                }

                throw new RepositoryException($"Points offer with ID {offerId} not found.");
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to get points offer with ID {offerId}.", exception);
            }
        }

        public void WinPoints(int amount, int userId)
        {
            try
            {
                const string sqlCommand = @"
                    UPDATE Wallet
                    SET points = points + @amount
                    WHERE user_id = @user_id";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterNames.Amount, amount),
                    new SqlParameter(ParameterNames.UserId, userId)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to add winning points to wallet for user {userId}.", exception);
            }
        }
    }
}