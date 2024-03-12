namespace WMSWebApp.Wrapper
{
    public interface IMemoryCacheWrapper
    {
        bool TryGetValue<T> (string key, out T value);
        void Set<T>(string key, T cache, double expireMinutes);
        void Set<T>(string key, T cache);
    }
}
