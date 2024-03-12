using Microsoft.Extensions.Caching.Memory;
using System;

namespace WMSWebApp.Wrapper
{
    public class MemoryCacheWrapper : IMemoryCacheWrapper
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheWrapper(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Set<T>(string key, T cache, double expireMinutes)
        {
            _memoryCache.Set(key, cache, TimeSpan.FromMinutes(expireMinutes));
        }

        public void Set<T>(string key, T cache)
        {
            _memoryCache.Set(key, cache);
        }

        public bool TryGetValue<T>(string key, out T cache)
        {
            if(_memoryCache.TryGetValue(key, out T cachedItem))
            {
                cache = cachedItem;
                return true;
            }
            cache = default(T);
            return false;
        }
    }
}
