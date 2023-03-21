# FileBackedCache
File-based implementation of the [IDistributedCache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache?view=dotnet-plat-ext-7.0). Intended mostly for use in dev environment/performing smoke tests when setting up other implementations (redis/database/etc.) would be impractical. In case of distributed systems, [in-memory distributed cache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.memorydistributedcache?view=dotnet-plat-ext-7.0) does not provide required functionality.

# Overview
Cache entries are written to a pre-defined folder. File access contention is regulated mostly through [semaphores](https://learn.microsoft.com/en-us/dotnet/api/system.threading.semaphore?view=net-7.0). App-level locking is not within the IDistributedCache scope (though semaphore-based approach could be leverated for this scenario).

# Configuration

File-backed cache implementation can be registered in the service collection either through providing a folder for storing cache entries:
>builder.Services.AddFileBackedCache(<folder path>);
  
 or by fully specifying the configuration options:
>builder.Services.AddFileBackedCache(new CacheConfiguration(rootFolder: "filePath", serviceLifetime: ServiceLifetime.Singleton, lockTimeout: TimeSpan.FromSeconds(20)));
