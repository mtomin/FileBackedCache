namespace FileBackedCache.Tests.LockTests
{
    using FileBackedCache.Models;
    using FileBackedCache.Tests.Helpers;
    using FluentAssertions;
    using Xunit;

    public class WriterLockTest
    {
        [Fact]
        public void WriterLock_WhenDisposed_SignalsReadAllowed()
        {
            var testContext = CreateLockContext(lockAcquired: true);
            testContext.Service.Dispose();

            testContext.Context.Get<EventWaitHandle>().WaitOne(0).Should().BeTrue();
        }

        [Fact]
        public void WriterLock_WhenDisposed_ReleasesSemaphore()
        {
            var testContext = CreateLockContext(lockAcquired: true);
            testContext.Service.Dispose();

            testContext.Context.Get<Semaphore>().WaitOne(0).Should().BeTrue();
        }

        [Fact]
        public void WriterLock_WhenDisposedAndLockNotAcquired_DoesNotSignalReadFinished()
        {
            var testContext = CreateLockContext(lockAcquired: false);
            testContext.Service.Dispose();

            testContext.Context.Get<Semaphore>().WaitOne(0).Should().BeFalse();
        }

        [Fact]
        public void WriterLock_WhenDisposedAndLockNotAcquired_DoesNotReleaseSemaphore()
        {
            var testContext = CreateLockContext(lockAcquired: false);
            testContext.Service.Dispose();

            testContext.Context.Get<EventWaitHandle>().WaitOne(0).Should().BeFalse();
        }

        private static ServiceContext<WriterLock> CreateLockContext(bool lockAcquired)
        {
            var readAllowed = new EventWaitHandle(false, EventResetMode.ManualReset);
            var semaphore = new Semaphore(0, 1);
            var @lock = new WriterLock(readAllowed, semaphore, lockAcquired);

            var context = new Context()
                .With(readAllowed)
                .With(semaphore);

            return new ServiceContext<WriterLock>(@lock, context);
        }
    }
}
