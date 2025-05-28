namespace SteamHub.ApiContract.Models.Session;
using System;
using System.Threading.Tasks;
using System.Threading;

public sealed class UserSession
{
    private static UserSession? instance;
    private static readonly object LockObject = new object();
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private static readonly TimeSpan _semaphoreTimeout = TimeSpan.FromSeconds(5);

    private UserSession()
    {
        CurrentSessionId = null;
        UserId = 0;
        CreatedAt = DateTime.MinValue;
        ExpiresAt = DateTime.MinValue;
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

    public async Task<bool> UpdateSessionAsync(Guid sessionId, int userId, DateTime createdTime, DateTime expireTime)
    {
        if (await _semaphore.WaitAsync(_semaphoreTimeout))
        {
            try
            {
                CurrentSessionId = sessionId;
                UserId = userId;
                CreatedAt = createdTime;
                ExpiresAt = expireTime;
                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        return false;
    }

    public async Task<bool> ClearSessionAsync()
    {
        if (await _semaphore.WaitAsync(_semaphoreTimeout))
        {
            try
            {
                CurrentSessionId = null;
                UserId = 0;
                CreatedAt = DateTime.MinValue;
                ExpiresAt = DateTime.MinValue;
                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        return false;
    }

    public async Task<bool> IsSessionValidAsync()
    {
        if (await _semaphore.WaitAsync(_semaphoreTimeout))
        {
            try
            {
                return CurrentSessionId.HasValue && 
                       UserId > 0 && 
                       CreatedAt != DateTime.MinValue && 
                       ExpiresAt != DateTime.MinValue && 
                       DateTime.UtcNow < ExpiresAt;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        return false;
    }

    // For backward compatibility - consider these methods deprecated
    public bool UpdateSession(Guid sessionId, int userId, DateTime createdTime, DateTime expireTime)
    {
        if (_semaphore.Wait(_semaphoreTimeout))
        {
            try
            {
                CurrentSessionId = sessionId;
                UserId = userId;
                CreatedAt = createdTime;
                ExpiresAt = expireTime;
                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        return false;
    }

    public bool ClearSession()
    {
        if (_semaphore.Wait(_semaphoreTimeout))
        {
            try
            {
                CurrentSessionId = null;
                UserId = 0;
                CreatedAt = DateTime.MinValue;
                ExpiresAt = DateTime.MinValue;
                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        return false;
    }

    public bool IsSessionValid()
    {
        if (_semaphore.Wait(_semaphoreTimeout))
        {
            try
            {
                return CurrentSessionId.HasValue && 
                       UserId > 0 && 
                       CreatedAt != DateTime.MinValue && 
                       ExpiresAt != DateTime.MinValue && 
                       DateTime.UtcNow < ExpiresAt;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        return false;
    }

    public async Task<SessionInfo> GetSessionInfoAsync()
    {
        if (await _semaphore.WaitAsync(_semaphoreTimeout))
        {
            try
            {
                return new SessionInfo
                {
                    SessionId = CurrentSessionId,
                    UserId = UserId,
                    CreatedAt = CreatedAt,
                    ExpiresAt = ExpiresAt
                };
            }
            finally
            {
                _semaphore.Release();
            }
        }
        throw new TimeoutException("Could not acquire lock to read session info");
    }
}

public class SessionInfo
{
    public Guid? SessionId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}