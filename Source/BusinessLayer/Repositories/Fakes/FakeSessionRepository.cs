using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories.Fakes
{
    internal class FakeSessionRepository : ISessionRepository
    {
        public SessionDetails CreateSession(int userId)
        {
            SessionDetails sessionDetails = new SessionDetails();
            sessionDetails.UserId = userId;
            return sessionDetails;
        }

        public void DeleteSession(Guid sessionId)
        {
            // throws exception
        }

        public void DeleteUserSessions(int userId)
        {
            // throws exception
        }

        public List<Guid> GetExpiredSessions()
        {
            List<Guid> guids = new List<Guid>();
            Guid guid = Guid.Empty;
            guids.Add(guid);
            return guids;
        }

        public SessionDetails GetSessionById(Guid sessionId)
        {
            SessionDetails sessionDetails = new SessionDetails
            {
                SessionId = sessionId,
            };
            return sessionDetails;
        }

        public UserWithSessionDetails GetUserFromSession(Guid sessionId)
        {
            UserWithSessionDetails userWithSessionDetails = new UserWithSessionDetails();
            userWithSessionDetails.SessionId = sessionId;
            return userWithSessionDetails;
        }
    }
}
