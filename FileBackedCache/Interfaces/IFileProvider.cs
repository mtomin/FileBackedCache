namespace FileBackedCache.Interfaces
{
    /// <summary>
    /// Provides interface for file I/O.
    /// </summary>
    public interface IFileProvider
    {
        /// <summary>
        /// Synchronously reads contents of a file as a byte array.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>File content.</returns>
        byte[]? Read(string path);

        /// <summary>
        /// Asynchronously reads contents of a file as a byte array.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>File content.</returns>
        /// /// <param name="token">Cancellation token.</param>
        Task<byte[]?> ReadAsync(string path, CancellationToken token);

        /// <summary>
        /// Synchronously writes file to disk.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="data">File content.</param>
        void Write(string path, byte[] data);

        /// <summary>
        /// Asynchronously writes file to disk.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="data">File content.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task WriteAsync(string path, byte[] data, CancellationToken token);

        /// <summary>
        /// Deletes file on a given path.
        /// </summary>
        /// <param name="path">File path.</param>
        void Delete(string path);
    }
}
