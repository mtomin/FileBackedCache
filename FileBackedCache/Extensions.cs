namespace FileBackedCache
{
    using FileBackedCache.Configuration;
    using FileBackedCache.Implementation;
    using FileBackedCache.Interfaces;
    using FileBackedCache.Services;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Register services in DI container.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Register dependencies for file-backed caching.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="config">Cache configuration.</param>
        /// <returns>Service collection with registered services.</returns>
        public static IServiceCollection AddFileBackedCache(this IServiceCollection services, Action<CacheConfiguration> config)
        {
            var options = new CacheConfiguration();
            config.Invoke(options);

            return services.AddFileBackedCache(options);
        }

        /// <summary>
        /// Register dependencies for file-backed caching.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="rootFolder">Folder for storing cache files.</param>
        /// <returns>Service collection with registered services.</returns>
        public static IServiceCollection AddFileBackedCache(this IServiceCollection services, string rootFolder)
        {
            var options = new CacheConfiguration(rootFolder);
            return services.AddFileBackedCache(options);
        }

        /// <summary>
        /// Register dependencies for file-backed caching.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="options">Cache configuration.</param>
        /// <returns>Service collection with registered services.</returns>
        public static IServiceCollection AddFileBackedCache(this IServiceCollection services, CacheConfiguration options)
        {
            services.AddSingleton<ILockConfiguration>(_ => options);
            services.AddSingleton<ICacheConfiguration>(_ => options);
            services.Add(new ServiceDescriptor(typeof(ISerializationProvider), typeof(SerializationProvider), options.ServiceLifetime));
            services.Add(new ServiceDescriptor(typeof(IFileProvider), typeof(FileProvider), options.ServiceLifetime));
            services.Add(new ServiceDescriptor(typeof(ILockProvider), typeof(LockProvider), options.ServiceLifetime));
            services.Add(new ServiceDescriptor(typeof(IDistributedCache), typeof(Cache), options.ServiceLifetime));
            services.Decorate<IDistributedCache, SafeCache>();

            return services;
        }
    }
}
