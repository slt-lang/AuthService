using AuthService.Adapters.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Adapters.Database
{
    public class AuthServiceContext(DbContextOptions options) : DbContext(options)
    {
        public const string AuthScheme = "AuthService";

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Invite> Invites { get; set; } = default!;
        public DbSet<UserPermission> UserPermissions { get; set; } = default!;
        public DbSet<UserVariable> UserVariables { get; set; } = default!;
        public DbSet<InvitePermission> InvitePermissions { get; set; } = default!;
        public DbSet<InviteVariable> InviteVariables { get; set; } = default!;
        public DbSet<RefPermission> RefPermissions { get; set; } = default!;
        public DbSet<RefVariable> RefVariables { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable(nameof(Users), AuthScheme);

                e.HasKey(e => e.Id);
                e.Property(e => e.Username).IsRequired().IsUnicode(false).HasMaxLength(64);
                e.Property(e => e.PasswordHash).IsRequired(false).IsUnicode(false).HasMaxLength(256);
                e.Property(e => e.RegisterDate).IsRequired().HasDefaultValueSql(DateTimeNowSqlFunction);
                //e.Property(e => e.InvitedBy).IsRequired(false);

                //e.HasOne(e => e.InvitedBy).WithOne();
                e.HasMany(e => e.Variables).WithOne(e => e.User);
                e.HasMany(e => e.Permissions).WithOne(e => e.User);
            });

            modelBuilder.Entity<Invite>(e =>
            {
                e.ToTable(nameof(Invites), AuthScheme);

                e.HasKey(e => e.Id);

                e.HasMany(e => e.Variables).WithOne(e => e.Invite);
                e.HasMany(e => e.Permissions).WithOne(e => e.Invite);
            });
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
