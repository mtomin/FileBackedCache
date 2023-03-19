namespace FileBackedCache.Implementation
{
    using System;
    using FileBackedCache.Configuration;
    using FileBackedCache.Interfaces;
    using Microsoft.Extensions.Caching.Distributed;

    /// <summary>
    /// Thread- and process-safe cache.
    /// </summary>
    internal sealed class SafeCache : FileCacheBase, IDistributedCache
    {
        private readonly IDistributedCache _innerCache;
        private readonly ILockProvider _lockProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeCache"/> class.
        /// </summary>
        /// <param name="innerCache">Cache.</param>
        /// <param name="configuration">Cache configuration.</param>
        /// <param name="lockProvider">Concurrency handling.</param>
        public SafeCache(IDistributedCache innerCache, ICacheConfiguration configuration, ILockProvider lockProvider)
            : base(configuration)
        {
            _innerCache = innerCache;
            _lockProvider = lockProvider;
        }

        /// <inheritdoc/>
        public byte[]? Get(string key)
        {
            return TryExecuteWithReadLock(() => _innerCache.Get(key), key);
        }

        /// <inheritdoc/>
        public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            return await TryExecuteWithReadLockAsync(() => _innerCache.GetAsync(key, token), key);
        }

        /// <inheritdoc/>
        public void Refresh(string key)
        {
            TryExecuteWithWriteLock(() => _innerCache.Refresh(key), key);
        }

        /// <inheritdoc/>
        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            await TryExecuteWithWriteLockAsync(() => _innerCache.RefreshAsync(key, token), key);
        }

        /// <inheritdoc/>
        public void Remove(string key)
        {
            TryExecuteWithWriteLock(() => _innerCache.Remove(key), key);
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            await TryExecuteWithWriteLockAsync(() => _innerCache.RemoveAsync(key, token), key);
        }

        /// <inheritdoc/>
        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            TryExecuteWithWriteLock(() => _innerCache.Set(key, value, options), key);
        }

        /// <inheritdoc/>
        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            await TryExecuteWithWriteLockAsync(() => _innerCache.SetAsync(key, value, options, token), key);
        }

        private T? TryExecuteWithReadLock<T>(Func<T> action, string key)
        {
            using var @lock = _lockProvider.AcquireReadLock(GetFilePath(key));
            if (@lock.LockAcquired)
            {
                return action.Invoke();
            }

            return default;
        }

        private async Task<T?> TryExecuteWithReadLockAsync<T>(Func<Task<T>> action, string key)
        {
            using var @lock = _lockProvider.AcquireReadLock(GetFilePath(key));
            if (@lock.LockAcquired)
            {
                return await action.Invoke();
            }

            return default;
        }

        private void TryExecuteWithWriteLock(Action action, string key)
        {
            using var @lock = _lockProvider.AcquireWriteLock(GetFilePath(key));
            if (@lock.LockAcquired)
            {
                action.Invoke();
            }
        }

        private async Task TryExecuteWithWriteLockAsync(Func<Task> action, string key)
        {
            using var @lock = _lockProvider.AcquireWriteLock(GetFilePath(key));
            if (@lock.LockAcquired)
            {
                await action.Invoke();
            }
        }
    }
}
