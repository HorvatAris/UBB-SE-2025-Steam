using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using BusinessLayer.Services;
using BusinessLayer.Models;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;
using Moq;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class AchievementsServiceTests
    {
        private Mock<IAchievementsRepository> mockAchievementsRepository;
        private AchievementsService achievementsService;

        public static class Categories
        {
            public const string Friendships = "Friendships";
            public const string OwnedGames = "Owned Games";
            public const string SoldGames = "Sold Games";
            public const string YearsOfActivity = "Years of Activity";
            public const string NumberOfPosts = "Number of Posts";
            public const string NumberOfReviewsGiven = "Number of Reviews Given";
            public const string NumberOfReviewsReceived = "Number of Reviews Received";
            public const string Developer = "Developer";
        }

        [SetUp]
        public void SetUp()
        {
            mockAchievementsRepository = new Mock<IAchievementsRepository>();
            achievementsService = new AchievementsService(mockAchievementsRepository.Object);
        }

        [Test]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var service = new AchievementsService(null);
            });

            // & Assert
            Assert.That(exception.ParamName, Is.EqualTo("achievementsRepository"));
        }

        [Test]
        public void GetGroupedAchievementsForUser_Called_ReturnsAllAchievements()
        {
            // Arrange
            var achievements = new List<AchievementWithStatus>
    {
        new AchievementWithStatus { Achievement = new Achievement { AchievementType = Categories.Friendships } },
        new AchievementWithStatus { Achievement = new Achievement { AchievementType = Categories.OwnedGames } },
        new AchievementWithStatus { Achievement = new Achievement { AchievementType = Categories.Friendships } },
        new AchievementWithStatus { Achievement = new Achievement { AchievementType = Categories.Developer } },
    };
            mockAchievementsRepository.Setup(repository => repository.GetAchievementsWithStatusForUser(It.IsAny<int>()))
                     .Returns(achievements);

            var service = new AchievementsService(mockAchievementsRepository.Object);

            // Act
            var result = service.GetGroupedAchievementsForUser(1);

            // Assert
            Assert.That(result.AllAchievements.Count, Is.EqualTo(4));
        }

        [Test]
        public void GetGroupedAchievementsForUser_Called_GroupsFriendshipsCorrectly()
        {
            // Arrange
            var achievements = new List<AchievementWithStatus>
    {
        new AchievementWithStatus { Achievement = new Achievement { AchievementType = Categories.Friendships } },
        new AchievementWithStatus { Achievement = new Achievement { AchievementType = Categories.Friendships } },
    };
            // Arrange
            mockAchievementsRepository.Setup(repository => repository.GetAchievementsWithStatusForUser(It.IsAny<int>()))
                     .Returns(achievements);

            var service = new AchievementsService(mockAchievementsRepository.Object);

            // Act
            var result = service.GetGroupedAchievementsForUser(1);

            // Assert
            Assert.That(result.Friendships.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetGroupedAchievementsForUser_Called_GroupsOwnedGamesCorrectly()
        {
            // Arrange
            var achievements = new List<AchievementWithStatus>
    {
        new AchievementWithStatus { Achievement = new Achievement { AchievementType = Categories.OwnedGames } },
    };

            mockAchievementsRepository.Setup(repository => repository.GetAchievementsWithStatusForUser(It.IsAny<int>()))
                     .Returns(achievements);

            var service = new AchievementsService(mockAchievementsRepository.Object);

            // Act
            var result = service.GetGroupedAchievementsForUser(1);

            // Assert
            Assert.That(result.OwnedGames.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetGroupedAchievementsForUser_Called_GroupsDeveloperCorrectly()
        {
            // Arrange
            var achievements = new List<AchievementWithStatus>
    {
        new AchievementWithStatus { Achievement = new Achievement { AchievementType = Categories.Developer } },
    };

            mockAchievementsRepository.Setup(repository => repository.GetAchievementsWithStatusForUser(It.IsAny<int>()))
                     .Returns(achievements);

            var service = new AchievementsService(mockAchievementsRepository.Object);

            // Act
            var result = service.GetGroupedAchievementsForUser(1);

            // Assert
            Assert.That(result.Developer.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetGroupedAchievementsForUser_Called_ReturnsEmptySoldGamesWhenNoneExist()
        {
            // Arrange
            var achievements = new List<AchievementWithStatus>
    {
        new AchievementWithStatus { Achievement = new Achievement { AchievementType = Categories.Friendships } },
    };

            mockAchievementsRepository.Setup(repository => repository.GetAchievementsWithStatusForUser(It.IsAny<int>()))
                     .Returns(achievements);

            var service = new AchievementsService(mockAchievementsRepository.Object);

            // Act
            var result = service.GetGroupedAchievementsForUser(1);

            // Assert
            Assert.That(result.SoldGames, Is.Empty);
        }

        [Test]
        public void GetGroupedAchievementsForUser_WhenRepoThrows_ThrowsServiceExceptionWithCorrectMessage()
        {
            // Arrange
            mockAchievementsRepository.Setup(repository => repository.GetAchievementsWithStatusForUser(It.IsAny<int>()))
                     .Throws(new RepositoryException("DB error"));

            var service = new AchievementsService(mockAchievementsRepository.Object);

            // Act & Assert
            var exception = Assert.Throws<ServiceException>(() => service.GetGroupedAchievementsForUser(42));
            Assert.That(exception.Message, Does.Contain("grouping achievements"));
        }

        [Test]
        public void GetGroupedAchievementsForUser_WhenRepoThrows_ThrowsServiceException_WithInnerException()
        {
            // Arrange
            mockAchievementsRepository.Setup(repository => repository.GetAchievementsWithStatusForUser(It.IsAny<int>()))
                     .Throws(new RepositoryException("DB error"));

            var service = new AchievementsService(mockAchievementsRepository.Object);

            // Act
            var exception = Assert.Throws<ServiceException>(() => service.GetGroupedAchievementsForUser(42));

            // Assert
            Assert.That(exception.InnerException, Is.TypeOf<RepositoryException>());
        }

        [Test]
        public void InitializeAchievements_WhenTableIsEmpty_CallsInsertAchievements()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository
            {
                AchievementsTableIsEmpty = true
            };

            var service = new AchievementsService(fakeRepository);
            // Act
            service.InitializeAchievements();

            // Assert
            Assert.That(fakeRepository.InsertAchievementsCalled, Is.True);
        }

        [Test]
        public void InitializeAchievements_WhenTableIsEmpty_CallsUpdatesIcons()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository
            {
                AchievementsTableIsEmpty = true
            };

            var service = new AchievementsService(fakeRepository);
            // Act
            service.InitializeAchievements();

            // Assert
            Assert.That(fakeRepository.UpdatedIcons.Count, Is.EqualTo(5));
        }

        [Test]
        public void InitializeAchievements_WhenRepoThrows_DoesNotCrash()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository
            {
                AchievementsTableIsEmpty = true,
                ThrowOnInsertAchievements = true
            };

            // Act
            var service = new AchievementsService(fakeRepository);

            // Assert
            Assert.DoesNotThrow(() => service.InitializeAchievements());
        }

        [Test]
        public void GetAchievementsForUser_WhenCalled_ReturnsNumberOfAchievements()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            fakeRepository.AllAchievementsToReturn = new List<Achievement>
    {
        new Achievement { AchievementId = 1 },
        new Achievement { AchievementId = 2 }
    };

            var service = new AchievementsService(fakeRepository);
            // Act
            var result = service.GetAchievementsForUser(5);
            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetAchievementsForUser_WhenCalled_ReturnsFirstAchievements()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            fakeRepository.AllAchievementsToReturn = new List<Achievement>
    {
        new Achievement { AchievementId = 1 },
        new Achievement { AchievementId = 2 }
    };

            var service = new AchievementsService(fakeRepository);
            // Act
            var result = service.GetAchievementsForUser(5);
            // Assert
            Assert.That(result[0].AchievementId, Is.EqualTo(1));
        }

        [Test]
        public void GetAchievementsForUser_WhenRepoThrows_ThrowsServiceException()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository { ThrowOnGetAllAchievements = true };

            // Act
            var service = new AchievementsService(fakeRepository);

            // Assert
            Assert.Throws<ServiceException>(() => service.GetAchievementsForUser(123));
        }

        [Test]
        public void UpdateAchievementIconUrls_WhenRepositoryThrows_DoesNotCrash()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository
            {
                AchievementsTableIsEmpty = true,
                ThrowOnUpdateIconUrl = true
            };

            var service = new AchievementsService(fakeRepository);

            // Act + Assert: Method should catch exception and not throw it
            Assert.DoesNotThrow(() => service.InitializeAchievements());
        }

        [Test]
        public void UnlockAchievementForUser_WithValidFriendshipCount_UnlocksAchievement()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepository);

            int userId = 1;
            int friendCount = 5;
            int expectedAchievementId = 123;

            fakeRepository.NumberOfFriends = friendCount;

            fakeRepository.AchievementIds["Friendships"] = new Dictionary<int, int?>
    {
        { 2, expectedAchievementId }
    };
            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepository.UnlockedAchievements.Any(repository =>
                repository.userId == userId && repository.achievementId == expectedAchievementId), Is.True,
                "The achievement should be unlocked for the valid friendship count.");
        }

        [Test]
        public void UnlockAchievementForUser_WithNoMatchingAchievementId_DoesNotUnlockAnything()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepository);

            int userId = 1;
            int friendCount = 5;

            fakeRepository.NumberOfFriends = friendCount;

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepository.UnlockedAchievements, Is.Empty,
                "No achievements should be unlocked if the achievement ID is null.");
        }

        [Test]
        public void UnlockAchievementForUser_WithValidOwnedGamesCount_UnlocksAchievement()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepository);

            int userId = 1;
            int ownedGames = 10;
            int expectedAchievementId = 456;

            fakeRepository.NumberOfOwnedGames = ownedGames;

            fakeRepository.AchievementIds[Categories.OwnedGames] = new Dictionary<int, int?> { { 3, expectedAchievementId } };

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepository.UnlockedAchievements.Any(repository =>
                repository.userId == userId && repository.achievementId == expectedAchievementId), Is.True,
                "The achievement should be unlocked for the valid owned games count.");
        }

        [Test]
        public void UnlockAchievementForUser_WithOwnedGamesAchievementIdNull_DoesNotUnlock()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepository);
            int userId = 1;
            fakeRepository.NumberOfOwnedGames = 10;

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepository.UnlockedAchievements, Is.Empty);
        }

        [Test]
        public void UnlockAchievementForUser_UnlocksAchievement_ForValidSoldGames()
        {
            // Arrange
            var userId = 1;
            var expectedAchievementId = 123;

            var fakeRepository = new FakeAchievementsRepository
            {
                NumberOfSoldGames = 10 // Will trigger the unlock logic
            };

            fakeRepository.SetAchievementId(Categories.SoldGames, 3, expectedAchievementId);

            var service = new AchievementsService(fakeRepository);

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepository.UnlockedAchievements.Any(repository =>
                repository.userId == userId && repository.achievementId == expectedAchievementId), Is.True,
                "The achievement should be unlocked for valid sold games count.");
        }

        [Test]
        public void UnlockAchievementForUser_WithSoldGamesAchievementIdNull_DoesNotUnlock()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepository);
            int userId = 1;
            fakeRepository.NumberOfSoldGames = 5;

            // Act
            service.UnlockAchievementForUser(userId);
            // Assert
            Assert.That(fakeRepository.UnlockedAchievements, Is.Empty);
        }

        [Test]
        public void UnlockAchievementForUser_WithValidReviewsGivenCount_UnlocksAchievement()
        {
            // Arrange
            var userId = 1;
            var expectedAchievementId = 3;
            var reviewsGiven = 10;

            var fakeRepository = new FakeAchievementsRepository
            {
                NumberOfReviewsGiven = reviewsGiven
            };

            fakeRepository.SetAchievementId(Categories.NumberOfReviewsGiven, 3, expectedAchievementId);

            var service = new AchievementsService(fakeRepository);

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepository.UnlockedAchievements.Any(repository =>
                repository.userId == userId && repository.achievementId == expectedAchievementId), Is.True,
                "The achievement should be unlocked for the valid number of reviews given.");
        }

        [Test]
        public void UnlockAchievementForUser_WithReviewsGivenAchievementIdNull_DoesNotUnlock()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepository);
            int userId = 1;
            fakeRepository.NumberOfReviewsGiven = 1;

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepository.UnlockedAchievements, Is.Empty);
        }

        [Test]
        public void UnlockAchievementForUser_WithValidReviewsReceivedCount_UnlocksAchievement()
        {
            // Arrange
            var userId = 1;
            var expectedAchievementId = 303;

            var fakeRepository = new FakeAchievementsRepository
            {
                NumberOfReviewsReceived = 10
            };

            fakeRepository.SetAchievementId(Categories.NumberOfReviewsReceived, 3, expectedAchievementId);

            var service = new AchievementsService(fakeRepository);

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepository.UnlockedAchievements.Any(repository =>
                repository.userId == userId && repository.achievementId == expectedAchievementId),
                Is.True, "The achievement should be unlocked for the valid number of reviews received.");
        }

        [Test]
        public void UnlockAchievementForUser_WithReviewsReceivedAchievementIdNull_DoesNotUnlock()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepository);
            int userId = 1;
            fakeRepository.NumberOfReviewsReceived = 10;
            // Act
            service.UnlockAchievementForUser(userId);
            // Assert
            Assert.That(fakeRepository.UnlockedAchievements, Is.Empty);
        }

        [Test]
        public void UnlockAchievementForUser_UnlocksAchievement_ForValidNumberOfPosts()
        {
            // Arrange
            var userId = 1;
            var numberOfPosts = 10;
            var expectedAchievementId = 888;

            var fakeRepository = new FakeAchievementsRepository
            {
                NumberOfPosts = numberOfPosts
            };

            fakeRepository.SetAchievementId(Categories.NumberOfPosts, 3, expectedAchievementId);

            var service = new AchievementsService(fakeRepository);

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepository.UnlockedAchievements.Any(repository =>
                repository.userId == userId && repository.achievementId == expectedAchievementId),
                Is.True,
                "The achievement should be unlocked for the valid number of posts.");
        }

        [Test]
        public void UnlockAchievementForUser_WithPostsAchievementIdNull_DoesNotUnlock()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepository);
            int userId = 1;
            fakeRepository.NumberOfPosts = 50;
            // Act
            service.UnlockAchievementForUser(userId);
            // Assert
            Assert.That(fakeRepository.UnlockedAchievements, Is.Empty);
        }

        [Test]
        public void UnlockAchievementForUser_UnlocksAchievement_ForValidYearsOfActivity()
        {
            // Arrange
            var userId = 1;
            var yearsOfActivity = 3;
            var expectedAchievementId = 333;

            var fakeRepository = new FakeAchievementsRepository
            {
                GetYearsOfAcftivity = (id) => yearsOfActivity
            };
            fakeRepository.SetAchievementId(Categories.YearsOfActivity, 3, expectedAchievementId);

            var service = new AchievementsService(fakeRepository);

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepository.UnlockedAchievements.Any(repository =>
                repository.userId == userId && repository.achievementId == expectedAchievementId),
                Is.True,
                "The achievement should be unlocked for 3 years of activity.");
        }

        [Test]
        public void UnlockAchievementForUser_WithYearsOfActivityAchievementIdNull_DoesNotUnlock()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepository);
            int userId = 1;
            fakeRepository.GetYearsOfAcftivity = _ => 3;

            // Act
            service.UnlockAchievementForUser(userId);
            // Assert
            Assert.That(fakeRepository.UnlockedAchievements, Is.Empty);
        }

        [Test]
        public void UnlockAchievementForUser_UnlocksAchievement_ForDeveloper()
        {
            // Arrange
            var userId = 1;
            var expectedAchievementId = 777;

            var fakeRepository = new FakeAchievementsRepository
            {
                IsUserDeveloper = (id) => true
            };

            fakeRepository.SetAchievementId(Categories.Developer, 1, expectedAchievementId);

            var service = new AchievementsService(fakeRepository);

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepository.UnlockedAchievements.Any(repository =>
                repository.userId == userId && repository.achievementId == expectedAchievementId),
                Is.True, "The achievement should be unlocked for developer.");
        }

        [Test]
        public void UnlockAchievementForUser_WithDeveloperAchievementIdNull_DoesNotUnlock()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepository);
            int userId = 1;
            fakeRepository.IsUserDeveloper = _ => true;

            // Act
            service.UnlockAchievementForUser(userId);
            // Assert
            Assert.That(fakeRepository.UnlockedAchievements, Is.Empty);
        }

        [Test]
        public void UnlockAchievementForUser_WhenRepositoryThrowsException_IsHandledGracefully()
        {
            // Arrange
            var userId = 1;

            var fakeRepository = new FakeAchievementsRepository
            {
                // Simulate failure on one of the repo methods
                ThrowOnGetNumberOfSoldGames = true // You'll need to add support for this flag
            };

            var service = new AchievementsService(fakeRepository);

            // Act & Assert: The method should NOT throw
            Assert.DoesNotThrow(() => service.UnlockAchievementForUser(userId));
        }

        [Test]
        public void RemoveAchievement_CallsRepositoryWithoutException()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepository);

            // Act
            Assert.DoesNotThrow(() => service.RemoveAchievement(1, 123));
        }

        [Test]
        public void RemoveAchievement_WhenRepositoryThrows_ThrowsServiceException()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository
            {
                ThrowOnRemoveAchievement = true
            };
            // Act
            var service = new AchievementsService(fakeRepository);
            // Assert
            Assert.Throws<ServiceException>(() => service.RemoveAchievement(1, 123));
        }

        [Test]
        public void GetUnlockedAchievementsForUser_ReturnsCorrectList()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            var expected = new List<Achievement>
    {
        new Achievement { AchievementId = 1, AchievementName = "Test" }
    };
            fakeRepository.UnlockedAchievementsToReturn = expected; // you'll add this

            var service = new AchievementsService(fakeRepository);
            // Act
            var result = service.GetUnlockedAchievementsForUser(1);
            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetUnlockedAchievementsForUser_WhenRepositoryThrows_ThrowsServiceException()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository
            {
                ThrowOnGetUnlockedAchievements = true
            };
            // Act
            var service = new AchievementsService(fakeRepository);

            // Assert
            Assert.Throws<ServiceException>(() => service.GetUnlockedAchievementsForUser(1));
        }

        [Test]
        public void GetAllAchievements_ReturnsAllAchievements()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            var expected = new List<Achievement>
    {
        new Achievement { AchievementId = 1, AchievementName = "Test Achievement" }
    };
            fakeRepository.AllAchievementsToReturn = expected;

            var service = new AchievementsService(fakeRepository);
            // Act
            var result = service.GetAllAchievements();
            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetAllAchievements_WhenRepositoryThrows_ThrowsServiceException()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository
            {
                ThrowOnGetAllAchievements = true
            };
            // Act
            var service = new AchievementsService(fakeRepository);
            // Assert
            Assert.Throws<ServiceException>(() => service.GetAllAchievements());
        }

        [Test]
        public void GetUnlockedDataForAchievement_ReturnsData()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            var expected = new AchievementUnlockedData
            {
                AchievementName = "Test",
                UnlockDate = DateTime.Now
            };

            fakeRepository.UnlockedDataToReturn = expected;

            var service = new AchievementsService(fakeRepository);

            // Act
            var result = service.GetUnlockedDataForAchievement(1, 2);
            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetUnlockedDataForAchievement_WhenRepositoryThrows_ThrowsServiceException()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository
            {
                ThrowOnGetUnlockedData = true
            };
            // Act
            var service = new AchievementsService(fakeRepository);
            // Assert
            Assert.Throws<ServiceException>(() => service.GetUnlockedDataForAchievement(1, 1));
        }

        [Test]
        public void GetAchievementsWithStatusForUser_ReturnsCorrectData()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository();
            var expected = new List<AchievementWithStatus>
    {
        new AchievementWithStatus
        {
            Achievement = new Achievement { AchievementId = 1, AchievementName = "Test" },
            IsUnlocked = true,
            UnlockedDate = DateTime.Now
        }
    };
            fakeRepository.AchievementsWithStatusToReturn = expected;

            var service = new AchievementsService(fakeRepository);
            // Act
            var result = service.GetAchievementsWithStatusForUser(1);
            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetAchievementsWithStatusForUser_WhenRepositoryThrows_ThrowsServiceException()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository
            {
                ThrowOnGetAchievementsWithStatus = true
            };

            // Act
            var service = new AchievementsService(fakeRepository);
            // Assert
            Assert.Throws<ServiceException>(() => service.GetAchievementsWithStatusForUser(1));
        }
        [Test]
        public void GetPointsForUnlockedAchievement_ReturnsPoints_WhenAchievementIsUnlocked()
        {
            // Arrange
            var userId = 1;
            var achievementId = 42;
            var expectedPoints = 100;

            var fakeRepository = new FakeAchievementsRepository();
            fakeRepository.UnlockAchievement(userId, achievementId); // Mark it as unlocked
            fakeRepository.AllAchievementsToReturn = new List<Achievement>
    {
        new Achievement { AchievementId = achievementId, Points = expectedPoints }
    };

            var service = new AchievementsService(fakeRepository);

            // Act
            var result = service.GetPointsForUnlockedAchievement(userId, achievementId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedPoints));
        }

        [Test]
        public void GetPointsForUnlockedAchievement_Throws_WhenAchievementNotUnlocked()
        {
            // Arrange
            var userId = 1;
            var achievementId = 42;

            var fakeRepository = new FakeAchievementsRepository(); // Not unlocking it
            fakeRepository.AllAchievementsToReturn = new List<Achievement>
    {
        new Achievement { AchievementId = achievementId, Points = 50 }
    };
            // Act
            var service = new AchievementsService(fakeRepository);
            // Assert
            Assert.Throws<ServiceException>(() =>
                service.GetPointsForUnlockedAchievement(userId, achievementId));
        }
        [Test]
        public void GetPointsForUnlockedAchievement_Throws_WhenAchievementNotFoundInList()
        {
            // Arrange
            var userId = 1;
            var achievementId = 42;

            var fakeRepository = new FakeAchievementsRepository();
            fakeRepository.UnlockAchievement(userId, achievementId); // It's unlocked
            fakeRepository.AllAchievementsToReturn = new List<Achievement>(); // But not in the list
            // Act
            var service = new AchievementsService(fakeRepository);

            // Assert
            Assert.Throws<ServiceException>(() =>
                service.GetPointsForUnlockedAchievement(userId, achievementId));
        }

        [Test]
        public void GetPointsForUnlockedAchievement_ThrowsServiceException_WhenRepositoryFails()
        {
            // Arrange
            var fakeRepository = new FakeAchievementsRepository
            {
                ThrowOnIsUnlocked = true
            };

            // Act
            var service = new AchievementsService(fakeRepository);

            // Assert
            Assert.Throws<ServiceException>(() =>
                service.GetPointsForUnlockedAchievement(1, 1));
        }
        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownFriendshipCount_ReturnsNull()
        {
            // Act & Assert
            var result = achievementsService.GetAchievementIdByTypeAndCount(Categories.Friendships, 999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownOwnedGamesCount_ReturnsNull()
        {
            // Act & Assert
            var result = achievementsService.GetAchievementIdByTypeAndCount(Categories.OwnedGames, 999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownSoldGamesCount_ReturnsNull()
        {
            // Act & Assert
            var result = achievementsService.GetAchievementIdByTypeAndCount(Categories.SoldGames, 999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownReviewsGivenCount_ReturnsNull()
        {
            // Act & Assert
            var result = achievementsService.GetAchievementIdByTypeAndCount(Categories.NumberOfReviewsGiven, 999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownReviewsReceivedCount_ReturnsNull()
        {
            // Act & Assert
            var result = achievementsService.GetAchievementIdByTypeAndCount(Categories.NumberOfReviewsReceived, 999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownYearsOfActivityCount_ReturnsNull()
        {
            // Act & Assert
            var result = achievementsService.GetAchievementIdByTypeAndCount(Categories.YearsOfActivity, 999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownPostsCount_ReturnsNull()
        {
            // Act & Assert
            var result = achievementsService.GetAchievementIdByTypeAndCount(Categories.NumberOfPosts, 999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithInvalidDeveloperCount_ReturnsNull()
        {
            // Act & Assert
            var result = achievementsService.GetAchievementIdByTypeAndCount(Categories.Developer, 999); // Developer only accepts 1
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownCategory_ReturnsNull()
        {
            // Act & Assert
            var result = achievementsService.GetAchievementIdByTypeAndCount("Unknown Type", 1);
            Assert.That(result, Is.Null);
        }
    }

    [TestFixture]
    public class AchievementServiceTests_GetAchievementIdByTypeAndCount
    {
        [TestCase("Friendships", 1, "FRIENDSHIP1")]
        [TestCase("Friendships", 5, "FRIENDSHIP2")]
        [TestCase("Friendships", 10, "FRIENDSHIP3")]
        [TestCase("Friendships", 50, "FRIENDSHIP4")]
        [TestCase("Friendships", 100, "FRIENDSHIP5")]

        [TestCase("Owned Games", 1, "OWNEDGAMES1")]
        [TestCase("Owned Games", 5, "OWNEDGAMES2")]
        [TestCase("Owned Games", 10, "OWNEDGAMES3")]
        [TestCase("Owned Games", 50, "OWNEDGAMES4")]

        [TestCase("Sold Games", 1, "SOLDGAMES1")]
        [TestCase("Sold Games", 5, "SOLDGAMES2")]
        [TestCase("Sold Games", 10, "SOLDGAMES3")]
        [TestCase("Sold Games", 50, "SOLDGAMES4")]

        [TestCase("Number of Reviews Given", 1, "REVIEW1")]
        [TestCase("Number of Reviews Given", 5, "REVIEW2")]
        [TestCase("Number of Reviews Given", 10, "REVIEW3")]
        [TestCase("Number of Reviews Given", 50, "REVIEW4")]

        [TestCase("Number of Reviews Received", 1, "REVIEWR1")]
        [TestCase("Number of Reviews Received", 5, "REVIEWR2")]
        [TestCase("Number of Reviews Received", 10, "REVIEWR3")]
        [TestCase("Number of Reviews Received", 50, "REVIEWR4")]

        [TestCase("Years of Activity", 1, "ACTIVITY1")]
        [TestCase("Years of Activity", 2, "ACTIVITY2")]
        [TestCase("Years of Activity", 3, "ACTIVITY3")]
        [TestCase("Years of Activity", 4, "ACTIVITY4")]

        [TestCase("Number of Posts", 1, "POSTS1")]
        [TestCase("Number of Posts", 5, "POSTS2")]
        [TestCase("Number of Posts", 10, "POSTS3")]
        [TestCase("Number of Posts", 50, "POSTS4")]

        [TestCase("Developer", 1, "DEVELOPER")]

        public void GetAchievementIdByTypeAndCount_ReturnsExpectedId(string type, int count, string expectedName)
        {
            // Arrange
            var mockRepo = new Mock<IAchievementsRepository>();
            mockRepo.Setup(repository => repository.GetAchievementIdByName(expectedName)).Returns(123);

            var service = new AchievementsService(mockRepo.Object);

            // Act
            var result = service.GetAchievementIdByTypeAndCount(type, count);

            // Assert
            Assert.That(result, Is.EqualTo(123));
        }
    }
}

