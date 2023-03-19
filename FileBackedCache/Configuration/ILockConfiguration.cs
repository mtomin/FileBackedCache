namespace FileBackedCache.Configuration
{
    /// <summary>
    /// Configuration for the thread-safe cache implementation.
    /// </summary>
    public interface ILockConfiguration
    {
        /// <summary>
        /// Timeout for trying to acquire a lock.
        /// </summary>
        public TimeSpan LockTimeout { get; set; }
    }
}
