namespace FileBackedCache.Services
{
    using System.Text.Json;
    using FileBackedCache.Interfaces;

    /// <summary>
    /// Serializer based on System.Text.Json.
    /// </summary>
    internal class SerializationProvider : ISerializationProvider
    {
        /// <inheritdoc/>
        public byte[] Serialize<T>(T value)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value);
        }

        /// <inheritdoc/>
        public T? Deserialize<T>(byte[] data)
        {
            return JsonSerializer.Deserialize<T>(data);
        }
    }
}
