using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SteamHub.Api.Context;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Repositories
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private readonly DataContext context;

        public FriendRequestRepository(DataContext newContext)
        {
            context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public async Task<IEnumerable<FriendRequest>> GetFriendRequestsAsync(string username)
        {
            // Convert username to userId first
            var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return new List<FriendRequest>();
            }

            var entityResults = await context.FriendRequests
                .AsNoTracking()
                .Include(fr => fr.Sender)
                .Include(fr => fr.Receiver)
                .Where(fr => fr.ReceiverUserId == user.UserId)
                .ToListAsync();

            // Map from Entity to Model
            return entityResults.Select(entity => new FriendRequest
            {
                Username = entity.Sender?.Username ?? "",
                ReceiverUsername = entity.Receiver?.Username ?? "",
                RequestDate = entity.RequestDate
            });
        }

        public async Task<bool> AddFriendRequestAsync(FriendRequest request)
        {
            try
            {
                // Convert usernames to userIds
                var senderUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == request.Username);
                var receiverUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == request.ReceiverUsername);

                if (senderUser == null || receiverUser == null)
                {
                    return false;
                }

                // Map from Model to Entity
                var entityRequest = new SteamHub.Api.Entities.FriendRequest
                {
                    SenderUserId = senderUser.UserId,
                    ReceiverUserId = receiverUser.UserId,
                    RequestDate = request.RequestDate,
                    Status = SteamHub.Api.Entities.FriendRequestStatus.Pending // Default status
                };

                context.FriendRequests.Add(entityRequest);
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
                // Convert usernames to userIds
                var senderUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == senderUsername);
                var receiverUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == receiverUsername);

                if (senderUser == null || receiverUser == null)
                {
                    return false;
                }

                var entity = await context.FriendRequests
                    .FirstOrDefaultAsync(fr => fr.SenderUserId == senderUser.UserId && fr.ReceiverUserId == receiverUser.UserId);

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