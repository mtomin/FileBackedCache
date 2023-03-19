namespace FileBackedCache.Tests
{
    using System.Text.Json;
    using FileBackedCache.Models;
    using Microsoft.Extensions.Caching.Distributed;

    internal static class CacheModels
    {
        private const int SlidingExpirationMinutes = 10;
        private static readonly TimeSpan SlidingExpiration = TimeSpan.FromMinutes(SlidingExpirationMinutes);

        internal static CacheEntry WithExpiredDuration =>
            CreateModel(m => m.ExpirationTime = DateTime.UtcNow.Add(-SlidingExpiration));

        internal static CacheEntry WithValidDuration =>
            CreateModel(
                m => m.ExpirationTime = DateTime.UtcNow.Add(SlidingExpiration),
                o => o.SlidingExpiration = SlidingExpiration);

        internal static CacheEntry WithValidDuration_NearAbsoluteDuration =>
           CreateModel(
               adjustModel: m =>
               {
                   var now = DateTime.UtcNow;
                   m.ExpirationTime = now.Add(SlidingExpiration);
               },
               adjustOptions: o =>
               {
                   o.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(SlidingExpirationMinutes - 1);
                   o.SlidingExpiration = SlidingExpiration;
               });

        internal static CacheEntry WithNoExpiry => CreateModel();

        private static CacheEntry CreateModel(
            Action<CacheEntry>? adjustModel = null,
            Action<DistributedCacheEntryOptions>? adjustOptions = null)
        {
            var value = JsonSerializer.SerializeToUtf8Bytes("test");
            var options = new DistributedCacheEntryOptions();
            adjustOptions?.Invoke(options);
            var model = new CacheEntry(value, options);
            adjustModel?.Invoke(model);
            return model;
        }
    }
}
