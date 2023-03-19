namespace FileBackedCache.Tests.CacheTests
{
    using FileBackedCache.Configuration;
    using FileBackedCache.Implementation;
    using FileBackedCache.Interfaces;
    using FileBackedCache.Models;
    using FileBackedCache.Tests.Helpers;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class Refresh
    {
        private const string Key = "key";

        [Fact]
        public void Refresh_WhenEntryIsNull_DoesNotUpdateEntry()
        {
            var serviceContext = CreateCacheContext(null);
            serviceContext.Service.Refresh(Key);

            serviceContext.Context.Mock<ISerializationProvider>()
                .Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Never);
            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.Write(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
        }

        [Fact]
        public void Refresh_WhenEntryHasExpired_DoesNotUpdateEntry()
        {
            var model = CacheModels.WithExpiredDuration;
            var serviceContext = CreateCacheContext(model);
            serviceContext.Service.Refresh(Key);

            serviceContext.Context.Mock<ISerializationProvider>()
                .Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Never);
            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.Write(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
        }

        [Fact]
        public void Refresh_WhenEntryHasNotExpired_UpdatesEntryWithNewSlidingExpiration()
        {
            var model = CacheModels.WithValidDuration;
            var serviceContext = CreateCacheContext(model);
            var expectedNewExpiration = DateTime.UtcNow.Add(model.SlidingExpiration!.Value);
            serviceContext.Service.Refresh(Key);

            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var refreshedEntry = serializer.GetSerializeArgument();
            refreshedEntry.Should().NotBeNull();
            refreshedEntry!.Value.Should().BeEquivalentTo(model.Value);
            refreshedEntry.ExpirationTime.Should().BeAfter(expectedNewExpiration);

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.Write(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public void Refresh_WhenEntryIsCloseToAbsoluteExpiration_UpdatesEntryWithSlidingExpirationEqualToAbsoluteExpiration()
        {
            var model = CacheModels.WithValidDuration_NearAbsoluteDuration;
            var serviceContext = CreateCacheContext(model);
            serviceContext.Service.Refresh(Key);

            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var refreshedEntry = serializer.GetSerializeArgument();
            refreshedEntry.Should().NotBeNull();
            refreshedEntry!.Value.Should().BeEquivalentTo(model.Value);
            refreshedEntry.ExpirationTime.Should().Be(model.AbsoluteExpirationTime);

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.Write(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public async Task RefreshAsync_WhenEntryIsNull_DoesNotUpdateEntry()
        {
            var serviceContext = CreateCacheContext(null);
            await serviceContext.Service.RefreshAsync(Key);

            serviceContext.Context.Mock<ISerializationProvider>()
                .Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Never);
            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RefreshAsync_WhenEntryHasExpired_DoesNotUpdateEntry()
        {
            var model = CacheModels.WithExpiredDuration;
            var serviceContext = CreateCacheContext(model);
            await serviceContext.Service.RefreshAsync(Key);

            serviceContext.Context.Mock<ISerializationProvider>()
                .Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Never);
            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RefreshAsync_WhenEntryHasNotExpired_UpdatesEntryWithNewSlidingExpiration()
        {
            var model = CacheModels.WithValidDuration;
            var serviceContext = CreateCacheContext(model);
            var expectedNewExpirationLowerBound = DateTime.UtcNow.Add(model.SlidingExpiration!.Value);
            await serviceContext.Service.RefreshAsync(Key);
            var expectedNewExpirationUpperBound = DateTime.UtcNow.Add(model.SlidingExpiration!.Value);

            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var refreshedEntry = serializer.GetSerializeArgument();
            refreshedEntry.Should().NotBeNull();
            refreshedEntry!.Value.Should().BeEquivalentTo(model.Value);
            refreshedEntry.ExpirationTime.Should().BeAfter(expectedNewExpirationLowerBound);
            refreshedEntry.ExpirationTime.Should().BeBefore(expectedNewExpirationUpperBound);

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RefreshAsync_WhenEntryIsCloseToAbsoluteExpiration_UpdatesEntryWithSlidingExpirationEqualToAbsoluteExpiration()
        {
            var model = CacheModels.WithValidDuration_NearAbsoluteDuration;
            var serviceContext = CreateCacheContext(model);
            await serviceContext.Service.RefreshAsync(Key);

            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var refreshedEntry = serializer.GetSerializeArgument();
            refreshedEntry.Should().NotBeNull();
            refreshedEntry!.Value.Should().BeEquivalentTo(model.Value);
            refreshedEntry.ExpirationTime.Should().Be(model.AbsoluteExpirationTime);

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        private static ServiceContext<Cache> CreateCacheContext(CacheEntry? deserializedResult)
        {
            var fileProvider = new Mock<IFileProvider>();

            var configuration = new Mock<ICacheConfiguration>();
            configuration.SetupGet(c => c.RootFolder).Returns("folder");
            var serializer = new Mock<ISerializationProvider>();
            serializer.Setup(s => s.Deserialize<CacheEntry>(It.IsAny<byte[]>())).Returns(deserializedResult);
            var service = new Cache(fileProvider.Object, serializer.Object, configuration.Object);

            var context = new Context()
                .With(fileProvider)
                .With(serializer);

            return new ServiceContext<Cache>(service, context);
        }
    }
}
