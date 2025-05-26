using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Data;
using BusinessLayer.Exceptions;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class DataLinkTests
    {
        // DataLink is a singleton with a direct database dependency.
        private DataLink dataLink;

        [SetUp]
        public void SetUp()
        {
            // Get the singleton instance for use in all tests.
            dataLink = DataLink.Instance;
        }

        [Test]
        public void Instance_ReturnsNonNullInstance()
        {
            // Act
            var instance = DataLink.Instance;
            // Assert
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void Instance_ReturnsSameInstanceOnMultipleCalls()
        {
            // Act
            var instance1 = DataLink.Instance;
            var instance2 = DataLink.Instance;
            // Assert
            Assert.That(instance2, Is.SameAs(instance1));
        }

        [Test]
        public void ExecuteScalar_GetFriendshipCountForUser_ReturnsExpectedValue()
        {
            // Assume that for user_id = 11, the seed data in your test DB indicates 2 friendships.
            string storedProcedure = "GetFriendshipCountForUser";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 11 }
            };

            // Act
            int count = dataLink.ExecuteScalar<int>(storedProcedure, parameters);
            // Assert
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void ExecuteScalar_GetFriendshipId_WithValidParameters_ReturnsExpectedValue()
        {
            // Assume that for user_id = 11 and friend_id = 12, the friendship exists with friendship_id = 1.
            string storedProcedure = "GetFriendshipId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 11 },
                new SqlParameter("@friend_id", SqlDbType.Int) { Value = 12 }
            };

            // Act
            int friendshipId = dataLink.ExecuteScalar<int>(storedProcedure, parameters);
            // Assert
            Assert.That(friendshipId, Is.EqualTo(1));
        }

        [Test]
        public void ExecuteReader_GetFriendsForUser_ReturnsDataTableWithExpectedStructure()
        {
            // For user_id = 1, expect to get the list of friendships.
            string storedProcedure = "GetFriendsForUser";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 11 }
            };

            // Act
            DataTable dt = dataLink.ExecuteReader(storedProcedure, parameters);

            // Assert that the DataTable has the correct columns and row count based on your seeded data.
            Assert.That(dt.Rows.Count, Is.EqualTo(2));
        }

        [Test]
        public void ExecuteReader_GetUserById_ReturnsUserData()
        {
            // For user_id = 1, assume the user is present and has username "User1".
            string storedProcedure = "GetUserById";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 11 }
            };

            // Act
            DataTable dt = dataLink.ExecuteReader(storedProcedure, parameters);

            // Assert that the DataTable contains the expected user data.
            Assert.That(dt.Columns.Contains("user_id"), Is.True);
        }

        [Test]
        public void ExecuteReader_GetUserProfileByUserId_ReturnsProfileData()
        {
            // For user_id = 2, assume the profile exists and the profile_picture is "alice.jpg".
            string storedProcedure = "GetUserProfileByUserId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 2 }
            };

            // Act
            DataTable dt = dataLink.ExecuteReader(storedProcedure, parameters);

            // Assert that the DataTable contains the expected profile data.
            Assert.That(dt.Rows[0]["profile_picture"].ToString(), Is.EqualTo("ms-appx:///Assets\\download.jpg"));
        }

        [Test]
        public void ExecuteNonQuery_WithInvalidProcedureName_ThrowsDatabaseOperationException()
        {
            // Using an invalid stored procedure name should throw an exception.
            string invalidProcedure = "NonExistentProcedure";

            // Act & Assert
            Assert.That(() => dataLink.ExecuteNonQuery(invalidProcedure),
                Throws.TypeOf<DatabaseOperationException>());
        }

        [Test]
        public async Task ExecuteReaderAsync_WithInvalidProcedureName_ThrowsDatabaseOperationException()
        {
            // Arrange
            string invalidProcedure = "NonExistentProcedure";

            // Act & Assert
            Assert.That(async () => await dataLink.ExecuteReaderAsync(invalidProcedure),
                Throws.TypeOf<DatabaseOperationException>());
        }

        [Test]
        public async Task ExecuteNonQueryAsync_WithInvalidProcedureName_ThrowsDatabaseOperationException()
        {
            // Arrange
            string invalidProcedure = "NonExistentProcedure";

            // Act & Assert
            Assert.That(async () => await dataLink.ExecuteNonQueryAsync(invalidProcedure),
                Throws.TypeOf<DatabaseOperationException>());
        }

        [Test]
        public void ExecuteScalar_NullResult_ReturnsDefaultValue()
        {
            // Assume that "GetFriendshipId" returns NULL (or no result) when the friendship does not exist.
            // For user_id = 1 and friend_id = 999 (non-existent friend), expect default (0 or null)
            string storedProcedure = "GetFriendshipId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 1 },
                new SqlParameter("@friend_id", SqlDbType.Int) { Value = 999 }
            };

            // Act
            int? result = dataLink.ExecuteScalar<int?>(storedProcedure, parameters);

            // Assert - expecting a null result.
            Assert.That(result, Is.Null);
        }

        [Test]
        public void ExecuteReader_GetAllCollectionsForUser_ReturnsAllCollections()
        {
            // For user_id = 1, assume there are 4 collections.
            string storedProcedure = "GetAllCollectionsForUser";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 1 }
            };

            // Act
            DataTable dt = dataLink.ExecuteReader(storedProcedure, parameters);
            // Assert
            Assert.That(dt.Columns.Contains("collection_id"), Is.True);
            Assert.That(dt.Columns.Contains("name"), Is.True);
            Assert.That(dt.Rows.Count, Is.EqualTo(4));
        }

        [Test]
        public void ExecuteReader_GetPublicCollectionsForUser_ReturnsOnlyPublicCollections()
        {
            // For user_id = 1, assume only 3 of the seeded collections are public.
            string storedProcedure = "GetPublicCollectionsForUser";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 1 }
            };

            // Act
            DataTable dt = dataLink.ExecuteReader(storedProcedure, parameters);
            // Assert
            Assert.That(dt.Columns.Contains("collection_id"), Is.True);
            Assert.That(dt.Columns.Contains("is_public"), Is.True);
            Assert.That(dt.Rows.Count, Is.EqualTo(3));
        }

        [Test]
        public void ExecuteReader_GetGamesInCollection_ReturnsGamesInCollection()
        {
            // For collection_id = 1, assume there are 0 games.
            string storedProcedure = "GetGamesInCollection";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@collection_id", SqlDbType.Int) { Value = 1 }
            };

            // Act
            DataTable dt = dataLink.ExecuteReader(storedProcedure, parameters);
            // Assert
            Assert.That(dt.Columns.Contains("game_id"), Is.True);
            Assert.That(dt.Columns.Contains("title"), Is.True);
            Assert.That(dt.Rows.Count, Is.EqualTo(0));
        }

        [Test]
        public void ExecuteReader_GetGamesNotInCollection_ReturnsGamesNotInSpecifiedCollection()
        {
            // For user_id = 1 and collection_id = 1, assume there is not a game not in that collection.
            string storedProcedure = "GetGamesNotInCollection";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@collection_id", SqlDbType.Int) { Value = 1 },
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 1 }
            };

            // Act
            DataTable dt = dataLink.ExecuteReader(storedProcedure, parameters);
            // Assert
            Assert.That(dt.Columns.Contains("game_id"), Is.True);
            Assert.That(dt.Rows.Count, Is.EqualTo(0));
        }
    }
}
