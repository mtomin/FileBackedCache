namespace FileBackedCache.Tests.Helpers
{
    using System.Linq;
    using FileBackedCache.Interfaces;
    using FileBackedCache.Models;
    using Moq;

    internal static class MockExtensions
    {
        public static CacheEntry? GetSerializeArgument(this Mock<ISerializationProvider> mock)
        {
            return mock.Invocations
                .FirstOrDefault(i => i?.Method?.Name == nameof(ISerializationProvider.Serialize))
                ?.Arguments[0] as CacheEntry;
        }
    }
}
