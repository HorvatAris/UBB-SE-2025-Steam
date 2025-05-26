using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BusinessLayer.Data;
using BusinessLayer.DataContext;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Repositories
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private readonly ApplicationDbContext context;

        public FriendRequestRepository(ApplicationDbContext newContext)
        {
            context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public async Task<IEnumerable<FriendRequest>> GetFriendRequestsAsync(string username)
        {
            return await context.FriendRequests
                .AsNoTracking()
                .Where(fr => fr.ReceiverUsername == username)
                .ToListAsync();
        }

        public async Task<bool> AddFriendRequestAsync(FriendRequest request)
        {
            try
            {
                context.FriendRequests.Add(request);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteFriendRequestAsync(string senderUsername, string receiverUsername)
        {
            try
            {
                var entity = await context.FriendRequests
                    .FirstOrDefaultAsync(fr => fr.Username == senderUsername && fr.ReceiverUsername == receiverUsername);

                if (entity == null)
                {
                    return false;
                }

                context.FriendRequests.Remove(entity);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}