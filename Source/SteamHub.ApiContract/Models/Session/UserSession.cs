namespace SteamHub.ApiContract.Models.Session;
using System;
using System.Threading.Tasks;
using System.Threading;

public sealed class UserSession
{
    private static UserSession? instance;
    private static readonly object LockObject = new object();
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

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

    public async Task UpdateSessionAsync(Guid sessionId, int userId, DateTime createdTime, DateTime expireTime)
    {
        await _semaphore.WaitAsync();
        try
        {
            CurrentSessionId = sessionId;
            UserId = userId;
            CreatedAt = createdTime;
            ExpiresAt = expireTime;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task ClearSessionAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            CurrentSessionId = null;
            UserId = 0;
            CreatedAt = DateTime.MinValue;
            ExpiresAt = DateTime.MinValue;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> IsSessionValidAsync()
    {
        await _semaphore.WaitAsync();
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

    // For backward compatibility
    public void UpdateSession(Guid sessionId, int userId, DateTime createdTime, DateTime expireTime)
    {
        _semaphore.Wait();
        try
        {
            CurrentSessionId = sessionId;
            UserId = userId;
            CreatedAt = createdTime;
            ExpiresAt = expireTime;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void ClearSession()
    {
        _semaphore.Wait();
        try
        {
            CurrentSessionId = null;
            UserId = 0;
            CreatedAt = DateTime.MinValue;
            ExpiresAt = DateTime.MinValue;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public bool IsSessionValid()
    {
        _semaphore.Wait();
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
}