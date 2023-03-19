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

    public class Set
    {
        private const string Key = "key";

        [Fact]
        public void Set_WhenNoExpirationDefined_CreatesCacheEntryWithoutExpiration()
        {
            var serviceContext = CreateCacheContext();
            var options = CacheOptionsModels.CacheOptions_NoExpiration;
            serviceContext.Service.Set(Key, Array.Empty<byte>(), options);
            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var cacheEntry = serializer.GetSerializeArgument();
            cacheEntry.Should().NotBeNull();

            cacheEntry!.ExpirationTime.Should().BeNull();
            cacheEntry!.AbsoluteExpirationTime.Should().BeNull();
            cacheEntry!.SlidingExpiration.Should().BeNull();

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.Write(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public void Set_WhenSlidingExpirationDefined_CreatesCacheEntryWithSlidingExpiration()
        {
            var serviceContext = CreateCacheContext();
            var options = CacheOptionsModels.CacheOptions_WithSlidingExpiration;
            var expirationLowerBound = DateTime.UtcNow.Add(options.SlidingExpiration!.Value);
            serviceContext.Service.Set(Key, Array.Empty<byte>(), options);
            var expirationUpperBound = DateTime.UtcNow.Add(options.SlidingExpiration!.Value);
            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var cacheEntry = serializer.GetSerializeArgument();
            cacheEntry.Should().NotBeNull();

            cacheEntry!.ExpirationTime.Should().BeAfter(expirationLowerBound);
            cacheEntry!.ExpirationTime.Should().BeBefore(expirationUpperBound);
            cacheEntry!.AbsoluteExpirationTime.Should().BeNull();
            cacheEntry!.SlidingExpiration.Should().Be(options.SlidingExpiration);

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.Write(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public void Set_WhenAbsoluteExpirationDefined_CreatesCacheEntryWithAbsoluteExpiration()
        {
            var serviceContext = CreateCacheContext();
            var options = CacheOptionsModels.CacheOptions_WithAbsoluteExpiration;
            serviceContext.Service.Set(Key, Array.Empty<byte>(), options);
            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var cacheEntry = serializer.GetSerializeArgument();
            cacheEntry.Should().NotBeNull();

            cacheEntry!.ExpirationTime.Should().Be(options.AbsoluteExpiration!.Value.DateTime);
            cacheEntry!.AbsoluteExpirationTime.Should().Be(options.AbsoluteExpiration!.Value.DateTime);
            cacheEntry!.SlidingExpiration.Should().BeNull();

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.Write(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public void Set_WhenAbsoluteExpirationRelativeToNowDefined_CreatesCacheEntryWithAbsoluteExpiration()
        {
            var serviceContext = CreateCacheContext();
            var options = CacheOptionsModels.CacheOptions_WithAbsoluteExpirationRelativeToNow;
            var expirationLowerBound = DateTime.UtcNow.Add(options.AbsoluteExpirationRelativeToNow!.Value);
            serviceContext.Service.Set(Key, Array.Empty<byte>(), options);
            var expirationUpperBound = DateTime.UtcNow.Add(options.AbsoluteExpirationRelativeToNow!.Value);
            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var cacheEntry = serializer.GetSerializeArgument();
            cacheEntry.Should().NotBeNull();

            cacheEntry!.ExpirationTime.Should().BeAfter(expirationLowerBound);
            cacheEntry!.ExpirationTime.Should().BeBefore(expirationUpperBound);
            cacheEntry!.AbsoluteExpirationTime.Should().BeBefore(expirationUpperBound);
            cacheEntry!.AbsoluteExpirationTime.Should().BeBefore(expirationUpperBound);

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.Write(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public void Set_WhenRelativeToNowExpirationBeforeAbsolute_CreatesCacheEntryWithAbsoluteExpirationRelativeToNow()
        {
            var serviceContext = CreateCacheContext();
            var options = CacheOptionsModels.CacheOptions_WithAbsoluteExpirationRelativeToNowBeforeAbsolute;
            var expirationLowerBound = DateTime.UtcNow.Add(options.AbsoluteExpirationRelativeToNow!.Value);
            serviceContext.Service.Set(Key, Array.Empty<byte>(), options);
            var expirationUpperBound = DateTime.UtcNow.Add(options.AbsoluteExpirationRelativeToNow!.Value);
            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var cacheEntry = serializer.GetSerializeArgument();
            cacheEntry.Should().NotBeNull();

            cacheEntry!.ExpirationTime.Should().BeAfter(expirationLowerBound);
            cacheEntry!.ExpirationTime.Should().BeBefore(expirationUpperBound);
            cacheEntry!.AbsoluteExpirationTime.Should().BeBefore(expirationUpperBound);
            cacheEntry!.AbsoluteExpirationTime.Should().BeBefore(expirationUpperBound);

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.Write(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public void Set_WhenRelativeToNowExpirationAfterAbsolute_CreatesCacheEntryWithAbsoluteExpiration()
        {
            var serviceContext = CreateCacheContext();
            var options = CacheOptionsModels.CacheOptions_WithAbsoluteExpirationRelativeToNowAfterAbsolute;
            serviceContext.Service.Set(Key, Array.Empty<byte>(), options);
            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var cacheEntry = serializer.GetSerializeArgument();
            cacheEntry.Should().NotBeNull();

            cacheEntry!.ExpirationTime.Should().Be(options.AbsoluteExpiration!.Value.DateTime);
            cacheEntry!.AbsoluteExpirationTime.Should().Be(options.AbsoluteExpiration!.Value.DateTime);
            cacheEntry!.SlidingExpiration.Should().BeNull();

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.Write(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public void Set_WhenAbsoluteExpirationBeforeSliding_CreatesCacheEntryWithAbsoluteAndSlidingExpirationBasedOnAbsolute()
        {
            var serviceContext = CreateCacheContext();
            var options = CacheOptionsModels.CacheOptions_WithAbsoluteExpirationBeforeSliding;
            serviceContext.Service.Set(Key, Array.Empty<byte>(), options);
            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var cacheEntry = serializer.GetSerializeArgument();
            cacheEntry.Should().NotBeNull();

            cacheEntry!.ExpirationTime.Should().Be(options.AbsoluteExpiration!.Value.DateTime);
            cacheEntry!.AbsoluteExpirationTime.Should().Be(options.AbsoluteExpiration!.Value.DateTime);
            cacheEntry!.SlidingExpiration.Should().Be(options.SlidingExpiration);

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.Write(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public async Task SetAsync_WhenNoExpirationDefined_CreatesCacheEntryWithoutExpiration()
        {
            var serviceContext = CreateCacheContext();
            var options = CacheOptionsModels.CacheOptions_NoExpiration;
            await serviceContext.Service.SetAsync(Key, Array.Empty<byte>(), options);
            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var cacheEntry = serializer.GetSerializeArgument();
            cacheEntry.Should().NotBeNull();

            cacheEntry!.ExpirationTime.Should().BeNull();
            cacheEntry!.AbsoluteExpirationTime.Should().BeNull();
            cacheEntry!.SlidingExpiration.Should().BeNull();

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetAsync_WhenSlidingExpirationDefined_CreatesCacheEntryWithSlidingExpiration()
        {
            var serviceContext = CreateCacheContext();
            var options = CacheOptionsModels.CacheOptions_WithSlidingExpiration;
            var expirationLowerBound = DateTime.UtcNow.Add(options.SlidingExpiration!.Value);
            await serviceContext.Service.SetAsync(Key, Array.Empty<byte>(), options);
            var expirationUpperBound = DateTime.UtcNow.Add(options.SlidingExpiration!.Value);
            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var cacheEntry = serializer.GetSerializeArgument();
            cacheEntry.Should().NotBeNull();

            cacheEntry!.ExpirationTime.Should().BeAfter(expirationLowerBound);
            cacheEntry!.ExpirationTime.Should().BeBefore(expirationUpperBound);
            cacheEntry!.AbsoluteExpirationTime.Should().BeNull();
            cacheEntry!.SlidingExpiration.Should().Be(options.SlidingExpiration);

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetAsync_WhenAbsoluteExpirationDefined_CreatesCacheEntryWithAbsoluteExpiration()
        {
            var serviceContext = CreateCacheContext();
            var options = CacheOptionsModels.CacheOptions_WithAbsoluteExpiration;
            await serviceContext.Service.SetAsync(Key, Array.Empty<byte>(), options);
            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var cacheEntry = serializer.GetSerializeArgument();
            cacheEntry.Should().NotBeNull();

            cacheEntry!.ExpirationTime.Should().Be(options.AbsoluteExpiration!.Value.DateTime);
            cacheEntry!.AbsoluteExpirationTime.Should().Be(options.AbsoluteExpiration!.Value.DateTime);
            cacheEntry!.SlidingExpiration.Should().BeNull();

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetAsync_WhenAbsoluteExpirationRelativeToNowDefined_CreatesCacheEntryWithAbsoluteExpiration()
        {
            var serviceContext = CreateCacheContext();
            var options = CacheOptionsModels.CacheOptions_WithAbsoluteExpirationRelativeToNow;
            var expirationLowerBound = DateTime.UtcNow.Add(options.AbsoluteExpirationRelativeToNow!.Value);
            await serviceContext.Service.SetAsync(Key, Array.Empty<byte>(), options);
            var expirationUpperBound = DateTime.UtcNow.Add(options.AbsoluteExpirationRelativeToNow!.Value);
            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var cacheEntry = serializer.GetSerializeArgument();
            cacheEntry.Should().NotBeNull();

            cacheEntry!.ExpirationTime.Should().BeAfter(expirationLowerBound);
            cacheEntry!.ExpirationTime.Should().BeBefore(expirationUpperBound);
            cacheEntry!.AbsoluteExpirationTime.Should().BeBefore(expirationUpperBound);
            cacheEntry!.AbsoluteExpirationTime.Should().BeBefore(expirationUpperBound);

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetAsync_WhenRelativeToNowExpirationBeforeAbsolute_CreatesCacheEntryWithAbsoluteExpirationRelativeToNow()
        {
            var serviceContext = CreateCacheContext();
            var options = CacheOptionsModels.CacheOptions_WithAbsoluteExpirationRelativeToNowBeforeAbsolute;
            var expirationLowerBound = DateTime.UtcNow.Add(options.AbsoluteExpirationRelativeToNow!.Value);
            await serviceContext.Service.SetAsync(Key, Array.Empty<byte>(), options);
            var expirationUpperBound = DateTime.UtcNow.Add(options.AbsoluteExpirationRelativeToNow!.Value);
            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var cacheEntry = serializer.GetSerializeArgument();
            cacheEntry.Should().NotBeNull();

            cacheEntry!.ExpirationTime.Should().BeAfter(expirationLowerBound);
            cacheEntry!.ExpirationTime.Should().BeBefore(expirationUpperBound);
            cacheEntry!.AbsoluteExpirationTime.Should().BeBefore(expirationUpperBound);
            cacheEntry!.AbsoluteExpirationTime.Should().BeBefore(expirationUpperBound);

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetAsync_WhenRelativeToNowExpirationAfterAbsolute_CreatesCacheEntryWithAbsoluteExpiration()
        {
            var serviceContext = CreateCacheContext();
            var options = CacheOptionsModels.CacheOptions_WithAbsoluteExpirationRelativeToNowAfterAbsolute;
            await serviceContext.Service.SetAsync(Key, Array.Empty<byte>(), options);
            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var cacheEntry = serializer.GetSerializeArgument();
            cacheEntry.Should().NotBeNull();

            cacheEntry!.ExpirationTime.Should().Be(options.AbsoluteExpiration!.Value.DateTime);
            cacheEntry!.AbsoluteExpirationTime.Should().Be(options.AbsoluteExpiration!.Value.DateTime);
            cacheEntry!.SlidingExpiration.Should().BeNull();

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetAsync_WhenAbsoluteExpirationBeforeSliding_CreatesCacheEntryWithAbsoluteAndSlidingExpirationBasedOnAbsolute()
        {
            var serviceContext = CreateCacheContext();
            var options = CacheOptionsModels.CacheOptions_WithAbsoluteExpirationBeforeSliding;
            await serviceContext.Service.SetAsync(Key, Array.Empty<byte>(), options);
            var serializer = serviceContext.Context.Mock<ISerializationProvider>();

            serializer.Verify(m => m.Serialize(It.IsAny<CacheEntry>()), Times.Once);
            var cacheEntry = serializer.GetSerializeArgument();
            cacheEntry.Should().NotBeNull();

            cacheEntry!.ExpirationTime.Should().Be(options.AbsoluteExpiration!.Value.DateTime);
            cacheEntry!.AbsoluteExpirationTime.Should().Be(options.AbsoluteExpiration!.Value.DateTime);
            cacheEntry!.SlidingExpiration.Should().Be(options.SlidingExpiration);

            serviceContext.Context.Mock<IFileProvider>()
                .Verify(m => m.WriteAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        private static ServiceContext<Cache> CreateCacheContext()
        {
            var fileProvider = new Mock<IFileProvider>();

            var configuration = new Mock<ICacheConfiguration>();
            configuration.SetupGet(c => c.RootFolder).Returns("folder");
            var serializer = new Mock<ISerializationProvider>();
            var service = new Cache(fileProvider.Object, serializer.Object, configuration.Object);

            var context = new Context()
                .With(fileProvider)
                .With(serializer);

            return new ServiceContext<Cache>(service, context);
        }
    }
}
