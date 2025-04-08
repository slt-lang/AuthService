using AuthService.Adapters.Database;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Tests.Environment
{
    internal static class Extensions
    {
        public static AuthServiceContext GetAuthServiceContext(this DbContextOptions<AuthServiceContext> options)
        {
            var context = new AuthServiceContext(options);
            context.Database.EnsureCreated();
            context.Database.EnsureDeleted();
            return context;
        }

        public static DbContextOptions<T> GetInMemoryOptions<T>() where T : DbContext
        {
            return new DbContextOptionsBuilder<T>().UseInMemoryDatabase(databaseName: "Fake" + typeof(T).Name).Options;
        }
    }
}
