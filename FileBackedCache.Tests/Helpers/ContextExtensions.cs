namespace FileBackedCache.Tests.Helpers
{
    using Moq;

    internal static class ContextExtensions
    {
        public static Context With<T>(this Context context, T instance)
            where T : class
        {
            context.Members[typeof(T)] = instance;
            return context;
        }

        public static T Get<T>(this Context context)
            where T : class
        {
            return context.Members.GetValueOrDefault(typeof(T)) as T;
        }

        public static Mock<T> Mock<T>(this Context context)
            where T : class
        {
            return context.Members.GetValueOrDefault(typeof(Mock<T>)) as Mock<T>;
        }
    }
}
