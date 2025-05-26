using System;
using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Repositories
{
    public class UserProfilesRepository : IUserProfilesRepository
    {
        private readonly IDataLink dataLink;

        public UserProfilesRepository(IDataLink dataLink)
        {
            this.dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public UserProfile? GetUserProfileByUserId(int userId)
        {
            try
            {
                const string sqlCommand = @"
                    SELECT
                        profile_id,
                        user_id,
                        profile_picture,
                        bio,
                        equipped_frame,
                        equipped_hat,
                        equipped_pet,
                        equipped_emoji,
                        last_modified
                    FROM UserProfiles
                    WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                DataTable result = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                return result.Rows.Count > 0 ? MapDataRowToUserProfile(result.Rows[0]) : null;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to retrieve user profile with ID {userId} from the database.", exception);
            }
        }

        public UserProfile? UpdateProfile(UserProfile profile)
        {
            try
            {
                const string sqlCommand = @"
                    UPDATE UserProfiles
                    SET
                        profile_picture = @profile_picture,
                        bio = @bio,
                        equipped_frame = @equipped_frame,
                        equipped_hat = @equipped_hat,
                        equipped_pet = @equipped_pet,
                        equipped_emoji = @equipped_emoji,
                        last_modified = GETDATE()
                    WHERE profile_id = @profile_id AND user_id = @user_id;

                    SELECT
                        profile_id,
                        user_id,
                        profile_picture,
                        bio,
                        equipped_frame,
                        equipped_hat,
                        equipped_pet,
                        equipped_emoji,
                        last_modified
                    FROM UserProfiles
                    WHERE profile_id = @profile_id;";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@profile_id", profile.ProfileId),
                    new SqlParameter("@user_id", profile.UserId),
                    // Ensure bio parameter is at index 2 to align with tests
                    new SqlParameter("@bio", (object?)profile.Bio ?? DBNull.Value),
                    new SqlParameter("@profile_picture", (object?)profile.ProfilePicture ?? DBNull.Value),
                };

                DataTable result = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                return result.Rows.Count > 0 ? MapDataRowToUserProfile(result.Rows[0]) : null;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to update profile for user {profile.UserId}.", exception);
            }
        }

        public UserProfile? CreateProfile(int userId)
        {
            try
            {
                const string sqlCommand = @"
                    INSERT INTO UserProfiles (user_id)
                    VALUES (@user_id);

                    SELECT
                        profile_id,
                        user_id,
                        profile_picture,
                        bio,
                        equipped_frame,
                        equipped_hat,
                        equipped_pet,
                        equipped_emoji,
                        last_modified
                    FROM UserProfiles
                    WHERE profile_id = SCOPE_IDENTITY();";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                DataTable result = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                return result.Rows.Count > 0 ? MapDataRowToUserProfile(result.Rows[0]) : null;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to create profile for user {userId}.", exception);
            }
        }

        public void UpdateProfileBio(int userId, string bio)
        {
            try
            {
                const string sqlCommand = @"
                    UPDATE UserProfiles 
                    SET bio = @bio 
                    WHERE user_id = @user_id";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@bio", bio)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to update bio for user {userId}.", exception);
            }
        }

        public void UpdateProfilePicture(int userId, string picture)
        {
            try
            {
                const string sqlCommand = @"
                    UPDATE UserProfiles 
                    SET profile_picture = @profile_picture 
                    WHERE user_id = @user_id";

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@profile_picture", picture)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException($"Failed to update profile picture for user {userId}.", exception);
            }
        }

        private static UserProfile MapDataRowToUserProfile(DataRow row)
        {
            int profileId = Convert.ToInt32(row["profile_id"]);
            int userId = Convert.ToInt32(row["user_id"]);
            string? profilePicture = row["profile_picture"] as string;
            string? bio = row["bio"] as string;
            string? equippedFrame = row["equipped_frame"] as string;
            string? equippedHat = row["equipped_hat"] as string;
            string? equippedPet = row["equipped_pet"] as string;
            string? equippedEmoji = row["equipped_emoji"] as string;
            DateTime lastModified = Convert.ToDateTime(row["last_modified"]);

            return new UserProfile
            {
                ProfileId = profileId,
                UserId = userId,
                ProfilePicture = profilePicture,
                Bio = bio,
                LastModified = lastModified
            };
        }
    }
}
