using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.DataContext;
using BusinessLayer.Exceptions;

public class UsersRepositoryTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public void GetAllUsers_ReturnsSortedUsers()
    {
        var context = GetInMemoryDbContext();
        context.Users.AddRange(new User { Username = "Zoe" }, new User { Username = "Amy" });
        context.SaveChanges();

        var repo = new UsersRepository(context);
        var result = repo.GetAllUsers();

        Assert.Equal("Amy", result.First().Username);
    }

    [Fact]
    public void GetUserById_ReturnsUser()
    {
        var context = GetInMemoryDbContext();
        var user = new User { UserId = 1, Username = "Test" };
        context.Users.Add(user);
        context.SaveChanges();

        var repo = new UsersRepository(context);
        var result = repo.GetUserById(1);

        Assert.Equal("Test", result.Username);
    }

    [Fact]
    public void GetUserById_NonExistent_ReturnsNull()
    {
        var repo = new UsersRepository(GetInMemoryDbContext());
        var result = repo.GetUserById(999);

        Assert.Null(result);
    }

    [Fact]
    public void UpdateProfileBio_ValidUser_UpdatesBio()
    {
        var context = GetInMemoryDbContext();
        context.Users.Add(new User { UserId = 1, Username = "test" });
        context.SaveChanges();

        var repo = new UsersRepository(context);
        repo.UpdateProfileBio(1, "new bio");

        Assert.Equal("new bio", context.Users.First().Bio);
    }

    [Fact]
    public void UpdateProfileBio_InvalidUser_Throws()
    {
        var repo = new UsersRepository(GetInMemoryDbContext());

        Assert.Throws<RepositoryException>(() => repo.UpdateProfileBio(1, "bio"));
    }

    [Fact]
    public void UpdateUser_ValidUser_UpdatesData()
    {
        var context = GetInMemoryDbContext();
        var user = new User { UserId = 1, Email = "old@mail.com", Username = "old" };
        context.Users.Add(user);
        context.SaveChanges();

        var repo = new UsersRepository(context);
        var updated = new User { UserId = 1, Email = "new@mail.com", Username = "new" };
        var result = repo.UpdateUser(updated);

        Assert.Equal("new@mail.com", result.Email);
    }

    [Fact]
    public void UpdateUser_InvalidUser_Throws()
    {
        var repo = new UsersRepository(GetInMemoryDbContext());
        var user = new User { UserId = 999 };

        Assert.Throws<RepositoryException>(() => repo.UpdateUser(user));
    }

    [Fact]
    public void CreateUser_AddsUserToContext()
    {
        var context = GetInMemoryDbContext();
        var repo = new UsersRepository(context);

        var user = new User { Email = "mail@mail.com", Username = "user" };
        var result = repo.CreateUser(user);

        Assert.Single(context.Users);
    }

    [Fact]
    public void DeleteUser_RemovesUser()
    {
        var context = GetInMemoryDbContext();
        var user = new User { UserId = 1 };
        context.Users.Add(user);
        context.SaveChanges();

        var repo = new UsersRepository(context);
        repo.DeleteUser(1);

        Assert.Empty(context.Users);
    }

    [Fact]
    public void DeleteUser_NonExistent_DoesNothing()
    {
        var context = GetInMemoryDbContext();
        var repo = new UsersRepository(context);

        repo.DeleteUser(99);

        Assert.Empty(context.Users);
    }

    [Fact]
    public void GetUserByEmail_ReturnsUser()
    {
        var context = GetInMemoryDbContext();
        context.Users.Add(new User { Email = "mail@test.com" });
        context.SaveChanges();

        var repo = new UsersRepository(context);
        var result = repo.GetUserByEmail("mail@test.com");

        Assert.NotNull(result);
    }

    [Fact]
    public void GetUserByEmail_NotFound_ReturnsNull()
    {
        var repo = new UsersRepository(GetInMemoryDbContext());
        var result = repo.GetUserByEmail("x@x.com");

        Assert.Null(result);
    }

    [Fact]
    public void ChangeEmail_ValidUser_UpdatesEmail()
    {
        var context = GetInMemoryDbContext();
        context.Users.Add(new User { UserId = 1, Email = "old@mail.com" });
        context.SaveChanges();

        var repo = new UsersRepository(context);
        repo.ChangeEmail(1, "new@mail.com");

        Assert.Equal("new@mail.com", context.Users.First().Email);
    }

    [Fact]
    public void ChangeEmail_InvalidUser_Throws()
    {
        var repo = new UsersRepository(GetInMemoryDbContext());
        Assert.Throws<RepositoryException>(() => repo.ChangeEmail(1, "mail"));
    }
}