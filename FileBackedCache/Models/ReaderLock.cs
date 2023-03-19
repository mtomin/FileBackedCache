namespace FileBackedCache.Models
{
    internal sealed class ReaderLock : LockBase, IDisposable
    {
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
            Readers.Release();
            ReadFinished.Set();
            GC.SuppressFinalize(this);
        }
    }
}
