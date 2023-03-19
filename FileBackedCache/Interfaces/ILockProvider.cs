namespace FileBackedCache.Interfaces
{
    using FileBackedCache.Models;

    /// <summary>
    /// Provide a locking mechanism to handle concurrent file reads/writes in a distributed system.
    /// </summary>
    internal interface ILockProvider
    {
        /// <summary>
        /// Acquire a shared reader lock.
        /// </summary>
        /// <param name="filePath">Path to file which should be locked.</param>
        /// <returns>Lock object that releases locks on dispose.</returns>
        ReaderLock AcquireReadLock(string filePath);

        /// <summary>
        /// Acquire an exclusive writer lock.
        /// </summary>
        /// <returns>Lock object that releases locks on dispose.</returns>
        /// <param name="filePath">Path to file which should be locked.</param>
        WriterLock AcquireWriteLock(string filePath);
    }
}
