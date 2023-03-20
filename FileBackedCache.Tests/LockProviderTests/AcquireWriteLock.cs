namespace FileBackedCache.Tests.LockProviderTests
{
    using FileBackedCache.Configuration;
    using FileBackedCache.Interfaces;
    using FileBackedCache.Services;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class AcquireWriteLock
    {
        [Fact]
        public void AcquireWriteLock_WhenTimedOut_ReturnsLockAcquiredFalse()
        {
            var service = CreateLockProviderInstance(lockTimeoutSeconds: 0);
            var writeLock = service.AcquireWriteLock(nameof(AcquireWriteLock_WhenTimedOut_ReturnsLockAcquiredFalse));
            writeLock.LockAcquired.Should().BeFalse();
        }

        [Fact]
        public void AcquireWriteLock_WhenLockAcquired_ReturnsLock()
        {
            var service = CreateLockProviderInstance();
            var writeLock = service.AcquireWriteLock(nameof(AcquireWriteLock_WhenLockAcquired_ReturnsLock));
            writeLock.LockAcquired.Should().BeTrue();
        }

        [Fact]
        public void AcquireWriteLock_WhenAnotherWriterExists_ReturnsLockAcquiredFalse()
        {
            const string uniqueName = nameof(AcquireWriteLock_WhenAnotherWriterExists_ReturnsLockAcquiredFalse);
            var handleName = $"{nameof(FileBackedCache)}-readAllowed:{uniqueName}".Replace(Path.DirectorySeparatorChar, '_');
            var service = CreateLockProviderInstance(1);
            var semaphoreName = $"{nameof(FileBackedCache)}_writers:{uniqueName.Replace(Path.DirectorySeparatorChar, '_')}";
            Semaphore readers = new (0, 1, semaphoreName);
            var writeLock = service.AcquireWriteLock(uniqueName);
            writeLock.LockAcquired.Should().BeFalse();
        }

        [Fact]
        public void AcquireWriteLock_WhenReadersActive_ReturnsLockAcquiredFalse()
        {
            const string uniqueName = nameof(AcquireWriteLock_WhenAnotherWriterExists_ReturnsLockAcquiredFalse);
            var handleName = $"{nameof(FileBackedCache)}-readAllowed:{uniqueName}".Replace(Path.DirectorySeparatorChar, '_');
            var service = CreateLockProviderInstance(1);
            var semaphoreName = $"{nameof(FileBackedCache)}_readers:{uniqueName.Replace(Path.DirectorySeparatorChar, '_')}";
            Semaphore readers = new (0, 1, semaphoreName);
            var writeLock = service.AcquireWriteLock(uniqueName);
            writeLock.LockAcquired.Should().BeFalse();
        }

        private static LockProvider CreateLockProviderInstance(int lockTimeoutSeconds = 20)
        {
            var fileProvider = new Mock<IFileProvider>();

            var configuration = new Mock<ILockConfiguration>();
            configuration.SetupGet(c => c.LockTimeout).Returns(TimeSpan.FromSeconds(lockTimeoutSeconds));
            return new LockProvider(configuration.Object);
        }
    }
}
