using AuthService.Adapters.Database;
using AuthService.Domain;
using AuthService.Domain.Logic;
using AuthService.Domain.Ports;
using Microsoft.Extensions.Caching.Memory;

namespace AuthService.Tests.Environment
{
    public static class CommonMocks
    {
        public static IDateTime DateTimeProvider = new DateTimeProvider();
        public static IAuthDb AuthDb => new AuthDb(DateTimeProvider, MemoryCache, AuthContext, Config);
        public static AuthServiceContext AuthContext => Extensions.GetInMemoryOptions<AuthServiceContext>().GetAuthServiceContext();
        public static Config Config => new()
        {

        };

        public static readonly IMemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());
    }
}
