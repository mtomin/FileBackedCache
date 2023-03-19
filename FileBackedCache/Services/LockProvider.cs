namespace FileBackedCache.Services
{
    using System.Diagnostics;
    using FileBackedCache.Configuration;
    using FileBackedCache.Interfaces;
    using FileBackedCache.Models;

    /// <summary>
    /// Provide thread- and process-safe locks for reading/writing files.
    /// </summary>
    internal class LockProvider : ILockProvider
    {
        private readonly ILockConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="LockProvider"/> class.
        /// </summary>
        /// <param name="configuration">Cache configuration.</param>
        public LockProvider(ILockConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Acquires a shared lock based on a semaphore for readers. Blocking readers when writing is handled through EventWaitHandles.
        /// </summary>
        /// <param name="filePath">File path of the locked file (used as unique identifier).</param>
        /// <returns>A shared reader lock.</returns>
        public ReaderLock AcquireReadLock(string filePath)
        {
            EventWaitHandle readAllowed = new(true, EventResetMode.ManualReset, ReadAllowedHandleName(filePath));
            EventWaitHandle readFinished = new(false, EventResetMode.ManualReset, ReadFinishedHandleName(filePath));
            Semaphore readers = new(int.MaxValue, int.MaxValue, ReaderSemaphoreName(filePath));

            var sw = new Stopwatch();
            sw.Start();

            while (true)
            {
                if (sw.Elapsed > _configuration.LockTimeout)
                {
                    return new ReaderLock(readers, readFinished, lockAcquired: false);
                }

                // signal that I'm reading.
                readers.WaitOne();

                // check whether I'm actually allowed to read
                if (readAllowed.WaitOne(0))
                {
                    return new ReaderLock(readers, readFinished); // great!
                }

                // oops, nevermind, signal that I'm not reading
                readers.Release();
                readFinished.Set();

                // block until it's ok to read
                readAllowed.WaitOne();
            }
        }

        /// <summary>
        /// Acquires an exclusive lock based on a semaphore for writers (max 1). Blocking readers when writing is handled through EventWaitHandles.
        /// </summary>
        /// <param name="filePath">File path of the locked file (used as unique identifier).</param>
        /// <returns>An exclusive writer lock.</returns>
        public WriterLock AcquireWriteLock(string filePath)
        {
            EventWaitHandle readAllowed = new(true, EventResetMode.ManualReset, ReadAllowedHandleName(filePath));
            EventWaitHandle readFinished = new(false, EventResetMode.ManualReset, ReadFinishedHandleName(filePath));
            Semaphore writer = new(1, 1, WriterSemaphoreName(filePath));
            Semaphore readers = new(int.MaxValue, int.MaxValue, ReaderSemaphoreName(filePath));

            var sw = new Stopwatch();
            sw.Start();

            // block until I am the only writer
            try
            {
                writer.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                // The mutex was abandoned in another process, but it was still acquired
                // TODO: log?
            }

            // signal that readers need to cease
            readAllowed.Reset();

            // loop until there are no readers
            int readerCount = -1;
            while (readerCount != 0)
            {
                if (sw.Elapsed > _configuration.LockTimeout)
                {
                    return new WriterLock(readAllowed, writer, lockAcquired: false);
                }

                // wipe the knowledge that a reader recently finished
                readFinished.Reset();

                // check if there is a reader
                readers.WaitOne();
                readerCount = int.MaxValue - (readers.Release() + 1);
                if (readerCount > 0)
                {
                    // block until some reader finishes
                    readFinished.WaitOne();
                }
            }

            return new WriterLock(readAllowed, writer);
        }

        private string ReadAllowedHandleName(string filePath)
            => $"{nameof(FileBackedCache)}-readAllowed:{filePath}".Replace(Path.DirectorySeparatorChar, '_');

        private string ReadFinishedHandleName(string filePath)
            => $"{nameof(FileBackedCache)}-readFinished:{filePath}".Replace(Path.DirectorySeparatorChar, '_');

        private string ReaderSemaphoreName(string filePath)
            => $"{nameof(FileBackedCache)}_readers:{filePath.Replace(Path.DirectorySeparatorChar, '_')}";

        private string WriterSemaphoreName(string filePath)
           => $"{nameof(FileBackedCache)}_writer:{filePath.Replace(Path.DirectorySeparatorChar, '_')}";
    }
}
