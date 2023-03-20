namespace FileBackedCache.Models
{
    /// <summary>
    /// Semaphore-based exclusive lock.
    /// </summary>
    internal sealed class WriterLock : LockBase, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriterLock"/> class.
        /// </summary>
        /// <param name="readAllowed">Block readers while writing; release after writing.</param>
        /// <param name="writer">Block writers while writing.</param>
        /// <param name="lockAcquired">Lock has been successfully acquired.</param>
        public WriterLock(EventWaitHandle readAllowed, Semaphore writer, bool lockAcquired = true)
            : base(lockAcquired)
        {
            ReadAllowed = readAllowed;
            Writer = writer;
        }

        private EventWaitHandle ReadAllowed { get; }

        private Semaphore Writer { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (LockAcquired)
            {
                // signal that readers may continue, and I am no longer the writer
                ReadAllowed.Set();
                Writer.Release();
            }

            GC.SuppressFinalize(this);
        }
    }
}
