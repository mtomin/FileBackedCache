namespace FileBackedCache.Implementation
{
    using FileBackedCache.Configuration;
    using FileBackedCache.Interfaces;
    using FileBackedCache.Models;
    using Microsoft.Extensions.Caching.Distributed;

    /// <summary>
    /// File-based cache; stores each cache entry as a separate file.
    /// </summary>
    internal class Cache : FileCacheBase, IDistributedCache
    {
        private readonly IFileProvider _fileProvider;
        private readonly ISerializationProvider _serialization;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache"/> class.
        /// </summary>
        /// <param name="fileProvider">Provides filesystem access.</param>
        /// <param name="serialization">Provides serialization.</param>
        /// <param name="configuration">Cache configuration.</param>
        public Cache(IFileProvider fileProvider, ISerializationProvider serialization, ICacheConfiguration configuration)
            : base(configuration)
        {
            _fileProvider = fileProvider;
            _serialization = serialization;
        }

        /// <inheritdoc/>
        public byte[]? Get(string key)
        {
            var cacheEntry = GetCacheEntry(key);
            return cacheEntry?.Value;
        }

        /// <inheritdoc/>
        public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            var cacheEntry = await GetCacheEntryAsync(key, token);
            return cacheEntry?.Value;
        }

        /// <inheritdoc/>
        public void Refresh(string key)
        {
            var cacheEntry = GetCacheEntry(key);
            if (cacheEntry != null)
            {
                RefreshCacheExpiration(cacheEntry);
                var path = GetFilePath(key);
                _fileProvider.Write(path, _serialization.Serialize(cacheEntry));
            }
        }

        /// <inheritdoc/>
        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            var cacheEntry = await GetCacheEntryAsync(key, token);
            if (cacheEntry != null)
            {
                RefreshCacheExpiration(cacheEntry);
                var path = GetFilePath(key);
                await _fileProvider.WriteAsync(path, _serialization.Serialize(cacheEntry), token);
            }
        }

        /// <inheritdoc/>
        public void Remove(string key)
        {
            var path = GetFilePath(key);
            _fileProvider.Delete(path);
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            var path = GetFilePath(key);
            _fileProvider.Delete(path);

            await Task.Yield();
        }

        /// <inheritdoc/>
        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            var cacheEntry = new CacheEntry(value, options);
            _fileProvider.Write(path: GetFilePath(key), data: _serialization.Serialize(cacheEntry));
        }

        /// <inheritdoc/>
        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            var cacheEntry = new CacheEntry(value, options);
            await _fileProvider.WriteAsync(path: GetFilePath(key), data: _serialization.Serialize(cacheEntry), token: token);
        }

        private static void RefreshCacheExpiration(CacheEntry cacheEntry)
        {
            if (cacheEntry.ExpirationTime.HasValue && cacheEntry.SlidingExpiration.HasValue)
            {
                var newExpirationTime = DateTime.UtcNow.Add(cacheEntry.SlidingExpiration.Value);
                cacheEntry.ExpirationTime = newExpirationTime > cacheEntry.AbsoluteExpirationTime ? cacheEntry.AbsoluteExpirationTime : newExpirationTime;
            }
        }

        private CacheEntry? GetCacheEntryIfNotExpired(byte[]? data)
        {
            if (data is null)
            {
                return null;
            }

            var result = _serialization.Deserialize<CacheEntry>(data);
            return result?.ExpirationTime == null || result.ExpirationTime > DateTime.UtcNow ? result : null;
        }

        private async Task<CacheEntry?> GetCacheEntryAsync(string key, CancellationToken token)
        {
            var path = GetFilePath(key);
            var content = await _fileProvider.ReadAsync(path, token);
            return GetCacheEntryIfNotExpired(content);
        }

        private CacheEntry? GetCacheEntry(string key)
        {
            var path = GetFilePath(key);
            var content = _fileProvider.Read(path);
            return GetCacheEntryIfNotExpired(content);
        }
    }
}