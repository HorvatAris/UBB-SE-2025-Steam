using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public sealed class UserSession
    {
        private static UserSession? instance;
        private static readonly object LockObject = new object();

        private UserSession()
        {
        }

        public static UserSession Instance
        {
            get
            {
                lock (LockObject)
                {
                    if (instance == null)
                    {
                        instance = new UserSession();
                    }
                    return instance;
                }
            }
        }

        public Guid? CurrentSessionId { get; private set; }
        public int UserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }

        public void UpdateSession(Guid sessionId, int userId, DateTime createdTime, DateTime expireTime)
        {
            lock (LockObject)
            {
                CurrentSessionId = sessionId;
                UserId = userId;
                CreatedAt = createdTime;
                ExpiresAt = expireTime;
            }
        }

        public void ClearSession()
        {
            lock (LockObject)
            {
                CurrentSessionId = null;
                UserId = 0;
                CreatedAt = DateTime.MinValue;
                ExpiresAt = DateTime.MinValue;
            }
        }

        public bool IsSessionValid()
        {
            lock (LockObject)
            {
                return CurrentSessionId.HasValue && UserId > 0 && CreatedAt != DateTime.MinValue && ExpiresAt != DateTime.MinValue && DateTime.Now < ExpiresAt;
            }
        }
    }
}