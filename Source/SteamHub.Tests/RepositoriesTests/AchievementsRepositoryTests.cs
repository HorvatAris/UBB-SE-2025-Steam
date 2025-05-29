using System;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.DataContext;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assert = Xunit.Assert;

namespace BusinessLayer.Tests.Repositories
{
    public class AchievementsRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext context;
        private readonly AchievementsRepository repository;

        public AchievementsRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            context = new ApplicationDbContext(options);
            repository = new AchievementsRepository(context);
        }

        public void Dispose()
        {
            context.Dispose();
        }

        [Fact]
        public async Task InsertAchievements_WhenTableEmpty_ShouldInsertRecords()
        {
            // Act
            repository.InsertAchievements();

            // Assert
            Xunit.Assert.Equal(30, await context.Achievements.CountAsync());
        }

        [Fact]
        public async Task InsertAchievements_WhenTableNotEmpty_ShouldNotInsertRecords()
        {
            // Arrange
            context.Achievements.Add(new Achievement { AchievementName = "TEST" });
            await context.SaveChangesAsync();

            // Act
            repository.InsertAchievements();

            // Assert
            Assert.Equal(1, await context.Achievements.CountAsync());
        }

        [Fact]
        public async Task IsAchievementsTableEmpty_WhenEmpty_ReturnsTrue()
        {
            // Act & Assert
            Assert.True(repository.IsAchievementsTableEmpty());
        }

        [Fact]
        public async Task IsAchievementsTableEmpty_WhenNotEmpty_ReturnsFalse()
        {
            // Arrange
            context.Achievements.Add(new Achievement());
            await context.SaveChangesAsync();

            // Act & Assert
            Assert.False(repository.IsAchievementsTableEmpty());
        }

        [Fact]
        public async Task UpdateAchievementIconUrl_WhenAchievementExists_UpdatesIcon()
        {
            // Arrange
            var achievement = new Achievement { Points = 5, Icon = "old.png" };
            context.Achievements.Add(achievement);
            await context.SaveChangesAsync();

            // Act
            repository.UpdateAchievementIconUrl(5, "new.png");

            // Assert
            Assert.Equal("new.png", context.Achievements.First().Icon);
        }

        [Fact]
        public async Task UpdateAchievementIconUrl_WhenAchievementNotExists_DoesNothing()
        {
            // Act
            repository.UpdateAchievementIconUrl(999, "new.png");

            // Assert
            Assert.Empty(context.Achievements);
        }

        [Fact]
        public async Task GetAllAchievements_ReturnsAllOrderedByPointsDesc()
        {
            // Arrange
            context.Achievements.AddRange(
                new Achievement { Points = 1 },
                new Achievement { Points = 10 },
                new Achievement { Points = 5 });
            await context.SaveChangesAsync();

            // Act
            var result = repository.GetAllAchievements();

            // Assert
            Assert.Equal(10, result.First().Points);
            Assert.Equal(1, result.Last().Points);
        }

        [Fact]
        public async Task GetUnlockedAchievementsForUser_ReturnsOnlyUsersAchievements()
        {
            // Arrange
            var achievement1 = new Achievement { AchievementId = 1 };
            var achievement2 = new Achievement { AchievementId = 2 };
            context.Achievements.AddRange(achievement1, achievement2);
            context.UserAchievements.Add(new UserAchievement { UserId = 1, AchievementId = 1 });
            await context.SaveChangesAsync();

            // Act
            var result = repository.GetUnlockedAchievementsForUser(1);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result.First().AchievementId);
        }

        [Fact]
        public async Task UnlockAchievement_WhenNotUnlocked_AddsRecord()
        {
            // Arrange
            context.Achievements.Add(new Achievement { AchievementId = 1 });
            await context.SaveChangesAsync();

            // Act
            repository.UnlockAchievement(1, 1);

            // Assert
            Assert.Single(context.UserAchievements);
        }

        [Fact]
        public async Task UnlockAchievement_WhenAlreadyUnlocked_DoesNothing()
        {
            // Arrange
            context.UserAchievements.Add(new UserAchievement { UserId = 1, AchievementId = 1 });
            await context.SaveChangesAsync();

            // Act
            repository.UnlockAchievement(1, 1);

            // Assert
            Assert.Single(context.UserAchievements);
        }

        [Fact]
        public async Task RemoveAchievement_WhenExists_RemovesRecord()
        {
            // Arrange
            context.UserAchievements.Add(new UserAchievement { UserId = 1, AchievementId = 1 });
            await context.SaveChangesAsync();

            // Act
            repository.RemoveAchievement(1, 1);

            // Assert
            Assert.Empty(context.UserAchievements);
        }

        [Fact]
        public async Task RemoveAchievement_WhenNotExists_DoesNothing()
        {
            // Act
            repository.RemoveAchievement(1, 1);

            // Assert
            Assert.Empty(context.UserAchievements);
        }

        [Fact]
        public async Task GetUnlockedDataForAchievement_WhenUnlocked_ReturnsData()
        {
            // Arrange
            var achievement = new Achievement { AchievementId = 1, AchievementName = "Test" };
            context.Achievements.Add(achievement);
            context.UserAchievements.Add(new UserAchievement
            {
                UserId = 1,
                AchievementId = 1,
                Achievement = achievement,
                UnlockedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            // Act
            var result = repository.GetUnlockedDataForAchievement(1, 1);

            // Assert
            Assert.Equal("Test", result.AchievementName);
        }

        [Fact]
        public async Task GetUnlockedDataForAchievement_WhenNotUnlocked_ReturnsNull()
        {
            // Act
            var result = repository.GetUnlockedDataForAchievement(1, 1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task IsAchievementUnlocked_WhenUnlocked_ReturnsTrue()
        {
            // Arrange
            context.UserAchievements.Add(new UserAchievement { UserId = 1, AchievementId = 1 });
            await context.SaveChangesAsync();

            // Act & Assert
            Assert.True(repository.IsAchievementUnlocked(1, 1));
        }

        [Fact]
        public async Task IsAchievementUnlocked_WhenNotUnlocked_ReturnsFalse()
        {
            // Act & Assert
            Assert.False(repository.IsAchievementUnlocked(1, 1));
        }

        [Fact]
        public async Task GetAchievementsWithStatusForUser_MarksUnlockedAchievements()
        {
            // Arrange
            context.Achievements.Add(new Achievement { AchievementId = 1 });
            context.UserAchievements.Add(new UserAchievement { UserId = 1, AchievementId = 1 });
            await context.SaveChangesAsync();

            // Act
            var result = repository.GetAchievementsWithStatusForUser(1);

            // Assert
            Assert.True(result.First().IsUnlocked);
        }

        [Fact]
        public async Task GetFriendshipCount_ReturnsCorrectCount()
        {
            // Arrange
            context.Friendships.AddRange(
                new Friendship { UserId = 1 },
                new Friendship { UserId = 1 },
                new Friendship { UserId = 2 });
            await context.SaveChangesAsync();

            // Act & Assert
            Assert.Equal(2, repository.GetFriendshipCount(1));
        }

        [Fact]
        public async Task GetYearsOfActivity_CalculatesCorrectYears()
        {
            // Arrange
            context.Users.Add(new User { UserId = 1, CreatedAt = DateTime.UtcNow.AddYears(-3) });
            await context.SaveChangesAsync();

            // Act & Assert
            Assert.Equal(3, repository.GetYearsOfAcftivity(1));
        }

        [Fact]
        public async Task GetAchievementIdByName_WhenExists_ReturnsId()
        {
            // Arrange
            context.Achievements.Add(new Achievement { AchievementId = 1, AchievementName = "TEST" });
            await context.SaveChangesAsync();

            // Act & Assert
            Assert.Equal(1, repository.GetAchievementIdByName("TEST"));
        }

        [Fact]
        public async Task GetAchievementIdByName_WhenNotExists_ReturnsNull()
        {
            // Act & Assert
            Assert.Null(repository.GetAchievementIdByName("MISSING"));
        }

        [Fact]
        public async Task IsUserDeveloper_WhenDeveloper_ReturnsTrue()
        {
            // Arrange
            context.Users.Add(new User { UserId = 1, IsDeveloper = true });
            await context.SaveChangesAsync();

            // Act & Assert
            Assert.True(repository.IsUserDeveloper(1));
        }

        [Fact]
        public async Task IsUserDeveloper_WhenNotDeveloper_ReturnsFalse()
        {
            // Arrange
            context.Users.Add(new User { UserId = 1, IsDeveloper = false });
            await context.SaveChangesAsync();

            // Act & Assert
            Assert.False(repository.IsUserDeveloper(1));
        }
    }
}