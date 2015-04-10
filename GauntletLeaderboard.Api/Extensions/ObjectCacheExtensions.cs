using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace GauntletLeaderboard.Api.Extensions
{
    public static class ObjectCacheExtensions
    {
        public static T GetOrAdd<T>(this ObjectCache cache, string key, Func<T> valueFactory, CacheItemPolicy cacheItemPolicy = null) where T : class
        {
            var policy = cacheItemPolicy ?? new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(5) };
            var newLazy = new Lazy<T>(valueFactory);
            var lazyFromCache = (Lazy<T>)cache.AddOrGetExisting(key, newLazy, policy);

            return (lazyFromCache ?? newLazy).Value;
        }
    }
}