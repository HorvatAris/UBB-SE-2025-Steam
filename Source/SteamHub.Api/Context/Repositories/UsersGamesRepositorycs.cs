using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Context;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.UsersGames;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Models.UsersGames;
using Microsoft.AspNetCore.Http.HttpResults;
public class UsersGamesRepository : IUsersGamesRepository
{
    private readonly DataContext context;
    public UsersGamesRepository(DataContext context)
    {
        this.context = context;
    }

    public async Task AddToCartAsync(UserGameRequest usersGames)
    {
        var userGames = await context.UsersGames
            .FirstOrDefaultAsync(userGame => userGame.UserId == usersGames.UserId && userGame.GameId == usersGames.GameId);

        if (userGames != null)
        {
            if(userGames.IsPurchased)
                throw new Exception("Game already purchased");
            userGames.IsInCart = true;
            await context.SaveChangesAsync();
        }
        else
        {
            var userExists = await context.Users.AnyAsync(user => user.UserId == usersGames.UserId);
            if (!userExists) throw new Exception("User not found");

            var gameExists = await context.Games.AnyAsync(game => game.GameId == usersGames.GameId);
            if (!gameExists) throw new Exception("Game not found");


            userGames = new UsersGames
            {
                UserId = usersGames.UserId,
                GameId = usersGames.GameId,
                IsInCart = true,
                IsPurchased = false,
                IsInWishlist = false
            };

            await context.UsersGames.AddAsync(userGames);
            await context.SaveChangesAsync();
        }
    }

    public async Task AddToWishlistAsync(UserGameRequest usersGames)
    {
        var currentUserGame = await context.UsersGames
            .FirstOrDefaultAsync(userGame => userGame.UserId == usersGames.UserId && userGame.GameId == usersGames.GameId);

        if (currentUserGame != null)
        {
            if (currentUserGame.IsPurchased)
                throw new Exception("Game already purchased");
            currentUserGame.IsInWishlist = true;
            await context.SaveChangesAsync();
        }
        else
        {
            var userExists = await context.Users.AnyAsync(user => user.UserId == usersGames.UserId);
            if (!userExists) throw new Exception("User not found");

            var gameExists = await context.Games.AnyAsync(game => game.GameId == usersGames.GameId);
            if (!gameExists) throw new Exception("Game not found");


            currentUserGame = new UsersGames
            {
                UserId = usersGames.UserId,
                GameId = usersGames.GameId,
                IsInCart = false,
                IsPurchased = false,
                IsInWishlist = true
            };

            await context.UsersGames.AddAsync(currentUserGame);
            await context.SaveChangesAsync();
        }
    }

    public async Task<GetUserGamesResponse> GetUserCartAsync(int userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null) throw new Exception("User not found");

        var userGames = await context.UsersGames
            .Where(userGame => userGame.UserId == userId && userGame.IsInCart)
            .Include(userGame => userGame.Game)
            .Select(userGame => new UserGamesResponse
            {
                GameId = userGame.Game.GameId,
                IsInCart = userGame.IsInCart,
                IsPurchased = userGame.IsPurchased,
                IsInWishlist = userGame.IsInWishlist
            })
            .ToListAsync();

        return new GetUserGamesResponse
        {
            UserGames = userGames
        };
    }

    public async Task<GetUserGamesResponse> GetUserGamesAsync(int userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null) throw new Exception("User not found");

        var userGames = await context.UsersGames
            .Where(userGame => userGame.UserId == userId)
            .Include(userGame => userGame.Game)
            .Select(userGame => new UserGamesResponse
            {
                GameId = userGame.Game.GameId,
                IsInCart = userGame.IsInCart,
                IsPurchased = userGame.IsPurchased,
                IsInWishlist = userGame.IsInWishlist
            })
            .ToListAsync();

        return new GetUserGamesResponse
        {
            UserGames = userGames
        };
    }

    public async Task<GetUserGamesResponse> GetUserPurchasedGamesAsync(int userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null) throw new Exception("User not found");

        var userGames = await context.UsersGames
            .Where(userGame => userGame.UserId == userId && userGame.IsPurchased)
            .Include(userGame => userGame.Game)
            .Select(userGame => new UserGamesResponse
            {
                GameId = userGame.Game.GameId,
                IsInCart = userGame.IsInCart,
                IsPurchased = userGame.IsPurchased,
                IsInWishlist = userGame.IsInWishlist

            })
            .ToListAsync();

        return new GetUserGamesResponse
        {
            UserGames = userGames
        };
    }

    public async Task<GetUserGamesResponse> GetUserWishlistAsync(int userId)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null) throw new Exception("User not found");

        var userGames = await context.UsersGames
            .Where(userGame => userGame.UserId == userId && userGame.IsInWishlist)
            .Include(userGame => userGame.Game)
            .Select(userGame => new UserGamesResponse
            {
                GameId = userGame.Game.GameId,
                IsInCart = userGame.IsInCart,
                IsPurchased = userGame.IsPurchased,
                IsInWishlist = userGame.IsInWishlist
            })
            .ToListAsync();

        return new GetUserGamesResponse
        {
            UserGames = userGames
        };
    }

    public async Task PurchaseGameAsync(UserGameRequest usersGames)
    {
        var selectedGame = await context.UsersGames
            .FirstOrDefaultAsync(userGame => userGame.UserId == usersGames.UserId && userGame.GameId == usersGames.GameId);

        if (selectedGame != null)
        {
            selectedGame.IsInCart = false;
            selectedGame.IsPurchased = true;
            selectedGame.IsInWishlist = false;
            await context.SaveChangesAsync();
        }
        else
        {
            var userExists = await context.Users.AnyAsync(u => u.UserId == usersGames.UserId);
            if (!userExists) throw new Exception("User not found");

            var gameExists = await context.Games.AnyAsync(g => g.GameId == usersGames.GameId);
            if (!gameExists) throw new Exception("Game not found");


            selectedGame = new UsersGames
            {
                UserId = usersGames.UserId,
                GameId = usersGames.GameId,
                IsInCart = true,
                IsPurchased = false,
                IsInWishlist = false
            };

            await context.UsersGames.AddAsync(selectedGame);
            await context.SaveChangesAsync();
        }
    }

    public async Task RemoveFromCartAsync(UserGameRequest usersGames)
    {
        var userExists = await context.Users.AnyAsync(user => user.UserId == usersGames.UserId);
        if (!userExists) throw new Exception("User not found");

        var gameExists = await context.Games.AnyAsync(game => game.GameId == usersGames.GameId);
        if (!gameExists) throw new Exception("Game not found");

        var deleteGame = await context.UsersGames
            .FirstOrDefaultAsync(userGame => userGame.UserId == usersGames.UserId && userGame.GameId == usersGames.GameId);
        if (deleteGame != null && deleteGame.IsInCart)
        {
            deleteGame.IsInCart = false;
            await context.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Game not found in cart");
        }
    }

    public async Task RemoveFromWishlistAsync(UserGameRequest usersGames)
    {
        var userExists = await context.Users.AnyAsync(u => u.UserId == usersGames.UserId);
        if (!userExists) throw new Exception("User not found");

        var gameExists = await context.Games.AnyAsync(g => g.GameId == usersGames.GameId);
        if (!gameExists) throw new Exception("Game not found");

        var removeGame = context.UsersGames
            .FirstOrDefault(userGame => userGame.UserId == usersGames.UserId && userGame.GameId == usersGames.GameId);
        if (removeGame != null && removeGame.IsInWishlist)
        {
            removeGame.IsInWishlist = false;
            await context.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Game not found in wishlist");
        }
    }
}
