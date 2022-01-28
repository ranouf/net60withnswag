using SK.Entities;
using SK.Extensions;
using SK.Session;
using ApiWithAuthentication.Domains.Core.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiWithAuthentication.Domains.Core.Items.Entities;

namespace ApiWithAuthentication.Domains.Infrastructure.SqlServer
{
    public class SKSamplesDbContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public IUserSession _session { get; private set; }
        protected Guid? UserId { get; set; }

        public DbSet<Item> Items { get; set; }


        public SKSamplesDbContext() { }

        public SKSamplesDbContext(DbContextOptions<SKSamplesDbContext> options) : base(options) { }

        public SKSamplesDbContext(
            DbContextOptions<SKSamplesDbContext> options,
            IUserSession session
        ) : base(options)
        {
            _session = session;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            PrepareIdentityModel(builder);
            PrepareItemModel(builder);

            static void PrepareIdentityModel(ModelBuilder builder)
            {
                builder.Entity<User>(entity =>
                {
                    entity.HasOne(u => u.CreatedByUser)
                        .WithMany()
                        .HasForeignKey(u => u.CreatedByUserId);
                    entity.HasOne(u => u.UpdatedByUser)
                        .WithMany()
                        .HasForeignKey(u => u.UpdatedByUserId);
                    entity.HasOne(u => u.DeletedByUser)
                        .WithMany()
                        .HasForeignKey(u => u.DeletedByUserId);
                    entity.HasOne(u => u.InvitedByUser)
                        .WithMany()
                        .HasForeignKey(u => u.InvitedByUserId);
                    entity.HasIndex(u => u.Email);
                    entity.HasIndex(u => u.NormalizedEmail);
                    entity.HasIndex(u => u.IsDeleted);
                    entity.Property(u => u.FullName)
                        .HasComputedColumnSql("[FirstName] + ' ' + [LastName]");
                    entity.HasIndex(u => u.IsDeleted);
                    entity.HasQueryFilter(p => !p.IsDeleted);
                });

                builder.Entity<Role>(entity =>
                {
                    entity.HasIndex(r => r.Name);
                    entity.HasIndex(r => r.NormalizedName);
                });

                builder.Entity<UserRole>(entity =>
                {
                    entity.HasKey(e => new { e.UserId, e.RoleId });

                    entity.HasOne(e => e.Role)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(e => e.RoleId)
                        .IsRequired();

                    entity.HasOne(e => e.User)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(e => e.UserId)
                        .IsRequired();
                });
            }

            static void PrepareItemModel(ModelBuilder builder)
            {
                builder.Entity<Item>(entity =>
                {
                    entity.HasIndex(u => u.Name);
                });
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_session.UserId.HasValue)
            {
                var user = Users.FirstOrDefault(u => u.Id == _session.UserId.Value);
                if (user != null)
                {
                    UserId = user.Id;
                }
            }

            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AddTimestamps()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        SetCreationAuditProperties(entry);
                        break;
                    case EntityState.Modified:
                        SetModificationAuditProperties(entry);

                        if (entry.Entity.IsAssignableToGenericType(typeof(ISoftDelete<>))
                            && entry.Entity.TryGetPropertyValue("IsDeleted", out bool isDeleted)
                            && isDeleted
                        )
                        {
                            SetDeletionAuditProperties(entry);
                        }
                        break;
                    case EntityState.Deleted:
                        CancelDeletionForSoftDelete(entry);
                        SetDeletionAuditProperties(entry);
                        break;
                }
            }
        }

        protected virtual void SetCreationAuditProperties(EntityEntry entry)
        {
            if (entry.Entity.IsAssignableToGenericType(typeof(ICreationAudited<>)))
            {
                if (entry.Entity.TryGetPropertyValue("CreatedAt", out DateTimeOffset? createdAt) && !createdAt.HasValue)
                {
                    entry.Entity.SetPropertyValue<DateTimeOffset?>("CreatedAt", DateTime.Now);
                }

                if (entry.Entity.TryGetPropertyValue("CreatedByUserId", out Guid? createdByUserId) && !createdByUserId.HasValue)
                {
                    entry.Entity.SetPropertyValue("CreatedByUserId", UserId);
                }
            }
        }

        protected virtual void SetModificationAuditProperties(EntityEntry entry)
        {
            if (entry.Entity.IsAssignableToGenericType(typeof(IUpdateAudited<>)))
            {
                entry.Entity.SetPropertyValue<DateTimeOffset?>("UpdatedAt", DateTime.Now);
                entry.Entity.SetPropertyValue("UpdatedByUserId", UserId);
            }
        }

        protected virtual void CancelDeletionForSoftDelete(EntityEntry entry)
        {
            if (entry.Entity.IsAssignableToGenericType(typeof(ISoftDelete<>)))
            {
                entry.State = EntityState.Unchanged;
                entry.Entity.SetPropertyValue("IsDeleted", true);
            }
        }

        protected virtual void SetDeletionAuditProperties(EntityEntry entry)
        {
            if (entry.Entity.IsAssignableToGenericType(typeof(IDeleteAudited<>)))
            {
                if (entry.Entity.TryGetPropertyValue("DeletedAt", out DateTimeOffset? deletedAt) && !deletedAt.HasValue)
                {
                    entry.Entity.SetPropertyValue<DateTimeOffset?>("DeletedAt", DateTime.Now);
                }

                if (entry.Entity.TryGetPropertyValue("DeletedByUserId", out Guid? deletedByUserId) && !deletedByUserId.HasValue)
                {
                    entry.Entity.SetPropertyValue("DeletedByUserId", UserId);
                }
            }
        }
    }
}
