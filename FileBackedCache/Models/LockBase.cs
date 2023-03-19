namespace FileBackedCache.Models
{
    internal abstract class LockBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LockBase"/> class.
        /// </summary>
        /// <param name="successfullyAcquired">Was lock acquired.</param>
        protected LockBase(bool successfullyAcquired)
        {
            LockAcquired = successfullyAcquired;
        }

        /// <summary>
        /// Indicates if lock was successfully acquired.
        /// </summary>
        public bool LockAcquired { get; }
    }
}
