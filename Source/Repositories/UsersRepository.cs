using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using BusinessLayer.Utils;
using Microsoft.Data.SqlClient;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Repositories
{
    public sealed class UsersRepository : IUsersRepository
    {
        private readonly IDataLink dataLink;

        public UsersRepository(IDataLink datalink)
        {
            this.dataLink = datalink ?? throw new ArgumentNullException(nameof(datalink));
        }

        public List<User> GetAllUsers()
        {
            try
            {
                const string sqlCommand = @"
                    SELECT
                        user_id,
                        username,
                        email,
                        developer,
                        created_at,
                        last_login
                    FROM Users
                    ORDER BY username;";

                var dataTable = dataLink.ExecuteReaderSql(sqlCommand);
                return MapDataTableToUsers(dataTable);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("Failed to retrieve users from the database.", exception);
            }
        }

        public User? GetUserById(int userId)
        {
            try
            {
                const string sqlCommand = @"
                    SELECT
                        user_id,
                        username,
                        email,
                        developer,
                        created_at,
                        last_login
                    FROM Users
                    WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                var dataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                return dataTable.Rows.Count > 0 ? MapDataRowToUser(dataTable.Rows[0]) : null;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to retrieve user with ID {userId} from the database.", exception);
            }
        }

        public User UpdateUser(User user)
        {
            try
            {
                const string sqlCommand = @"
                    UPDATE Users
                    SET
                        email = @email,
                        username = @username,
                        developer = @developer
                    WHERE user_id = @user_id;
                    
                    SELECT
                        user_id,
                        username,
                        email,
                        developer,
                        created_at,
                        last_login
                    FROM Users
                    WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", user.UserId),
                    new SqlParameter("@email", user.Email),
                    new SqlParameter("@username", user.Username),
                    new SqlParameter("@developer", user.IsDeveloper)
                };

                var dataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                if (dataTable.Rows.Count == 0)
                {
                    throw new RepositoryException($"User with ID {user.UserId} not found.");
                }

                return MapDataRowToUser(dataTable.Rows[0]);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to update user with ID {user.UserId}.", exception);
            }
        }

        public User CreateUser(User user)
        {
            try
            {
                const string sqlCommand = @"
                    INSERT INTO Users (username, email, hashed_password, developer)
                    VALUES (@username, @email, @hashed_password, @developer);
                    
                    SELECT
                        user_id,
                        username,
                        email,
                        hashed_password,
                        developer,
                        created_at,
                        last_login
                    FROM Users
                    WHERE user_id = SCOPE_IDENTITY();";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@email", user.Email),
                    new SqlParameter("@username", user.Username),
                    new SqlParameter("@hashed_password", user.Password),
                    new SqlParameter("@developer", user.IsDeveloper)
                };

                var dataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                if (dataTable.Rows.Count == 0)
                {
                    throw new RepositoryException("Failed to create user.");
                }
                return MapDataRowToUser(dataTable.Rows[0]);
            }
            catch (DatabaseOperationException exception)
            {
                Console.WriteLine($"Error creating user: {exception.Message}");
                throw new RepositoryException("Failed to create user.", exception);
            }
        }

        public void DeleteUser(int userId)
        {
            try
            {
                // First delete friendships for the user, then delete the user
                const string sqlCommand = @"
                    -- First delete friendships
                    DELETE FROM Friendships WHERE user_id1 = @user_id OR user_id2 = @user_id;
                    
                    -- Then delete the user
                    DELETE FROM Users WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to delete user with ID {userId}.", ex);
            }
        }

        public User? VerifyCredentials(string emailOrUsername)
        {
            try
            {
                const string sqlCommand = @"
                    SELECT 
                        user_id, 
                        username, 
                        email, 
                        hashed_password, 
                        developer, 
                        created_at, 
                        last_login
                    FROM Users
                    WHERE username = @EmailOrUsername OR email = @EmailOrUsername;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@EmailOrUsername", emailOrUsername),
                };

                var dataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);

                if (dataTable.Rows.Count > 0)
                {
                    var user = MapDataRowToUserWithPassword(dataTable.Rows[0]);
                    return user;
                }

                return null;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("Failed to verify user credentials.", exception);
            }
        }

        public User? GetUserByEmail(string email)
        {
            try
            {
                const string sqlCommand = @"
                    SELECT 
                        user_id, 
                        username, 
                        email, 
                        hashed_password, 
                        developer, 
                        created_at, 
                        last_login
                    FROM Users
                    WHERE email = @email;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@email", email)
                };

                var dataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                return dataTable.Rows.Count > 0 ? MapDataRowToUser(dataTable.Rows[0]) : null;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to retrieve user with email {email}.", exception);
            }
        }

        public User? GetUserByUsername(string username)
        {
            try
            {
                const string sqlCommand = @"
                    SELECT 
                        user_id, 
                        username, 
                        email, 
                        hashed_password, 
                        developer, 
                        created_at, 
                        last_login
                    FROM Users
                    WHERE username = @username;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@username", username)
                };

                var dataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                return dataTable.Rows.Count > 0 ? MapDataRowToUser(dataTable.Rows[0]) : null;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to retrieve user with username {username}.", exception);
            }
        }

        public string CheckUserExists(string email, string username)
        {
            try
            {
                const string sqlCommand = @"
                    SELECT
                        CASE
                            WHEN EXISTS (SELECT 1 FROM Users WHERE Email = @email) THEN 'EMAIL_EXISTS'
                            WHEN EXISTS (SELECT 1 FROM Users WHERE Username = @username) THEN 'USERNAME_EXISTS'
                            ELSE NULL
                        END AS ErrorType;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@email", email),
                    new SqlParameter("@username", username)
                };

                var dataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                if (dataTable.Rows.Count > 0)
                {
                    var errorType = dataTable.Rows[0]["ErrorType"];
                    return errorType == DBNull.Value ? null : errorType.ToString();
                }
                return null;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("Failed to check if user exists.", exception);
            }
        }

        public void ChangeEmail(int userId, string newEmail)
        {
            try
            {
                const string sqlCommand = @"
                    UPDATE Users
                    SET email = @newEmail
                    WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@newEmail", newEmail)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to change email for user ID {userId}.", exception);
            }
        }

        public void ChangePassword(int userId, string newPassword)
        {
            try
            {
                const string sqlCommand = @"
                    UPDATE Users 
                    SET hashed_password = @newHashedPassword 
                    WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@newHashedPassword", PasswordHasher.HashPassword(newPassword))
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to change password for user ID {userId}.", exception);
            }
        }

        public void ChangeUsername(int userId, string newUsername)
        {
            try
            {
                const string sqlCommand = @"
                    UPDATE Users 
                    SET username = @newUsername 
                    WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@newUsername", newUsername)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to change username for user ID {userId}.", exception);
            }
        }

        public void UpdateLastLogin(int userId)
        {
            try
            {
                const string sqlCommand = @"
                    UPDATE Users
                    SET last_login = GETDATE()
                    WHERE user_id = @user_id;
                    
                    SELECT
                        user_id,
                        username,
                        email,
                        developer,
                        created_at,
                        last_login
                    FROM Users
                    WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to update last login for user ID {userId}.", exception);
            }
        }

        private List<User> MapDataTableToUsers(DataTable dataTable)
        {
            return dataTable.AsEnumerable()
                .Select(MapDataRowToUser)
                .ToList();
        }

        public User? MapDataRowToUser(DataRow row)
        {
            if (row["user_id"] == DBNull.Value || row["email"] == DBNull.Value || row["username"] == DBNull.Value)
            {
                return null;
            }

            return new User
            {
                UserId = Convert.ToInt32(row["user_id"]),
                Username = row["username"].ToString(),
                Email = row["email"].ToString(),
                IsDeveloper = row["developer"] != DBNull.Value ? Convert.ToBoolean(row["developer"]) : false,
                CreatedAt = row["created_at"] != DBNull.Value ? Convert.ToDateTime(row["created_at"]) : DateTime.MinValue,
                LastLogin = row["last_login"] != DBNull.Value ? row["last_login"] as DateTime? : null
            };
        }

        public User? MapDataRowToUserWithPassword(DataRow row)
        {
            if (row["user_id"] == DBNull.Value || row["email"] == DBNull.Value || row["username"] == DBNull.Value || row["hashed_password"] == DBNull.Value)
            {
                return null;
            }

            var user = new User
            {
                UserId = Convert.ToInt32(row["user_id"]),
                Email = row["email"].ToString(),
                Username = row["username"].ToString(),
                IsDeveloper = row["developer"] != DBNull.Value ? Convert.ToBoolean(row["developer"]) : false,
                CreatedAt = row["created_at"] != DBNull.Value ? Convert.ToDateTime(row["created_at"]) : DateTime.MinValue,
                LastLogin = row["last_login"] != DBNull.Value ? row["last_login"] as DateTime? : null,
                Password = row["hashed_password"].ToString()
            };

            return user;
        }
    }
}