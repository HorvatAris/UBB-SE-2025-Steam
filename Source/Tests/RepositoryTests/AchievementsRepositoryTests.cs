using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
    public class AchievementsRepositoryTests
    {
        private AchievementsRepository achievementsRepository;
        private Mock<IDataLink> mockDataLink;

        [SetUp]
        public void Setup()
        {
            mockDataLink = new Mock<IDataLink>();
            achievementsRepository = new AchievementsRepository(mockDataLink.Object);
        }

        [Test]
        public void Constructor_WhenDataLinkIsNull_ThrowsArgumentNullException()
        {
            // Arrange & Act
            var exception = Assert.Throws<ArgumentNullException>(() => new AchievementsRepository(null));

            // Assert
            Assert.That(exception.ParamName, Is.EqualTo("dataLink"));
        }

        [Test]
        public void InsertAchievements_WhenNoException_DoesNotThrow()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuery("InsertAchievements", null)).Returns(1);

            // Act & Assert
            Assert.DoesNotThrow(() => achievementsRepository.InsertAchievements());
        }

        [Test]
        public void InsertAchievements_WhenException_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), null)).Throws(new DatabaseOperationException("Simulated DB failure"));

            // Act
            var exception = Assert.Throws<RepositoryException>(() => achievementsRepository.InsertAchievements());

            // Assert
            Assert.That(exception.Message, Does.Contain("inserting achievements"));
        }

        public class FakeSqlException : Exception
        {
        }

        [Test]
        public void IsAchievementsTableEmpty_WhenResultIsZero_ReturnsTrue()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalar<int>("IsAchievementsTableEmpty", null)).Returns(0);

            // Act
            bool result = achievementsRepository.IsAchievementsTableEmpty();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsAchievementsTableEmpty_WhenResultIsFive_ReturnsFalse()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), null)).Returns(5);

            // Act
            bool result = achievementsRepository.IsAchievementsTableEmpty();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsAchievementsTableEmpty_WhenGeneralExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink
                .Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), null))
                .Throws(new DatabaseOperationException("Something went wrong"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.IsAchievementsTableEmpty());
        }

        [Test]
        public void UpdateAchievementIconUrl_WhenCalledWithValidData_DoesNotThrow()
        {
            // Arrange
            mockDataLink
                .Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(1); // simulate success

            // Act & Assert
            Assert.DoesNotThrow(() => achievementsRepository.UpdateAchievementIconUrl(10, "icon.png"));
        }

        [Test]
        public void UpdateAchievementIconUrl_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("test"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.UpdateAchievementIconUrl(10, "url"));
        }

        [Test]
        public void GetAllAchievements_WhenDataReturned_ResultNotNull()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Desc", "TypeA", 10, "url.png");

            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), null)).Returns(table);

            // Act
            var result = achievementsRepository.GetAllAchievements();

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetAllAchievements_WhenDataReturned_CountIsOne()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Desc", "TypeA", 10, "url.png");

            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), null)).Returns(table);

            // Act
            var result = achievementsRepository.GetAllAchievements();

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetAllAchievements_WhenDataReturned_FirstAchievementNameACH1()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Desc", "TypeA", 10, "url.png");

            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), null)).Returns(table);

            // Act
            var result = achievementsRepository.GetAllAchievements();

            // Assert
            Assert.That(result[0].AchievementName, Is.EqualTo("ACH1"));
        }

        [Test]
        public void GetAllAchievements_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), null))
                         .Throws(new DatabaseOperationException("test"));

            // Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.GetAllAchievements());
        }

        [Test]
        public void GetUnlockedAchievementsForUser_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            var parameters = new SqlParameter[] { new SqlParameter("@userId", 1) };

            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("test"));

           // Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.GetUnlockedAchievementsForUser(1));
        }

        [Test]
        public void GetUnlockedAchievementsForUser_WhenCalledWithValidUserId_ReturnsAchievementNotNull()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Desc", "Type", 10, "url.png");

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var result = achievementsRepository.GetUnlockedAchievementsForUser(1);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetUnlockedAchievementsForUser_WhenCalledWithValidUserId_ReturnsAchievementCountEqualTo1()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Desc", "Type", 10, "url.png");

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var result = achievementsRepository.GetUnlockedAchievementsForUser(1);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetUnlockedAchievementsForUser_WhenCalledWithValidUserId_ReturnsAchievementNameACH1()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Desc", "Type", 10, "url.png");

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var result = achievementsRepository.GetUnlockedAchievementsForUser(1);

            // Assert
            Assert.That(result[0].AchievementName, Is.EqualTo("ACH1"));
        }

        [Test]
        public void UnlockAchievement_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("test"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.UnlockAchievement(1, 100));
        }

        [Test]
        public void UnlockAchievement_WhenCalledWithValidData_DoesNotThrow()
        {
            // Arrange
            mockDataLink
                .Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(1); // simulate successful update

            // Act & Assert
            Assert.DoesNotThrow(() => achievementsRepository.UnlockAchievement(1, 100));
        }

        [Test]
        public void RemoveAchievement_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("test"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.RemoveAchievement(1, 100));
        }

        [Test]
        public void RemoveAchievement_WhenCalledWithValidData_DoesNotThrow()
        {
            // Arrange
            mockDataLink
                .Setup(dataLink => dataLink.ExecuteNonQuerySql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(1);

            // Act & Assert
            Assert.DoesNotThrow(() => achievementsRepository.RemoveAchievement(1, 100));
        }

        [Test]
        public void GetUnlockedDataForAchievement_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("test"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.GetUnlockedDataForAchievement(1, 2));
        }

        [Test]
        public void GetUnlockedDataForAchievement_WhenNoData_ReturnsNull()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("AchievementName", typeof(string));
            table.Columns.Add("AchievementDescription", typeof(string));
            table.Columns.Add("UnlockDate", typeof(DateTime));

            // Note: No rows added!
            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var result = achievementsRepository.GetUnlockedDataForAchievement(1, 1);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetUnlockedDataForAchievement_WhenDataExists_ReturnsDataNotNull()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("AchievementName", typeof(string));
            table.Columns.Add("AchievementDescription", typeof(string));
            table.Columns.Add("UnlockDate", typeof(DateTime));

            table.Rows.Add("ACH1", "Description", DateTime.Today);

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var result = achievementsRepository.GetUnlockedDataForAchievement(1, 1);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetUnlockedDataForAchievement_WhenDataExists_ReturnsAchievementNameACH1()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("AchievementName", typeof(string));
            table.Columns.Add("AchievementDescription", typeof(string));
            table.Columns.Add("UnlockDate", typeof(DateTime));

            table.Rows.Add("ACH1", "Description", DateTime.Today);

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var result = achievementsRepository.GetUnlockedDataForAchievement(1, 1);

            // Assert
            Assert.That(result.AchievementName, Is.EqualTo("ACH1"));
        }

        [Test]
        public void GetAllAchievements_WhenDataReturned_ResultCountIs1()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Great job!", "TypeA", 10, "http://icon.url");

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), null))
                .Returns(table);

            // Act
            var results = achievementsRepository.GetAllAchievements();

            // Assert
            Assert.That(results.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetAllAchievements_WhenDataReturned_AchievementIdIs1()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Great job!", "TypeA", 10, "http://icon.url");

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), null))
                .Returns(table);

            // Act
            var results = achievementsRepository.GetAllAchievements();

            // Assert
            var ach = results[0];
            Assert.That(ach.AchievementId, Is.EqualTo(1));
        }

        [Test]
        public void GetAllAchievements_WhenDataReturned_AchievementNameIsACH1()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Great job!", "TypeA", 10, "http://icon.url");

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), null))
                .Returns(table);

            // Act
            var results = achievementsRepository.GetAllAchievements();

            // Assert
            var ach = results[0];
            Assert.That(ach.AchievementName, Is.EqualTo("ACH1"));
        }

        [Test]
        public void GetAllAchievements_WhenDataReturned_AchievementDescriptionIsGreatJob()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Great job!", "TypeA", 10, "http://icon.url");

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), null))
                .Returns(table);

            // Act
            var results = achievementsRepository.GetAllAchievements();

            // Assert
            var ach = results[0];
            Assert.That(ach.Description, Is.EqualTo("Great job!"));
        }

        [Test]
        public void GetAllAchievements_WhenDataReturned_AchievementTypeReturnsCorrectly()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Great job!", "TypeA", 10, "http://icon.url");

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), null))
                .Returns(table);

            // Act
            var results = achievementsRepository.GetAllAchievements();

            // Assert
            var ach = results[0];
            Assert.That(ach.AchievementType, Is.EqualTo("TypeA"));
        }

        [Test]
        public void GetAllAchievements_WhenDataReturned_AchievementPointsReturnsCorrectly()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Great job!", "TypeA", 10, "http://icon.url");

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), null))
                .Returns(table);

            // Act
            var results = achievementsRepository.GetAllAchievements();

            // Assert
            var ach = results[0];
            Assert.That(ach.Points, Is.EqualTo(10));
        }

        [Test]
        public void GetAllAchievements_WhenDataReturned_AchievementIconReturnsCorrectly()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Great job!", "TypeA", 10, "http://icon.url");

            mockDataLink
                .Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), null))
                .Returns(table);

            // Act
            var results = achievementsRepository.GetAllAchievements();

            // Assert
            var ach = results[0];
            Assert.That(ach.Icon, Is.EqualTo("http://icon.url"));
        }

        [Test]
        public void IsAchievementUnlocked_WhenResultIs1_ReturnsTrue()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>())).Returns(1);

            // Act
            bool isUnlocked = achievementsRepository.IsAchievementUnlocked(1, 2);

            // Assert
            Assert.That(isUnlocked, Is.True);
        }

        [Test]
        public void IsAchievementUnlocked_WhenResultIs0_ReturnsFalse()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>())).Returns(0);

            // Act
            bool isUnlocked = achievementsRepository.IsAchievementUnlocked(1, 2);

            // Assert
            Assert.That(isUnlocked, Is.False);
        }

        [Test]
        public void IsAchievementUnlocked_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink
                .Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Simulated failure"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.IsAchievementUnlocked(1, 10));
        }

        private DataTable CreateMockAchievementTable()
        {
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Desc", "Type", 10, "url.png");
            return table;
        }

        private DataTable CreateMockUnlockDataTable()
        {
            var table = new DataTable();
            table.Columns.Add("AchievementName", typeof(string));
            table.Columns.Add("AchievementDescription", typeof(string));
            table.Columns.Add("UnlockDate", typeof(DateTime));

            table.Rows.Add("ACH1", "Desc", DateTime.Today);
            return table;
        }

        [Test]
        public void GetAchievementsWithStatusForUser_WhenCalled_ReturnsAchievementsCountEqualTo1()
        {
            // Arrange
            mockDataLink.Setup(x => x.ExecuteReaderSql(It.Is<string>(sql => sql.Contains("FROM Achievements")), null))
                        .Returns(CreateMockAchievementTable());

            mockDataLink.Setup(x => x.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                        .Returns(1); // Unlocked

            mockDataLink.Setup(x => x.ExecuteReaderSql(It.Is<string>(sql => sql.Contains("FROM UserAchievements")), It.IsAny<SqlParameter[]>()))
                        .Returns(CreateMockUnlockDataTable());

            // Act
            var result = achievementsRepository.GetAchievementsWithStatusForUser(1);

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetAchievementsWithStatusForUser_WhenCalled_ReturnsAchievementsUnlocked()
        {
            // Arrange
            var achievements = new List<Achievement>
    {
        new Achievement { AchievementId = 1, AchievementName = "A1" }
    };

            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.Is<string>(sql => sql.Contains("FROM Achievements")), null))
                         .Returns(CreateMockAchievementTable());

            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(1);

            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.Is<string>(sql => sql.Contains("FROM UserAchievements")), It.IsAny<SqlParameter[]>()))
                         .Returns(CreateMockUnlockDataTable());

            // Act
            var result = achievementsRepository.GetAchievementsWithStatusForUser(1);

            // Assert
            Assert.That(result[0].IsUnlocked, Is.True);
        }

        [Test]
        public void GetAchievementsWithStatusForUser_WhenUnlockedDataIsNull_ResultCountIs1()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));
            table.Rows.Add(1, "A1", "Desc", "Type", 10, "url");

            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.Is<string>(sql => sql.Contains("FROM Achievements")), null))
                         .Returns(table);

            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(1);

            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.Is<string>(sql => sql.Contains("FROM UserAchievements")), It.IsAny<SqlParameter[]>()))
                         .Returns(new DataTable()); // empty table = null unlockedData

            // Act
            var result = achievementsRepository.GetAchievementsWithStatusForUser(1);

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
        }

        [Test]
        public void GetAchievementsWithStatusForUser_WhenUnlockedDataIsNull_SetsUnlockedDateToNull()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));
            table.Rows.Add(1, "A1", "Desc", "Type", 10, "url");

            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.Is<string>(sql => sql.Contains("FROM Achievements")), null))
                         .Returns(table);

            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(1);

            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.Is<string>(sql => sql.Contains("FROM UserAchievements")), It.IsAny<SqlParameter[]>()))
                         .Returns(new DataTable()); // empty table = null unlockedData

            // Act
            var result = achievementsRepository.GetAchievementsWithStatusForUser(1);

            // Assert
            Assert.That(result[0].UnlockedDate, Is.Null); // <-- key point!
        }

        [Test]
        public void GetAchievementsWithStatusForUser_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteReaderSql(It.IsAny<string>(), null))
                         .Throws(new Exception("DB error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.GetAchievementsWithStatusForUser(1));
        }

        [Test]
        public void GetNumberOfSoldGames_WhenCalled_ReturnsValue()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(5);

            // Act
            var result = achievementsRepository.GetNumberOfSoldGames(1);

            // Assert
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void GetNumberOfSoldGames_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("fail"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.GetNumberOfSoldGames(1));
        }

        [Test]
        public void GetFriendshipCount_WhenCalled_ReturnsCount()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(2);

            // Act
            var result = achievementsRepository.GetFriendshipCount(1);

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void GetFriendshipCount_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Act & Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("fail"));

            // Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.GetFriendshipCount(1));
        }

        [Test]
        public void GetNumberOfOwnedGames_WhenCalled_ReturnsCount()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(3);

            // Act
            var result = achievementsRepository.GetNumberOfOwnedGames(1);

            // Assert
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void GetNumberOfOwnedGames_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("fail"));

            // Act
            var exception = Assert.Throws<RepositoryException>(() => achievementsRepository.GetNumberOfOwnedGames(1));

            // Assert
            Assert.That(exception.Message, Does.Contain("owned games"));
        }

        [Test]
        public void GetNumberOfReviewsGiven_WhenCalled_ReturnsCount()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(10);

            // Act
            var result = achievementsRepository.GetNumberOfReviewsGiven(1);

            // Assert
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void GetNumberOfReviewsGiven_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("fail"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.GetNumberOfReviewsGiven(1));
        }

        [Test]
        public void GetNumberOfReviewsReceived_WhenCalled_ReturnsCount()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(8);

            // Act
            var result = achievementsRepository.GetNumberOfReviewsReceived(1);

            // Assert
            Assert.That(result, Is.EqualTo(8));
        }

        [Test]
        public void GetNumberOfReviewsReceived_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("Simulated failure"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.GetNumberOfReviewsReceived(1));
        }

        [Test]
        public void GetYearsOfActivity_WhenUserCreatedEarlier_ReturnsCorrectYears()
        {
            // Arrange
            var now = DateTime.Now;
            var fiveYearsAgo = now.AddYears(-5);

            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<DateTime>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(fiveYearsAgo);

            // Act
            var result = achievementsRepository.GetYearsOfAcftivity(1);

            // Assert
            Assert.That(result, Is.EqualTo(5).Or.EqualTo(4)); // Adjusted by day of year
        }

        [Test]
        public void GetYearsOfActivity_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<DateTime>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("Database fail"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.GetYearsOfAcftivity(1));
        }

        [Test]
        public void GetAchievementIdByName_WhenFound_ReturnsId()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int?>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(42);

            // Act
            var result = achievementsRepository.GetAchievementIdByName("ACH_NAME");

            // Assert
            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void GetAchievementIdByName_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int?>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("Database fail"));

            // Act
            var exception = Assert.Throws<RepositoryException>(() => achievementsRepository.GetAchievementIdByName("ACH_NAME"));

            // Assert
            Assert.That(exception.Message, Does.Contain("achievement ID"));
        }

        [Test]
        public void IsUserDeveloper_WhenUserIsDeveloper_ReturnsTrue()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<bool>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Returns(true);

            // Act
            var result = achievementsRepository.IsUserDeveloper(1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsUserDeveloper_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<bool>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                         .Throws(new DatabaseOperationException("fail"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.IsUserDeveloper(1));
        }

        [Test]
        public void GetNumberOfPosts_WhenReturns3_Returns3()
        {
            // Arrange
            mockDataLink.Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>())).Returns(3);

            // Act
            int posts = achievementsRepository.GetNumberOfPosts(1);

            // Assert
            Assert.That(posts, Is.EqualTo(3));
        }

        [Test]
        public void GetNumberOfPosts_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            mockDataLink
                .Setup(dataLink => dataLink.ExecuteScalarSql<int>(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Simulated DB failure"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => achievementsRepository.GetNumberOfPosts(1));
        }
    }
    */
}
