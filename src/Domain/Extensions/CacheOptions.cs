using Microsoft.Extensions.Caching.Distributed;

namespace Microsoft.Extensions.Caching.Distributed;

public static class CacheOptions
{
    public static DistributedCacheEntryOptions DefaultExpiration =>
        new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20) };
}