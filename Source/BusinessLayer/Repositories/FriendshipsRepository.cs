using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.DataContext;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Repositories
{
    public class FriendshipsRepository : IFriendshipsRepository
    {
        private readonly ApplicationDbContext context;

        public FriendshipsRepository(ApplicationDbContext newContext)
        {
            this.context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public List<Friendship> GetAllFriendships(int userIdentifier)
        {
            try
            {
                var query = from f in context.Friendships
                            join u in context.Users on f.FriendId equals u.UserId
                            join p in context.UserProfiles on f.FriendId equals p.UserId
                            where f.UserId == userIdentifier
                            orderby u.Username
                            select new Friendship
                            {
                                FriendshipId = f.FriendshipId,
                                UserId = f.UserId,
                                FriendId = f.FriendId,
                                FriendUsername = u.Username,
                                FriendProfilePicture = p.ProfilePhotoPath
                            };

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error occurred while retrieving friendships.", ex);
            }
        }

        public void AddFriendship(int userIdentifier, int friendUserIdentifier)
        {
            try
            {
                if (!context.Users.Any(u => u.UserId == userIdentifier))
                {
                    throw new RepositoryException($"User {userIdentifier} does not exist.");
                }
                if (!context.Users.Any(u => u.UserId == friendUserIdentifier))
                {
                    throw new RepositoryException($"User {friendUserIdentifier} does not exist.");
                }

                if (context.Friendships.Any(f => f.UserId == userIdentifier && f.FriendId == friendUserIdentifier))
                {
                    throw new RepositoryException($"Friendship already exists between {userIdentifier} and {friendUserIdentifier}.");
                }

                var friendship = new Friendship
                {
                    UserId = userIdentifier,
                    FriendId = friendUserIdentifier
                };

                context.Friendships.Add(friendship);
                context.SaveChanges();
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

        public Friendship GetFriendshipById(int friendshipIdentifier)
        {
            try
            {
                return context.Friendships.Find(friendshipIdentifier)
                    ?? throw new RepositoryException($"Friendship with ID {friendshipIdentifier} not found.");
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("Failed to retreive friendship", generalException);
            }
        }

        public void RemoveFriendship(int friendshipIdentifier)
        {
            try
            {
                var friendship = context.Friendships.Find(friendshipIdentifier);
                if (friendship == null)
                {
                    throw new RepositoryException($"Friendship with ID {friendshipIdentifier} not found.");
                }

                context.Friendships.Remove(friendship);
                context.SaveChanges();
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("Failed to remove friendship", generalException);
            }
        }

        public int GetFriendshipCount(int userIdentifier)
        {
            try
            {
                return context.Friendships.Count(f => f.UserId == userIdentifier);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("Failed to count friendship", generalException);
            }
        }

        public int? GetFriendshipId(int userIdentifier, int friendIdentifier)
        {
            try
            {
                return context.Friendships
                    .Where(f => f.UserId == userIdentifier && f.FriendId == friendIdentifier)
                    .Select(f => (int?)f.FriendshipId)
                    .FirstOrDefault();
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("Failed to retreive friendship ID", generalException);
            }
        }

        private static Friendship MapDataRowToFriendship(DataRow friendshipDataRow)
        {
            return new Friendship(
                friendshipId: Convert.ToInt32(friendshipDataRow["friendship_id"]),
                userId: Convert.ToInt32(friendshipDataRow["user_id"]),
                friendId: Convert.ToInt32(friendshipDataRow["friend_id"]));
        }
    }
}