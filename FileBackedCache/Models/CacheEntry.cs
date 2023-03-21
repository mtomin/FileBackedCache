using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;

[assembly: InternalsVisibleTo("FileBackedCache.Tests")]

namespace FileBackedCache.Models
{
    /// <summary>
    /// Class wrapping the cached value with expiration date/sliding expiration window.
    /// </summary>
    internal class CacheEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheEntry"/> class.
        /// Not to be called directly; used solely by the deserializer.
        /// </summary>
        [JsonConstructor]
        public CacheEntry(byte[] value, TimeSpan? slidingExpiration, DateTime? expirationTime, DateTime? absoluteExpirationTime)
        {
            Value = value;
            SlidingExpiration = slidingExpiration;
            ExpirationTime = expirationTime;
            AbsoluteExpirationTime = absoluteExpirationTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheEntry"/> class.
        /// </summary>
        /// <param name="value">Value to be persisted in cache.</param>
        /// <param name="options">Cache options.</param>
        public CacheEntry(byte[] value, DistributedCacheEntryOptions options)
        {
            var now = DateTime.UtcNow;
            Value = value;
            SlidingExpiration = options.SlidingExpiration;

            if (options.SlidingExpiration.HasValue)
            {
                ExpirationTime = now.Add(options.SlidingExpiration.Value);
            }

            AbsoluteExpirationTime = CalculateAbsoluteExpirationTime(options, now);

            if (AbsoluteExpirationTime < ExpirationTime || (AbsoluteExpirationTime.HasValue && !ExpirationTime.HasValue))
            {
                ExpirationTime = AbsoluteExpirationTime;
            }
        }

        /// <summary>
        /// Underlying cache value.
        /// </summary>
        public byte[] Value { get; }

        /// <summary>
        /// Sliding expiration for the cache entry.
        /// </summary>
        public TimeSpan? SlidingExpiration { get; }

        /// <summary>
        /// Current cache entry expiration time (UTC).
        /// </summary>
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// Absolute cache entry expiration time (UTC).
        /// </summary>
        public DateTime? AbsoluteExpirationTime { get; }

        private static DateTime? CalculateAbsoluteExpirationTime(DistributedCacheEntryOptions options, DateTime now)
        {
            DateTime? relativeExpiration = options.AbsoluteExpirationRelativeToNow.HasValue ? now.Add(options.AbsoluteExpirationRelativeToNow.Value) : null;
            DateTime? absoluteExpiration = options.AbsoluteExpiration?.ToUniversalTime().DateTime;

            if (absoluteExpiration.HasValue && relativeExpiration.HasValue)
            {
                return relativeExpiration.Value < absoluteExpiration.Value ? relativeExpiration.Value : absoluteExpiration.Value;
            }
            else
            {
                return relativeExpiration ?? absoluteExpiration;
            }
        }
    }
}
