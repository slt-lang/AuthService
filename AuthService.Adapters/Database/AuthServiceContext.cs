using AuthService.Adapters.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Adapters.Database
{
    public class AuthServiceContext(DbContextOptions options) : DbContext(options)
    {
        public const string AuthScheme = "AuthService";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        private string DateTimeNowSqlFunction
        {
            get
            {
                if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
                    return "NOW()";
                return "GETDATE()";
            }
        }
    }
}
