using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories
{
    public class FriendshipsRepository : IFriendshipsRepository
    {
        // Error messages
        private const string Error_GetFriendshipsDataBase = "Database error while retrieving friendships.";
        private const string Error_GetFriendshipsUnexpected = "An unexpected error occurred while retrieving friendships.";
        private const string Error_AddFriendshipDataBase = "Database error while adding friendship.";
        private const string Error_AddFriendshipUnexpected = "An unexpected error occurred while adding friendship.";
        private const string Error_UserDoesNotExist = "User with ID {0} does not exist.";
        private const string Error_FriendshipAlreadyExists = "Friendship already exists.";
        private const string Error_GetFriendshipByIdentifierDataBase = "Database error while retrieving friendship by ID.";
        private const string Error_GetFriendshipByIdentifierUnexpected = "An unexpected error occurred while retrieving friendship by ID.";
        private const string Error_RemoveFriendshipDataBase = "Database error while removing friendship.";
        private const string Error_RemoveFriendshipUnexpected = "An unexpected error occurred while removing friendship.";
        private const string Error_GetFriendshipCountDataBase = "Database error while retrieving friendship count.";
        private const string Error_GetFriendshipCountUnexpected = "An unexpected error occurred while retrieving friendship count.";
        private const string Error_GetFriendshipIdentifierDataBase = "Database error while retrieving friendship ID.";
        private const string Error_GetFriendshipIdentifierUnexpected = "An unexpected error occurred while retrieving friendship ID.";

        private readonly IDataLink dataLink;

        public FriendshipsRepository(IDataLink dataLink)
        {
            this.dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public List<Friendship> GetAllFriendships(int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
                    SELECT
                        f.friendship_id,
                        f.user_id,
                        f.friend_id,
                        u.username as friend_username,
                        p.profile_picture as friend_profile_picture
                    FROM Friendships f
                    JOIN Users u ON f.friend_id = u.user_id
                    JOIN UserProfiles p ON p.user_id = f.friend_id
                    WHERE f.user_id = @user_id
                    ORDER BY u.username";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userIdentifier)
                };

                var friendshipDataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);

                var listOfFriendships = new List<Friendship>();
                foreach (DataRow friendshipDataRow in friendshipDataTable.Rows)
                {
                    var friendship = new Friendship(
                        friendshipId: Convert.ToInt32(friendshipDataRow["friendship_id"]),
                        userId: Convert.ToInt32(friendshipDataRow["user_id"]),
                        friendId: Convert.ToInt32(friendshipDataRow["friend_id"]))
                    {
                        FriendUsername = friendshipDataRow["friend_username"].ToString(),
                        FriendProfilePicture = friendshipDataRow["friend_profile_picture"].ToString()
                    };

                    listOfFriendships.Add(friendship);
                }

                return listOfFriendships;
            }
            catch (DatabaseOperationException sqlException)
            {
                throw new RepositoryException(Error_GetFriendshipsDataBase, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetFriendshipsUnexpected, generalException);
            }
        }

        public void AddFriendship(int userIdentifier, int friendUserIdentifier)
        {
            try
            {
                // Check if user exists
                const string checkUserSql = @"
                    SELECT COUNT(*) FROM Users WHERE user_id = @user_id";

                var userExistenceParameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userIdentifier)
                };

                var userCount = dataLink.ExecuteScalarSql<int>(checkUserSql, userExistenceParameters);
                if (userCount == 0)
                {
                    throw new RepositoryException(string.Format(Error_UserDoesNotExist, userIdentifier));
                }

                // Check if friend exists
                var friendExistenceParameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", friendUserIdentifier)
                };

                var friendCount = dataLink.ExecuteScalarSql<int>(checkUserSql, friendExistenceParameters);
                if (friendCount == 0)
                {
                    throw new RepositoryException(string.Format(Error_UserDoesNotExist, friendUserIdentifier));
                }

                // Check if friendship already exists
                const string checkFriendshipSql = @"
                    SELECT COUNT(*) FROM Friendships 
                    WHERE user_id = @user_id AND friend_id = @friend_id";

                var checkFriendshipParameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userIdentifier),
                    new SqlParameter("@friend_id", friendUserIdentifier)
                };

                var friendshipCount = dataLink.ExecuteScalarSql<int>(checkFriendshipSql, checkFriendshipParameters);
                if (friendshipCount > 0)
                {
                    throw new RepositoryException(Error_FriendshipAlreadyExists);
                }

                // Add friendship
                const string addFriendshipSql = @"
                    INSERT INTO Friendships (user_id, friend_id)
                    VALUES (@user_id, @friend_id)";

                var addFriendshipParameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userIdentifier),
                    new SqlParameter("@friend_id", friendUserIdentifier)
                };

                dataLink.ExecuteNonQuerySql(addFriendshipSql, addFriendshipParameters);
            }
            catch (DatabaseOperationException sqlException)
            {
                throw new RepositoryException(Error_AddFriendshipDataBase, sqlException);
            }
            catch (RepositoryException)
            {
                throw;
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_AddFriendshipUnexpected, generalException);
            }
        }

        public Friendship GetFriendshipById(int friendshipIdentifier)
        {
            try
            {
                const string sqlCommand = @"
                    SELECT 
                        friendship_id,
                        user_id,
                        friend_id
                    FROM Friendships
                    WHERE friendship_id = @friendship_id";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@friendship_id", friendshipIdentifier)
                };

                var friendshipDataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);

                return friendshipDataTable.Rows.Count > 0
                    ? MapDataRowToFriendship(friendshipDataTable.Rows[0])
                    : null;
            }
            catch (DatabaseOperationException sqlException)
            {
                throw new RepositoryException(Error_GetFriendshipByIdentifierDataBase, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetFriendshipByIdentifierUnexpected, generalException);
            }
        }

        public void RemoveFriendship(int friendshipIdentifier)
        {
            try
            {
                const string sqlCommand = @"
                    DELETE FROM Friendships
                    WHERE friendship_id = @friendship_id";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@friendship_id", friendshipIdentifier)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException sqlException)
            {
                throw new RepositoryException(Error_RemoveFriendshipDataBase, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_RemoveFriendshipUnexpected, generalException);
            }
        }

        public int GetFriendshipCount(int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
                    SELECT COUNT(*) 
                    FROM Friendships
                    WHERE user_id = @user_id";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userIdentifier)
                };

                return dataLink.ExecuteScalarSql<int>(sqlCommand, parameters);
            }
            catch (DatabaseOperationException sqlException)
            {
                throw new RepositoryException(Error_GetFriendshipCountDataBase, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetFriendshipCountUnexpected, generalException);
            }
        }

        public int? GetFriendshipId(int userIdentifier, int friendIdentifier)
        {
            try
            {
                const string sqlCommand = @"
                    SELECT friendship_id
                    FROM Friendships
                    WHERE user_id = @user_id AND friend_id = @friend_id";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userIdentifier),
                    new SqlParameter("@friend_id", friendIdentifier)
                };

                return dataLink.ExecuteScalarSql<int?>(sqlCommand, parameters);
            }
            catch (DatabaseOperationException sqlException)
            {
                throw new RepositoryException(Error_GetFriendshipIdentifierDataBase, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetFriendshipIdentifierUnexpected, generalException);
            }
        }

        private static Friendship MapDataRowToFriendship(DataRow friendshipDataRow)
        {
            return new Friendship(
                friendshipId: Convert.ToInt32(friendshipDataRow["friendship_id"]),
                userId: Convert.ToInt32(friendshipDataRow["user_id"]),
                friendId: Convert.ToInt32(friendshipDataRow["friend_id"]));
        }
    }
}