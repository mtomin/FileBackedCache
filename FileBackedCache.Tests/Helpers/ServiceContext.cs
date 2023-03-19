namespace FileBackedCache.Tests.Helpers
{
    internal class ServiceContext<T>
        where T : class
    {
        public ServiceContext(T service, Context context)
        {
            Service = service;
            Context = context;
        }

        public T Service { get; }

        public Context Context { get; }
    }
}
