namespace FileBackedCache.Tests.Helpers
{
    internal class Context
    {
        public Context()
        {
            Members = new ();
        }

        public Dictionary<Type, object> Members { get; }
    }
}
