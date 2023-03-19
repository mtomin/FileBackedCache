namespace FileBackedCache.Tests.CacheTests
{
    using FileBackedCache.Configuration;
    using FileBackedCache.Implementation;
    using FileBackedCache.Interfaces;
    using FileBackedCache.Tests.Helpers;
    using Moq;
    using Xunit;

    public class Remove
    {
        private const string Key = "key";

        [Fact]
        public void Remove_CallsFileProviderDelete()
        {
            var serviceContext = CreateCacheContext();
            serviceContext.Service.Remove(Key);
            serviceContext.Context.Mock<IFileProvider>().Verify(fp => fp.Delete(It.IsAny<string>()), Times.Once);
        }

        private static ServiceContext<Cache> CreateCacheContext()
        {
            var fileProvider = new Mock<IFileProvider>();

            var configuration = new Mock<ICacheConfiguration>();
            configuration.SetupGet(c => c.RootFolder).Returns("folder");
            var service = new Cache(fileProvider.Object, null!, configuration.Object);

            var context = new Context()
                .With(fileProvider);

            return new ServiceContext<Cache>(service, context);
        }
    }
}
