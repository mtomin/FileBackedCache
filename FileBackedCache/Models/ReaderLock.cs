namespace FileBackedCache.Models
{
    /// <summary>
    /// Semaphore-based shared lock.
    /// </summary>
    internal sealed class ReaderLock : LockBase, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderLock"/> class.
        /// </summary>
        /// <param name="readers">Keeps track of readers - don't start writing if any readers active.</param>
        /// <param name="readFinished">Signal to stop reading when preparing to write.</param>
        /// <param name="lockAcquired">Lock has been successfully acquired.</param>
        public ReaderLock(Semaphore readers, EventWaitHandle readFinished, bool lockAcquired = true)
            : base(lockAcquired)
        {
            Readers = readers;
            ReadFinished = readFinished;
        }

        private Semaphore Readers { get; }

        private EventWaitHandle ReadFinished { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            // signal that I'm no longer reading
            if (LockAcquired)
            {
                Readers.Release();
                ReadFinished.Set();
            }

            GC.SuppressFinalize(this);
        }
    }
}
