using Ape.Volo.Common.Extensions;
using Ape.Volo.Core;
using Ape.Volo.Core.Caches;
using Ape.Volo.Core.Caches.Distributed;
using Ape.Volo.Core.Caches.Redis;
using Ape.Volo.Core.ConfigOptions;
using Microsoft.Extensions.DependencyInjection;

namespace Ape.Volo.Infrastructure.Extensions;

/// <summary>
/// 缓存启动器
/// </summary>
public static class CacheSetup
{
    public static void AddCacheSetup(this IServiceCollection services)
    {
        if (services.IsNull())
            throw new ArgumentNullException(nameof(services));
        services.AddDistributedMemoryCache(); //session需要

        if (App.GetOptions<SystemOptions>().UseRedisCache)
        {
            services.AddSingleton<ICache, RedisCache>();
            return;
        }

        services.AddSingleton<ICache, DistributedCache>();
    }
}
