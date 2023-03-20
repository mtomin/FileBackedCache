namespace FileBackedCache.Tests.LockTests
{
    using FileBackedCache.Models;
    using FileBackedCache.Tests.Helpers;
    using FluentAssertions;
    using Xunit;

    public class ReaderLockTest
    {
        [Fact]
        public void ReaderLock_WhenDisposed_SignalsReadFinished()
        {
            var testContext = CreateLockContext(lockAcquired: true);
            testContext.Service.Dispose();

            testContext.Context.Get<EventWaitHandle>().WaitOne(0).Should().BeTrue();
        }

        [Fact]
        public void ReaderLock_WhenDisposed_ReleasesSemaphore()
        {
            var testContext = CreateLockContext(lockAcquired: true);
            testContext.Service.Dispose();

            testContext.Context.Get<Semaphore>().WaitOne(0).Should().BeTrue();
        }

        [Fact]
        public void ReaderLock_WhenDisposedAndLockNotAcquired_DoesNotSignalReadFinished()
        {
            var testContext = CreateLockContext(lockAcquired: false);
            testContext.Service.Dispose();

            testContext.Context.Get<EventWaitHandle>().WaitOne(0).Should().BeFalse();
        }

        [Fact]
        public void ReaderLock_WhenDisposedAndLockNotAcquired_DoesNotReleaseSemaphore()
        {
            var testContext = CreateLockContext(lockAcquired: false);
            testContext.Service.Dispose();

            testContext.Context.Get<Semaphore>().WaitOne(0).Should().BeFalse();
        }

        private static ServiceContext<ReaderLock> CreateLockContext(bool lockAcquired)
        {
            var readFinished = new EventWaitHandle(false, EventResetMode.ManualReset);
            var semaphore = new Semaphore(0, 1);
            var @lock = new ReaderLock(semaphore, readFinished, lockAcquired);

            var context = new Context()
                .With(readFinished)
                .With(semaphore);

            return new ServiceContext<ReaderLock>(@lock, context);
        }
    }
}
