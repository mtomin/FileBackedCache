namespace FileBackedCache.Tests.CacheTests
{
    using FileBackedCache.Configuration;
    using FileBackedCache.Implementation;
    using FileBackedCache.Interfaces;
    using FileBackedCache.Models;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class Get
    {
        private const string Key = "key";

        [Fact]
        public void Get_WhenEntryIsNull_ReturnsNull()
        {
            var cache = CreateCacheInstance(null);
            var result = cache.Get(Key);
            result.Should().BeNull();
        }

        [Fact]
        public void Get_WhenEntryIsExpired_ReturnsNull()
        {
            var model = CacheModels.WithExpiredDuration;
            var cache = CreateCacheInstance(model);
            var result = cache.Get(Key);
            result.Should().BeNull();
        }

        [Fact]
        public void Get_WhenEntryIsNotExpired_ReturnsEntry()
        {
            var model = CacheModels.WithValidDuration;
            var cache = CreateCacheInstance(model);
            var result = cache.Get(Key);
            result.Should().BeEquivalentTo(model.Value);
        }

        [Fact]
        public void Get_WhenEntryHasNoExpirationTime_ReturnsEntry()
        {
            var model = CacheModels.WithNoExpiry;
            var cache = CreateCacheInstance(model);
            var result = cache.Get(Key);
            result.Should().BeEquivalentTo(model.Value);
        }

        [Fact]
        public async Task GetAsync_WhenEntryIsNull_ReturnsNull()
        {
            var cache = CreateCacheInstance(null);
            var result = await cache.GetAsync(Key);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAsync_WhenEntryIsExpired_ReturnsNull()
        {
            var model = CacheModels.WithExpiredDuration;
            var cache = CreateCacheInstance(model);
            var result = await cache.GetAsync(Key);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAsync_WhenEntryIsNotExpired_ReturnsEntry()
        {
            var model = CacheModels.WithValidDuration;
            var cache = CreateCacheInstance(model);
            var result = await cache.GetAsync(Key);
            result.Should().BeEquivalentTo(model.Value);
        }

        [Fact]
        public async Task GetAsync_WhenEntryHasNoExpirationTime_ReturnsEntry()
        {
            var model = CacheModels.WithNoExpiry;
            var cache = CreateCacheInstance(model);
            var result = await cache.GetAsync(Key);
            result.Should().BeEquivalentTo(model.Value);
        }

        private static Cache CreateCacheInstance(CacheEntry? deserializedResult)
        {
            var fileProvider = new Mock<IFileProvider>();

            var configuration = new Mock<ICacheConfiguration>();
            configuration.SetupGet(c => c.RootFolder).Returns("folder");
            var serializer = new Mock<ISerializationProvider>();
            serializer.Setup(s => s.Deserialize<CacheEntry>(It.IsAny<byte[]>())).Returns(deserializedResult);
            return new Cache(fileProvider.Object, serializer.Object, configuration.Object);
        }
    }
}