using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using Microsoft.Data.SqlClient;
using Moq;

namespace Tests.RepositoryTests
{
    /*
    [TestFixture]
    internal class UserProfileRepositoryTests
    {
        private UserProfilesRepository userProfileRepository;
        private Mock<IDataLink> mockDataLink;

        [SetUp]
        public void Setup()
        {
            mockDataLink = new Mock<IDataLink>();
            userProfileRepository = new UserProfilesRepository(mockDataLink.Object);
        }

        [Test]
        public void Constructor_NullDataLink_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UserProfilesRepository(null));
        }

        [Test]
        public void GetUserProfileByUserId_ValidUserId_ReturnsUserProfile()
        {
            // Arrange
            var userId = 1;
            var profileId = 101; // Example profile ID

            var dataTable = new DataTable();
            dataTable.Columns.Add("profile_id", typeof(int));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("bio", typeof(string));
            dataTable.Columns.Add("profile_picture", typeof(string));
            dataTable.Columns.Add("equipped_frame", typeof(string));
            dataTable.Columns.Add("equipped_hat", typeof(string));
            dataTable.Columns.Add("equipped_pet", typeof(string));
            dataTable.Columns.Add("equipped_emoji", typeof(string));
            dataTable.Columns.Add("last_modified", typeof(DateTime));
            dataTable.Rows.Add(profileId, userId, "Test Bio", "TestPicture.jpg", null, null, null, null, DateTime.Now);

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(
                    It.Is<string>(sql => sql.Contains("SELECT") && sql.Contains("FROM UserProfiles")),
                    It.Is<SqlParameter[]>(parameters => parameters.Length == 1 && (int)parameters[0].Value == userId)))
                .Returns(dataTable);

            // Act
            var result = userProfileRepository.GetUserProfileByUserId(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProfileId, Is.EqualTo(profileId));
            Assert.That(result.UserId, Is.EqualTo(userId));
            Assert.That(result.Bio, Is.EqualTo("Test Bio"));
            Assert.That(result.ProfilePicture, Is.EqualTo("TestPicture.jpg"));
        }

        [Test]
        public void GetUserProfileByUserId_NoRows_ReturnsNull()
        {
            // Arrange
            var userId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("profile_id", typeof(int));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("bio", typeof(string));
            dataTable.Columns.Add("profile_picture", typeof(string));
            dataTable.Columns.Add("equipped_frame", typeof(string));
            dataTable.Columns.Add("equipped_hat", typeof(string));
            dataTable.Columns.Add("equipped_pet", typeof(string));
            dataTable.Columns.Add("equipped_emoji", typeof(string));
            dataTable.Columns.Add("last_modified", typeof(DateTime));

            // No rows added
            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(
                    It.Is<string>(sql => sql.Contains("SELECT") && sql.Contains("FROM UserProfiles")),
                    It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = userProfileRepository.GetUserProfileByUserId(userId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetUserProfileByUserId_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var userId = 1;

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database Error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => userProfileRepository.GetUserProfileByUserId(userId));
        }

        [Test]
        public void UpdateProfile_ValidProfile_ReturnsUpdatedProfile()
        {
            // Arrange
            var profile = new UserProfile
            {
                ProfileId = 1,
                UserId = 1,
                Bio = "Test Bio",
                ProfilePicture = "test.jpg"
            };

            var dataTable = new DataTable();
            dataTable.Columns.Add("profile_id", typeof(int));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("bio", typeof(string));
            dataTable.Columns.Add("profile_picture", typeof(string));
            dataTable.Columns.Add("equipped_frame", typeof(string));
            dataTable.Columns.Add("equipped_hat", typeof(string));
            dataTable.Columns.Add("equipped_pet", typeof(string));
            dataTable.Columns.Add("equipped_emoji", typeof(string));
            dataTable.Columns.Add("last_modified", typeof(DateTime));
            dataTable.Rows.Add(profile.ProfileId, profile.UserId, profile.Bio, profile.ProfilePicture, null, null, null, null, DateTime.Now);

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(
                    It.Is<string>(sql => sql.Contains("UPDATE UserProfiles") && sql.Contains("SELECT")),
                    It.Is<SqlParameter[]>(parameters => parameters.Length == 4)))
                .Returns(dataTable);

            // Act
            var result = userProfileRepository.UpdateProfile(profile);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProfileId, Is.EqualTo(profile.ProfileId));
            Assert.That(result.UserId, Is.EqualTo(profile.UserId));
            Assert.That(result.Bio, Is.EqualTo(profile.Bio));
            Assert.That(result.ProfilePicture, Is.EqualTo(profile.ProfilePicture));
        }

        [Test]
        public void UpdateProfile_NoRows_ReturnsNull()
        {
            // Arrange
            var profile = new UserProfile
            {
                ProfileId = 1,
                UserId = 1,
                Bio = "Test Bio",
            };

            var dataTable = new DataTable();
            dataTable.Columns.Add("profile_id", typeof(int));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("bio", typeof(string));
            dataTable.Columns.Add("profile_picture", typeof(string));
            dataTable.Columns.Add("equipped_frame", typeof(string));
            dataTable.Columns.Add("equipped_hat", typeof(string));
            dataTable.Columns.Add("equipped_pet", typeof(string));
            dataTable.Columns.Add("equipped_emoji", typeof(string));
            dataTable.Columns.Add("last_modified", typeof(DateTime));

            // No rows added
            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(
                    It.Is<string>(sql => sql.Contains("UPDATE UserProfiles")),
                    It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = userProfileRepository.UpdateProfile(profile);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void UpdateProfile_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var profile = new UserProfile
            {
                ProfileId = 1,
                UserId = 1,
                Bio = "Test Bio",
            };

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database Error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => userProfileRepository.UpdateProfile(profile));
        }

        [Test]
        public void UpdateProfile_NullBio_SetsBioToDBNull()
        {
            // Arrange
            var profile = new UserProfile
            {
                ProfileId = 1,
                UserId = 1,
                Bio = null,
                ProfilePicture = "pic.png"
            };

            var dataTable = new DataTable();
            dataTable.Columns.Add("profile_id", typeof(int));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("bio", typeof(string));
            dataTable.Columns.Add("profile_picture", typeof(string));
            dataTable.Columns.Add("equipped_frame", typeof(string));
            dataTable.Columns.Add("equipped_hat", typeof(string));
            dataTable.Columns.Add("equipped_pet", typeof(string));
            dataTable.Columns.Add("equipped_emoji", typeof(string));
            dataTable.Columns.Add("last_modified", typeof(DateTime));
            dataTable.Rows.Add(profile.ProfileId, profile.UserId, DBNull.Value, profile.ProfilePicture, null, null, null, null, DateTime.Now);

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(
                    It.Is<string>(sql => sql.Contains("UPDATE UserProfiles")),
                    It.Is<SqlParameter[]>(parameters =>
                        parameters.Length == 4 &&
                        parameters[2].Value == DBNull.Value)))
                .Returns(dataTable);

            // Act
            var result = userProfileRepository.UpdateProfile(profile);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Bio, Is.Null);
        }

        [Test]
        public void CreateProfile_ValidUserId_ReturnsNewProfile()
        {
            // Arrange
            var userId = 1;
            var profileId = 101; // Example profile ID

            var dataTable = new DataTable();
            dataTable.Columns.Add("profile_id", typeof(int));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("bio", typeof(string));
            dataTable.Columns.Add("profile_picture", typeof(string));
            dataTable.Columns.Add("equipped_frame", typeof(string));
            dataTable.Columns.Add("equipped_hat", typeof(string));
            dataTable.Columns.Add("equipped_pet", typeof(string));
            dataTable.Columns.Add("equipped_emoji", typeof(string));
            dataTable.Columns.Add("last_modified", typeof(DateTime));
            dataTable.Rows.Add(profileId, userId, null, null, null, null, null, null, DateTime.Now);

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(
                    It.Is<string>(sql => sql.Contains("INSERT INTO UserProfiles") && sql.Contains("SCOPE_IDENTITY()")),
                    It.Is<SqlParameter[]>(parameters => parameters.Length == 1 && (int)parameters[0].Value == userId)))
                .Returns(dataTable);

            // Act
            var result = userProfileRepository.CreateProfile(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProfileId, Is.EqualTo(profileId));
            Assert.That(result.UserId, Is.EqualTo(userId));
        }

        [Test]
        public void CreateProfile_NoRows_ReturnsNull()
        {
            // Arrange
            var userId = 1;
            var dataTable = new DataTable();
            dataTable.Columns.Add("profile_id", typeof(int));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("bio", typeof(string));
            dataTable.Columns.Add("profile_picture", typeof(string));
            dataTable.Columns.Add("equipped_frame", typeof(string));
            dataTable.Columns.Add("equipped_hat", typeof(string));
            dataTable.Columns.Add("equipped_pet", typeof(string));
            dataTable.Columns.Add("equipped_emoji", typeof(string));
            dataTable.Columns.Add("last_modified", typeof(DateTime));

            // No rows added
            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(
                    It.Is<string>(sql => sql.Contains("INSERT INTO UserProfiles")),
                    It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = userProfileRepository.CreateProfile(userId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void CreateProfile_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var userId = 1;

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database Error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => userProfileRepository.CreateProfile(userId));
        }

        [Test]
        public void UpdateProfileBio_ValidInputs_ExecutesWithoutException()
        {
            // Arrange
            var userId = 1;
            var bio = "This is a test bio.";

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteNonQuerySql(
                    It.Is<string>(sql => sql.Contains("UPDATE UserProfiles") && sql.Contains("SET bio =")),
                    It.Is<SqlParameter[]>(parameters =>
                        parameters.Length == 2 &&
                        (int)parameters[0].Value == userId &&
                        (string)parameters[1].Value == bio)))
                .Verifiable();

            // Act
            userProfileRepository.UpdateProfileBio(userId, bio);

            // Assert
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(
                It.Is<string>(sql => sql.Contains("UPDATE UserProfiles") && sql.Contains("SET bio =")),
                It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void UpdateProfileBio_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var userId = 1;
            var bio = "This is a test bio.";

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteNonQuerySql(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            var exception = Assert.Throws<RepositoryException>(() => userProfileRepository.UpdateProfileBio(userId, bio));
            Assert.That(exception.Message, Is.EqualTo($"Failed to update bio for user {userId}."));
        }

        [Test]
        public void UpdateProfilePicture_ValidInputs_ExecutesWithoutException()
        {
            // Arrange
            var userId = 1;
            var picture = "profile_picture_url";

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteNonQuerySql(
                    It.Is<string>(sql => sql.Contains("UPDATE UserProfiles") && sql.Contains("SET profile_picture =")),
                    It.Is<SqlParameter[]>(parameters =>
                        parameters.Length == 2 &&
                        (int)parameters[0].Value == userId &&
                        (string)parameters[1].Value == picture)))
                .Verifiable();

            // Act
            userProfileRepository.UpdateProfilePicture(userId, picture);

            // Assert
            mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuerySql(
                It.Is<string>(sql => sql.Contains("UPDATE UserProfiles") && sql.Contains("SET profile_picture =")),
                It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void UpdateProfilePicture_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var userId = 1;
            var picture = "profile_picture_url";

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteNonQuerySql(
                    It.IsAny<string>(),
                    It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            var exception = Assert.Throws<RepositoryException>(() => userProfileRepository.UpdateProfilePicture(userId, picture));
            Assert.That(exception.Message, Is.EqualTo($"Failed to update profile picture for user {userId}."));
        }
    }
    */
}