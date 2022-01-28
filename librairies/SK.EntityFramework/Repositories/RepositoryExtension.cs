using SK.Entities;
using System;
using System.Linq;

namespace SK.EntityFramework.Repositories
{
    public static class RepositoryExtension
    {
        public static bool  IsAttached<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            return repository.GetDbContext().Set<TEntity>().Local.Any(e => e == entity);
        }
        public static bool IsAttached<TEntity>(this IRepository<TEntity> repository, TEntity entity) where TEntity : class, IEntity<Guid>
        {
            return repository.GetDbContext().Set<TEntity>().Local.Any(e => e == entity);
        }
    }
}
