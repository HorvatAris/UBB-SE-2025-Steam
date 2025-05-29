using Xunit;
using System;
using System.Linq;
using BusinessLayer.DataContext;
using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using Microsoft.EntityFrameworkCore;

public class FriendshipsRepositoryTests
{
    private readonly ApplicationDbContext context;
    private readonly FriendshipsRepository repository;

    public FriendshipsRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        context = new ApplicationDbContext(options);
        repository = new FriendshipsRepository(context);
    }

    [Fact]
    public void AddFriendship_ShouldAddFriendship_WhenValid()
    {
        var user = new User { UserId = 1, Username = "A" };
        var friend = new User { UserId = 2, Username = "B" };
        context.Users.AddRange(user, friend);
        context.SaveChanges();

        repository.AddFriendship(1, 2);

        Assert.Single(context.Friendships);
    }

    [Fact]
    public void AddFriendship_ShouldThrow_WhenFriendshipExists()
    {
        var user = new User { UserId = 1 };
        var friend = new User { UserId = 2 };
        context.Users.AddRange(user, friend);
        context.Friendships.Add(new Friendship { UserId = 1, FriendId = 2 });
        context.SaveChanges();

        var ex = Assert.Throws<RepositoryException>(() => repository.AddFriendship(1, 2));
        Assert.Contains("already exists", ex.Message);
    }

    [Fact]
    public void GetAllFriendships_ShouldReturnList()
    {
        context.Users.Add(new User { UserId = 1 });
        context.Users.Add(new User { UserId = 2, Username = "X" });
        context.UserProfiles.Add(new UserProfile { UserId = 2, ProfilePhotoPath = "pic.jpg" });
        context.Friendships.Add(new Friendship { UserId = 1, FriendId = 2 });
        context.SaveChanges();

        var result = repository.GetAllFriendships(1);

        Assert.Single(result);
    }

    [Fact]
    public void GetFriendshipById_ShouldReturnFriendship_WhenExists()
    {
        context.Friendships.Add(new Friendship { FriendshipId = 1, UserId = 1, FriendId = 2 });
        context.SaveChanges();

        var result = repository.GetFriendshipById(1);

        Assert.Equal(1, result.FriendshipId);
    }

    [Fact]
    public void GetFriendshipById_ShouldThrow_WhenNotExists()
    {
        var ex = Assert.Throws<RepositoryException>(() => repository.GetFriendshipById(99));
        Assert.Contains("not found", ex.Message);
    }

    [Fact]
    public void RemoveFriendship_ShouldRemove_WhenExists()
    {
        context.Friendships.Add(new Friendship { FriendshipId = 1 });
        context.SaveChanges();

        repository.RemoveFriendship(1);

        Assert.Empty(context.Friendships);
    }

    [Fact]
    public void RemoveFriendship_ShouldThrow_WhenNotExists()
    {
        var ex = Assert.Throws<RepositoryException>(() => repository.RemoveFriendship(42));
        Assert.Contains("not found", ex.Message);
    }

    [Fact]
    public void GetFriendshipCount_ShouldReturnCorrectCount()
    {
        context.Friendships.AddRange(
            new Friendship { UserId = 1, FriendId = 2 },
            new Friendship { UserId = 1, FriendId = 3 });
        context.SaveChanges();

        var count = repository.GetFriendshipCount(1);

        Assert.Equal(2, count);
    }

    [Fact]
    public void GetFriendshipId_ShouldReturnId_WhenExists()
    {
        context.Friendships.Add(new Friendship { FriendshipId = 5, UserId = 1, FriendId = 2 });
        context.SaveChanges();

        var id = repository.GetFriendshipId(1, 2);

        Assert.Equal(5, id);
    }

    [Fact]
    public void GetFriendshipId_ShouldReturnNull_WhenNotExists()
    {
        var id = repository.GetFriendshipId(1, 2);

        Assert.Null(id);
    }
}