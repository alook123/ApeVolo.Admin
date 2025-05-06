using Ape.Volo.Common.Extensions;
using Ape.Volo.Core;
using IP2Region.Net.Abstractions;
using IP2Region.Net.XDB;
using Microsoft.Extensions.DependencyInjection;

namespace Ape.Volo.Infrastructure.Extensions
{
    public static class IpSearcherSetup
    {
        public static void AddIpSearcherSetup(this IServiceCollection services)
        {
            if (services.IsNull()) throw new ArgumentNullException(nameof(services));
            services.AddSingleton<ISearcher>(new Searcher(CachePolicy.Content,
                Path.Combine(App.WebHostEnvironment.WebRootPath, "resources", "ip", "ip2region.xdb")));
        }
    }
}
