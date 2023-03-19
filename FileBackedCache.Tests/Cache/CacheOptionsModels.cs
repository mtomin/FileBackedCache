namespace FileBackedCache.Tests
{
    using Microsoft.Extensions.Caching.Distributed;

    internal static class CacheOptionsModels
    {
        private const int SlidingExpirationMinutes = 10;
        private static readonly TimeSpan SlidingExpiration = TimeSpan.FromMinutes(SlidingExpirationMinutes);

        public static DistributedCacheEntryOptions CacheOptions_NoExpiration => CreateOptions();

        public static DistributedCacheEntryOptions CacheOptions_WithSlidingExpiration
            => CreateOptions(o => o.SlidingExpiration = SlidingExpiration);

        public static DistributedCacheEntryOptions CacheOptions_WithAbsoluteExpiration
            => CreateOptions(o => o.AbsoluteExpiration = DateTimeOffset.UtcNow.Add(SlidingExpiration));

        public static DistributedCacheEntryOptions CacheOptions_WithAbsoluteExpirationRelativeToNow
            => CreateOptions(o => o.AbsoluteExpirationRelativeToNow = SlidingExpiration);

        public static DistributedCacheEntryOptions CacheOptions_WithAbsoluteExpirationRelativeToNowBeforeAbsolute
            => CreateOptions(o =>
            {
                o.AbsoluteExpirationRelativeToNow = SlidingExpiration;
                o.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(SlidingExpirationMinutes + 1);
            });

        public static DistributedCacheEntryOptions CacheOptions_WithAbsoluteExpirationRelativeToNowAfterAbsolute
           => CreateOptions(o =>
           {
               o.AbsoluteExpirationRelativeToNow = SlidingExpiration;
               o.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(SlidingExpirationMinutes - 1);
           });

        public static DistributedCacheEntryOptions CacheOptions_WithAbsoluteExpirationBeforeSliding
           => CreateOptions(o =>
           {
               o.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(SlidingExpirationMinutes - 1);
               o.SlidingExpiration = SlidingExpiration;
           });

        private static DistributedCacheEntryOptions CreateOptions(Action<DistributedCacheEntryOptions>? adjust = null)
        {
            var options = new DistributedCacheEntryOptions();
            adjust?.Invoke(options);
            return options;
        }
    }
}
