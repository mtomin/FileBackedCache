namespace FileBackedCache.Implementation
{
    using FileBackedCache.Configuration;

    /// <summary>
    /// Base class for file-backed cache.
    /// </summary>
    public abstract class FileCacheBase
    {
        private readonly ICacheConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCacheBase"/> class.
        /// </summary>
        /// <param name="configuration">Cache configuration.</param>
        protected FileCacheBase(ICacheConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Gerenates a file path for a cache entry.
        /// </summary>
        /// <param name="key">Cache entry key.</param>
        /// <returns>File path.</returns>
        protected string GetFilePath(string key)
        {
            return Path.Combine(_configuration.RootFolder, key);
        }
    }
}
