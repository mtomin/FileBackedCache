namespace FileBackedCache.Services
{
    using FileBackedCache.Interfaces;

    /// <summary>
    /// Provides file access.
    /// </summary>
    internal sealed class FileProvider : IFileProvider
    {
        /// <inheritdoc/>
        public byte[]? Read(string path)
        {
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }

        /// <inheritdoc/>
        public async Task<byte[]?> ReadAsync(string path, CancellationToken token)
        {
            return File.Exists(path) ? await File.ReadAllBytesAsync(path, token) : null;
        }

        /// <inheritdoc/>
        public void Write(string path, byte[] data)
        {
            File.WriteAllBytes(path, data);
        }

        /// <inheritdoc/>
        public async Task WriteAsync(string path, byte[] data, CancellationToken token)
        {
            await File.WriteAllBytesAsync(path, data, token);
        }

        /// <inheritdoc/>
        public void Delete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
