using AuthService.Adapters.Database.Models;
using Microsoft.EntityFrameworkCore;
using sltlang.Common.AuthService.Enums;
using sltlang.Common.Common;
using sltlang.Common.Common.Extensions;
using System.ComponentModel;

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
            modelBuilder.Entity<RefVariable>(e =>
            {
                e.ToTable(nameof(RefVariables), AuthScheme);

                e.HasKey(e => e.Id);

                e.Property(e => e.Id).HasConversion<int>().IsRequired().ValueGeneratedNever();
                e.Property(e => e.Name).IsRequired(true);
                e.Property(e => e.Description).IsRequired(false);

                e.HasData(EnumExtensions.EnumHelper<Variable>.Values.Select(x => new RefVariable()
                {
                    Id = x,
                    Name = x.ToString(),
                    Description = EnumExtensions.EnumHelper<Variable>.AttributeHelper<DescriptionAttribute>.With(x, desc => desc?.Description)!,
                    Type = EnumExtensions.EnumHelper<Variable>.AttributeHelper<ComponentTypeAttribute>.With(x, type => type?.Type.Name)!
                }));
            });

            modelBuilder.Entity<RefPermission>(e =>
            {
                e.ToTable(nameof(RefPermissions), AuthScheme);

                e.HasKey(e => e.Id);

                e.Property(e => e.Id).HasConversion<int>().IsRequired().ValueGeneratedNever();
                e.Property(e => e.Name).IsRequired(true);
                e.Property(e => e.Description).IsRequired(false);

                e.HasData(EnumExtensions.EnumHelper<Permission>.Values.Select(x => new RefPermission()
                {
                    Id = x,
                    Name = x.ToString(),
                    Description = EnumExtensions.EnumHelper<Permission>.AttributeHelper<DescriptionAttribute>.With(x, desc => desc?.Description)!,
                }));
            });

            modelBuilder.Entity<UserPermission>(e =>
            {
                e.ToTable(nameof(UserPermissions), AuthScheme);

                e.HasKey(e => e.Id);
                e.Property(e => e.PermissionId).HasConversion<int>();
                e.Property(e => e.CreateDate).IsRequired(true).HasDefaultValueSql(DateTimeNowSqlFunction);
                e.Property(e => e.EndDate).IsRequired(false);
                e.Property(e => e.AllowInheritance).IsRequired(true);
                e.Property(e => e.UserId).IsRequired(true);

                e.HasOne(e => e.RefPermission).WithMany().HasForeignKey(e => e.PermissionId);
                e.HasOne(e => e.User).WithMany(e => e.Permissions).HasForeignKey(e => e.UserId);

                e.HasIndex(e => e.UserId);
            });

            modelBuilder.Entity<InvitePermission>(e =>
            {
                e.ToTable(nameof(InvitePermissions), AuthScheme);

                e.HasKey(e => e.Id);
                e.Property(e => e.PermissionId).HasConversion<int>();
                e.Property(e => e.CreateDate).IsRequired(true).HasDefaultValueSql(DateTimeNowSqlFunction);
                e.Property(e => e.EndDate).IsRequired(false);
                e.Property(e => e.AllowInheritance).IsRequired(true);
                e.Property(e => e.InviteId).IsRequired(true);

                e.HasOne(e => e.RefPermission).WithMany().HasForeignKey(e => e.PermissionId);
                e.HasOne(e => e.Invite).WithMany(e => e.Permissions).HasForeignKey(e => e.InviteId);

                e.HasIndex(e => e.InviteId);
            });

            modelBuilder.Entity<UserVariable>(e =>
            {
                e.ToTable(nameof(UserVariables), AuthScheme);

                e.HasKey(e => e.Id);
                e.Property(e => e.Name).HasConversion<int>();
                e.Property(e => e.Value).IsRequired(true);
                e.Property(e => e.UserId).IsRequired(true);

                e.HasOne(e => e.RefVariable).WithMany().HasForeignKey(e => e.Name);
                e.HasOne(e => e.User).WithMany(e => e.Variables).HasForeignKey(e => e.UserId);

                e.HasIndex(e => e.UserId);
            });

            modelBuilder.Entity<InviteVariable>(e =>
            {
                e.ToTable(nameof(InviteVariables), AuthScheme);

                e.HasKey(e => e.Id);
                e.Property(e => e.Name).HasConversion<int>();
                e.Property(e => e.Value).IsRequired(true);
                e.Property(e => e.InviteId).IsRequired(true);

                e.HasOne(e => e.RefVariable).WithMany().HasForeignKey(e => e.Name);
                e.HasOne(e => e.Invite).WithMany(e => e.Variables).HasForeignKey(e => e.InviteId);

                e.HasIndex(e => e.InviteId);
            });

            modelBuilder.Entity<User>(e =>
            {
                e.ToTable(nameof(Users), AuthScheme);

                e.HasKey(e => e.Id);
                e.Property(e => e.Username).IsRequired().IsUnicode(false).HasMaxLength(64);
                e.Property(e => e.PasswordHash).IsRequired(false).IsUnicode(false).HasMaxLength(256);
                e.Property(e => e.RegisterDate).IsRequired().HasDefaultValueSql(DateTimeNowSqlFunction);
                e.Property(e => e.InvitedById).IsRequired(false);
                e.Property(e => e.Enabled).IsRequired(true);
                e.Property(e => e.IsTemplate).IsRequired(true);

                e.HasMany(e => e.InvitedUsers).WithOne(e => e.InvitedBy).HasForeignKey(e => e.InvitedById);

                e.HasIndex(e => e.Username);
            });

            modelBuilder.Entity<Invite>(e =>
            {
                e.ToTable(nameof(Invites), AuthScheme);

                e.HasKey(e => e.Id);

                e.Property(e => e.Link).IsRequired(true).HasMaxLength(200).IsUnicode(false);
                e.Property(e => e.Ttl).IsRequired(true);
                e.Property(e => e.UserId).IsRequired(true);
                e.Property(e => e.InheritanceUserId).IsRequired(false);

                e.HasOne(e => e.User).WithMany(e => e.Invites).HasForeignKey(e => e.UserId);
                e.HasOne(e => e.InheritanceUser).WithMany(e => e.InheritedInvites).HasForeignKey(e => e.InheritanceUserId);

                e.HasIndex(e => e.UserId);
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
