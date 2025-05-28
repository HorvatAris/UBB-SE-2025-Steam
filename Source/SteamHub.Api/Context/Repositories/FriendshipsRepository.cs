using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Repositories;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace SteamHub.Api.Context.Repositories
{
    public class FriendshipsRepository : IFriendshipsRepository
    {
        private readonly DataContext context;

        public FriendshipsRepository(DataContext newContext)
        {
            this.context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public async Task<List<Friendship>> GetAllFriendshipsAsync(int userIdentifier)
        {
            try
            {
                var query = from f in context.Friendships
                            join u in context.Users on
                                (f.FriendId == userIdentifier ? f.UserId : f.FriendId) equals u.UserId
                            join p in context.UserProfiles on
                                (f.FriendId == userIdentifier ? f.UserId : f.FriendId) equals p.UserId
                            where f.UserId == userIdentifier || f.FriendId == userIdentifier
                            orderby u.Username
                            select new Friendship
                            {
                                FriendshipId = f.FriendshipId,
                                UserId = f.UserId,
                                FriendId = f.FriendId,
                                FriendUsername = u.Username,
                                FriendProfilePicture = p.profilePhotoPath
                            };

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error occurred while retrieving friendships.", ex);
            }
        }

        public async Task AddFriendshipAsync(int userIdentifier, int friendUserIdentifier)
        {
            try
            {
                if (!await context.Users.AnyAsync(u => u.UserId == userIdentifier))
                {
                    throw new RepositoryException($"User {userIdentifier} does not exist.");
                }
                if (!await context.Users.AnyAsync(u => u.UserId == friendUserIdentifier))
                {
                    throw new RepositoryException($"User {friendUserIdentifier} does not exist.");
                }

                if (await context.Friendships.AnyAsync(f => f.UserId == userIdentifier && f.FriendId == friendUserIdentifier))
                {
                    throw new RepositoryException($"Friendship already exists between {userIdentifier} and {friendUserIdentifier}.");
                }

                var friendship = new SteamHub.Api.Entities.Friendship
                {
                    UserId = userIdentifier,
                    FriendId = friendUserIdentifier
                };

                await context.Friendships.AddAsync(friendship);
                await context.SaveChangesAsync();
            }
            catch (RepositoryException)
            {
                throw;
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("Failed to add friendship", generalException);
            }
        }

        public async Task<Friendship> GetFriendshipByIdAsync(int friendshipIdentifier)
        {
            try
            {
                var friendship = await context.Friendships.FindAsync(friendshipIdentifier)
                    ?? throw new RepositoryException($"Friendship with ID {friendshipIdentifier} not found.");

                return new Friendship
                {
                    FriendshipId = friendship.FriendshipId,
                    UserId = friendship.UserId,
                    FriendId = friendship.FriendId
                };
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("Failed to retrieve friendship", generalException);
            }
        }

        public async Task RemoveFriendshipAsync(int friendshipIdentifier)
        {
            try
            {
                var friendship = await context.Friendships.FindAsync(friendshipIdentifier);
                if (friendship == null)
                {
                    throw new RepositoryException($"Friendship with ID {friendshipIdentifier} not found.");
                }

                context.Friendships.Remove(friendship);
                await context.SaveChangesAsync();
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("Failed to remove friendship", generalException);
            }
        }

        public async Task<int> GetFriendshipCountAsync(int userIdentifier)
        {
            try
            {
                return await context.Friendships.CountAsync(f => f.UserId == userIdentifier);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("Failed to count friendships", generalException);
            }
        }

        public async Task<int?> GetFriendshipIdAsync(int userIdentifier, int friendIdentifier)
        {
            try
            {
                return await context.Friendships
                    .Where(f => f.UserId == userIdentifier && f.FriendId == friendIdentifier)
                    .Select(f => (int?)f.FriendshipId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("Failed to retrieve friendship ID", generalException);
            }
        }

        public async Task<bool> AddFriendAsync(string user1Username, string user2Username, string friendEmail, string friendProfilePhotoPath)
        {
            try
            {
                var (first, second) = string.Compare(user1Username, user2Username, StringComparison.Ordinal) <= 0
                    ? (user1Username, user2Username)
                    : (user2Username, user1Username);

                if (await context.FriendsTable.AnyAsync(f => f.User1.Username == first && f.User2.Username == second))
                {
                    return false;
                }

                var user1 = await context.Users.FirstOrDefaultAsync(u => u.Username == user1Username);
                var user2 = await context.Users.FirstOrDefaultAsync(u => u.Username == user2Username);

                if (user1 == null || user2 == null)
                {
                    return false;
                }

                await context.FriendsTable.AddAsync(new SteamHub.Api.Entities.FriendEntity
                {
                    User1 = user1,
                    User2 = user2,
                    User1Id = user1.UserId,
                    User2Id = user2.UserId,
                    CreatedDate = DateTime.Now
                });

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to add friend by username", ex);
            }
        }
    }
}
