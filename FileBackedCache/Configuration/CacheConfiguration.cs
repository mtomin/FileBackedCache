namespace FileBackedCache.Configuration
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Cache configuration. Contains folder location, service lifetime and lock tiemout.
    /// </summary>
    public class CacheConfiguration : ICacheConfiguration, ILockConfiguration
    {
        private const ServiceLifetime DefaultServiceLifetime = ServiceLifetime.Singleton;

        private static readonly TimeSpan DefaultLockTimeout = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheConfiguration"/> class.
        /// </summary>
        public CacheConfiguration()
           : this(string.Empty, DefaultServiceLifetime, DefaultLockTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheConfiguration"/> class.
        /// </summary>
        /// <param name="rootFolder">Root folder to store cache files.</param>
        public CacheConfiguration(string rootFolder)
            : this(rootFolder, DefaultServiceLifetime, DefaultLockTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheConfiguration"/> class.
        /// </summary>
        /// <param name="rootFolder">Root folder to store cache files.</param>
        /// <param name="serviceLifetime">Cache service lifetime.</param>
        public CacheConfiguration(string rootFolder, ServiceLifetime serviceLifetime)
            : this(rootFolder, serviceLifetime, DefaultLockTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheConfiguration"/> class.
        /// </summary>
        /// <param name="rootFolder">Root folder to store cache files.</param>
        /// <param name="lockTimeout">Timeout when trying to acquire a lock.</param>
        public CacheConfiguration(string rootFolder, TimeSpan lockTimeout)
           : this(rootFolder, DefaultServiceLifetime, lockTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheConfiguration"/> class.
        /// </summary>
        /// <param name="rootFolder">Root folder to store cache files.</param>
        /// <param name="serviceLifetime">Cache service lifetime.</param>
        /// <param name="lockTimeout">Timeout when trying to acquire a lock.</param>
        public CacheConfiguration(
            string rootFolder,
            ServiceLifetime serviceLifetime,
            TimeSpan lockTimeout)
        {
            RootFolder = rootFolder;
            ServiceLifetime = serviceLifetime;
            LockTimeout = lockTimeout;
        }

        /// <inheritdoc/>
        public string RootFolder { get; protected set; }

        /// <inheritdoc/>
        public ServiceLifetime ServiceLifetime { get; protected set; }

        /// <inheritdoc/>
        public TimeSpan LockTimeout { get; set; }
    }
}
