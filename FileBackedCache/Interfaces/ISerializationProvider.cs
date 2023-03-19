namespace FileBackedCache.Interfaces
{
    /// <summary>
    /// Provides a serializer to convert objects to byte array and vice versa.
    /// </summary>
    public interface ISerializationProvider
    {
        /// <summary>
        /// Serializes object to a byte array.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="value">Object to be serialized.</param>
        /// <returns>Byte array representing the object.</returns>
        byte[] Serialize<T>(T value);

        /// <summary>
        /// Deserialize byte array to object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="data">Data to deserialize.</param>
        /// <returns>Deserialized object.</returns>
        T? Deserialize<T>(byte[] data);
    }
}
