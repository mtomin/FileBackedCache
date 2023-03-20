namespace FileBackedCache.Tests.LockProviderTests
{
    using FileBackedCache.Configuration;
    using FileBackedCache.Interfaces;
    using FileBackedCache.Services;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class AcquireReadLock
    {
        [Fact]
        public void AcquireReadLock_WhenTimedOut_ReturnsLockAcquiredFalse()
        {
            var service = CreateLockProviderInstance(lockTimeoutSeconds: 0);
            var readLock = service.AcquireReadLock(nameof(AcquireReadLock_WhenLockAcquired_ReturnsLock));
            readLock.LockAcquired.Should().BeFalse();
        }

        [Fact]
        public void AcquireReadLock_WhenLockAcquired_ReturnsLock()
        {
            var service = CreateLockProviderInstance();
            var readLock = service.AcquireReadLock(nameof(AcquireReadLock_WhenLockAcquired_ReturnsLock));
            readLock.LockAcquired.Should().BeTrue();
        }

        [Fact]
        public void AcquireReadLock_WhenReadNotAllowed_ReturnsLockAcquiredFalse()
        {
            const string uniqueName = nameof(AcquireReadLock_WhenReadNotAllowed_ReturnsLockAcquiredFalse);
            var handleName = $"Global\\{nameof(FileBackedCache)}-readAllowed:{uniqueName}".Replace(Path.DirectorySeparatorChar, '_');
            EventWaitHandle readAllowed = new (true, EventResetMode.ManualReset, handleName);
            readAllowed.Reset();
            var service = CreateLockProviderInstance(1);
            var readLock = service.AcquireReadLock(uniqueName);
            readLock.LockAcquired.Should().BeFalse();
        }

        [Fact]
        public void AcquireReadLock_WhenMaxReadersReached_ReturnsLockAcquiredFalse()
        {
            const string uniqueName = nameof(AcquireReadLock_WhenMaxReadersReached_ReturnsLockAcquiredFalse);
            var semaphoreName = $"Global\\{nameof(FileBackedCache)}_readers:{uniqueName.Replace(Path.DirectorySeparatorChar, '_')}";
            var readers = new Semaphore(0, 1, semaphoreName);
            var service = CreateLockProviderInstance(3);
            var readLock = service.AcquireReadLock(uniqueName);
            readLock.LockAcquired.Should().BeFalse();
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
