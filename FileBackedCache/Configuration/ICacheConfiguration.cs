namespace FileBackedCache.Configuration
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Configuration for the simple cache implementation.
    /// </summary>
    public interface ICacheConfiguration
    {
        /// <summary>
        /// Root folder for storing cache entries.
        /// </summary>
        public string RootFolder { get; }

        /// <summary>
        /// Service lifetime.
        /// </summary>
        public ServiceLifetime ServiceLifetime { get; }
    }
}
