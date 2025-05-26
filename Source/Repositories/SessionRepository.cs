using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly IDataLink dataLink;

        public SessionRepository(IDataLink dataLink)
        {
            this.dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public SessionDetails CreateSession(int userId)
        {
            try
            {
                const string sqlCommand = @"
                    -- Delete any existing sessions for this user
                    DELETE FROM UserSessions WHERE user_id = @user_id;

                    -- Create new session with 2-hour expiration
                    INSERT INTO UserSessions (user_id, session_id, created_at, expires_at)
                    VALUES (
                        @user_id,
                        NEWID(),
                        GETDATE(),
                        DATEADD(HOUR, 2, GETDATE())
                    );

                    -- Return the session details
                    SELECT
                        us.session_id,
                        us.user_id,
                        us.created_at,
                        us.expires_at
                    FROM UserSessions us
                    WHERE us.user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                DataTable result = dataLink.ExecuteReaderSql(sqlCommand, parameters);

                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    return new SessionDetails
                    {
                        SessionId = (Guid)row["session_id"],
                        UserId = (int)row["user_id"],
                        CreatedAt = (DateTime)row["created_at"],
                        ExpiresAt = (DateTime)row["expires_at"]
                    };
                }

                throw new RepositoryException("Failed to create session");
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to create session for user ID {userId}.", exception);
            }
        }

        public void DeleteUserSessions(int userId)
        {
            try
            {
                const string sqlCommand = @"
                    DELETE FROM UserSessions 
                    WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to delete sessions for user ID {userId}.", exception);
            }
        }

        public void DeleteSession(Guid sessionId)
        {
            try
            {
                const string sqlCommand = @"
                    DELETE FROM UserSessions 
                    WHERE session_id = @session_id;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@session_id", sessionId)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to delete session with ID {sessionId}.", exception);
            }
        }

        public SessionDetails GetSessionById(Guid sessionId)
        {
            try
            {
                const string sqlCommand = @"
                    SELECT 
                        session_id, 
                        user_id, 
                        created_at, 
                        expires_at
                    FROM UserSessions
                    WHERE session_id = @session_id;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@session_id", sessionId)
                };

                DataTable result = dataLink.ExecuteReaderSql(sqlCommand, parameters);

                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    return new SessionDetails
                    {
                        SessionId = (Guid)row["session_id"],
                        UserId = (int)row["user_id"],
                        CreatedAt = (DateTime)row["created_at"],
                        ExpiresAt = (DateTime)row["expires_at"]
                    };
                }

                return null;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to get session with ID {sessionId}.", exception);
            }
        }

        public UserWithSessionDetails GetUserFromSession(Guid sessionId)
        {
            try
            {
                const string sqlCommand = @"
                    -- Check if session exists and is not expired
                    IF EXISTS (
                        SELECT 1 
                        FROM UserSessions 
                        WHERE session_id = @session_id 
                        AND expires_at > GETDATE()
                    )
                    BEGIN
                        -- Return user details
                        SELECT
                            us.session_id,
                            us.created_at,
                            us.expires_at,
                            u.user_id,
                            u.username,
                            u.email,
                            u.developer,
                            u.created_at,
                            u.last_login
                        FROM UserSessions us
                        JOIN Users u ON us.user_id = u.user_id
                        WHERE us.session_id = @session_id;
                    END
                    ELSE
                    BEGIN
                        -- If session is expired or doesn't exist, delete it
                        DELETE FROM UserSessions WHERE session_id = @session_id;
                        SELECT TOP 0 * FROM Users; -- Return empty result set
                    END";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@session_id", sessionId)
                };

                DataTable result = dataLink.ExecuteReaderSql(sqlCommand, parameters);

                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    return new UserWithSessionDetails
                    {
                        SessionId = sessionId,
                        CreatedAt = (DateTime)row["created_at"],
                        ExpiresAt = (DateTime)row["expires_at"],
                        UserId = (int)row["user_id"],
                        Username = (string)row["username"],
                        Email = (string)row["email"],
                        Developer = (bool)row["developer"],
                        UserCreatedAt = (DateTime)row["created_at"],
                        LastLogin = row["last_login"] == DBNull.Value ? null : (DateTime?)row["last_login"]
                    };
                }

                return null;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to get user from session with ID {sessionId}.", exception);
            }
        }

        public List<Guid> GetExpiredSessions()
        {
            try
            {
                const string sqlCommand = @"
                    SELECT session_id
                    FROM UserSessions
                    WHERE expires_at < GETDATE();";

                DataTable result = dataLink.ExecuteReaderSql(sqlCommand);
                var expiredSessions = new List<Guid>();

                foreach (DataRow row in result.Rows)
                {
                    Guid sessionId = (Guid)row["session_id"];
                    expiredSessions.Add(sessionId);
                }

                return expiredSessions;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("Failed to get expired sessions.", exception);
            }
        }

        public void CleanupExpiredSessions()
        {
            try
            {
                const string sqlCommand = @"
                    DELETE FROM UserSessions 
                    WHERE expires_at < GETDATE();";

                dataLink.ExecuteNonQuerySql(sqlCommand);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("Failed to clean up expired sessions.", exception);
            }
        }
    }
}