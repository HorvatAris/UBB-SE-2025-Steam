using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BusinessLayer.Data;
using BusinessLayer.DataContext;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

/* This is basically useless because we already have a class FriendshipsRepository implementing the same thing. Who had the task to merge the apps??? */
/* No reason to change this repository to use EF Core when we have another repository doing the same thing. */
/* TODO: remove this entire part of the program with its service and viewmodels OR remove the other one (which one is better) */

namespace BusinessLayer.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly ApplicationDbContext context;

        public FriendRepository(ApplicationDbContext newContext)
        {
            context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public async Task<IEnumerable<Friend>> GetFriendsAsync(string username)
        {
            // find all friendship entries where user is either side
            var entries = await context.FriendsTable
                .AsNoTracking()
                .Where(f => f.User1Username == username || f.User2Username == username)
                .ToListAsync();

            var result = new List<Friend>();
            foreach (var e in entries)
            {
                var otherUsername = e.User1Username == username ? e.User2Username : e.User1Username;
                var user = await context.Users.FirstOrDefaultAsync(u => u.Username == otherUsername);
                if (user == null)
                {
                    continue;
                }
                var profile = await context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.UserId);

                result.Add(new Friend
                {
                    Username = user.Username,
                    Email = user.Email,
                    ProfilePhotoPath = profile?.ProfilePicture ?? string.Empty
                });
            }
            return result;
        }

        public async Task<bool> AddFriendAsync(string user1Username, string user2Username, string friendEmail, string friendProfilePhotoPath)
        {
            try
            {
                var (first, second) = string.Compare(user1Username, user2Username, StringComparison.Ordinal) <= 0
                    ? (user1Username, user2Username)
                    : (user2Username, user1Username);

                if (await context.FriendsTable.AnyAsync(f => f.User1Username == first && f.User2Username == second))
                {
                    return false;
                }

                context.FriendsTable.Add(new FriendEntity { User1Username = first, User2Username = second });
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveFriendAsync(string user1Username, string user2Username)
        {
            try
            {
                var (first, second) = string.Compare(user1Username, user2Username, StringComparison.Ordinal) <= 0
                    ? (user1Username, user2Username)
                    : (user2Username, user1Username);

                var entry = await context.FriendsTable
                    .FirstOrDefaultAsync(f => f.User1Username == first && f.User2Username == second);
                if (entry == null)
                {
                    return false;
                }

                context.FriendsTable.Remove(entry);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}