using System;
using System.Data;
using BusinessLayer.Data;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly IDataLink dataLink;

        public PasswordResetRepository(IDataLink dataLink)
        {
            this.dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public void StoreResetCode(int userId, string code, DateTime expiryTime)
        {
            try
            {
                // First, delete any existing reset codes for this user
                const string deleteCommand = @"
                    DELETE FROM PasswordResetCodes
                    WHERE user_id = @userId";

                var deleteParameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userId)
                };

                dataLink.ExecuteNonQuerySql(deleteCommand, deleteParameters);

                // Then store the new reset code
                const string insertCommand = @"
                    INSERT INTO PasswordResetCodes (user_id, reset_code, expiration_time)
                    VALUES (@userId, @resetCode, @expirationTime)";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@resetCode", code),
                    new SqlParameter("@expirationTime", expiryTime)
                };

                dataLink.ExecuteNonQuerySql(insertCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to store reset code for user {userId}.", exception);
            }
        }

        public bool VerifyResetCode(string email, string code)
        {
            try
            {
                const string sqlCommand = @"
                    DECLARE @userId int
                    SELECT @userId = user_id FROM Users WHERE email = @email

                    SELECT 
                        p.user_id,
                        p.reset_code,
                        p.expiration_time,
                        p.used
                    FROM PasswordResetCodes p
                    WHERE p.user_id = @userId 
                    AND p.reset_code = @resetCode";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@email", email),
                    new SqlParameter("@resetCode", code)
                };

                // Get the reset code data from database
                DataTable result = dataLink.ExecuteReaderSql(sqlCommand, parameters);

                // Check if the reset code exists and is valid
                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    DateTime expirationTime = (DateTime)row["expiration_time"];
                    bool isCodeUsed = (bool)row["used"];

                    // Check if code is valid, unexpired, and unused
                    return !isCodeUsed && expirationTime > DateTime.UtcNow;
                }

                return false;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("Failed to verify reset code.", exception);
            }
        }

        public bool ResetPassword(string email, string code, string hashedPassword)
        {
            try
            {
                const string sqlCommand = @"
                    BEGIN TRANSACTION
                    
                    DECLARE @userId int
                    SELECT @userId = user_id FROM Users WHERE email = @email
                    
                    DECLARE @success bit = 0
                    
                    IF EXISTS (
                        SELECT 1 
                        FROM PasswordResetCodes 
                        WHERE user_id = @userId 
                        AND reset_code = @resetCode 
                        AND expiration_time > GETUTCDATE()
                        AND used = 0
                    )
                    BEGIN
                        UPDATE Users
                        SET hashed_password = @newPassword
                        WHERE user_id = @userId
                        
                        -- Delete the used reset code
                        DELETE FROM PasswordResetCodes
                        WHERE user_id = @userId
                        AND reset_code = @resetCode
                        
                        SET @success = 1
                    END
                    
                    IF @success = 1
                        COMMIT
                    ELSE
                        ROLLBACK
                        
                    SELECT @success as Result";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@email", email),
                    new SqlParameter("@resetCode", code),
                    new SqlParameter("@newPassword", hashedPassword)
                };

                DataTable result = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                return result.Rows.Count > 0 && Convert.ToBoolean(result.Rows[0]["Result"]);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("Failed to reset password.", exception);
            }
        }

        public bool ValidateResetCode(string email, string code)
        {
            try
            {
                const string sqlCommand = @"
                    DECLARE @isValid BIT = 0;
                    
                    -- Check if the code exists, is not used, and hasn't expired
                    IF EXISTS (
                        SELECT 1 
                        FROM PasswordResetCodes p
                        JOIN Users u ON p.user_id = u.user_id
                        WHERE u.email = @email 
                        AND p.reset_code = @resetCode 
                        AND p.used = 0 
                        AND p.expiration_time > GETDATE()
                    )
                    BEGIN
                        -- Mark the code as used
                        UPDATE PasswordResetCodes
                        SET used = 1
                        FROM PasswordResetCodes p
                        JOIN Users u ON p.user_id = u.user_id
                        WHERE u.email = @email
                        AND p.reset_code = @resetCode;
                        
                        SET @isValid = 1;
                    END
                    
                    SELECT @isValid AS isValid;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@email", email),
                    new SqlParameter("@resetCode", code)
                };

                DataTable result = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                return result.Rows.Count > 0 && Convert.ToBoolean(result.Rows[0]["isValid"]);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("Failed to validate reset code.", exception);
            }
        }

        public void CleanupExpiredCodes()
        {
            try
            {
                const string sqlCommand = @"
                    -- Delete expired codes
                    DELETE FROM PasswordResetCodes
                    WHERE expiration_time < GETUTCDATE()";

                dataLink.ExecuteNonQuerySql(sqlCommand);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("Failed to cleanup expired reset codes.", exception);
            }
        }
    }
}