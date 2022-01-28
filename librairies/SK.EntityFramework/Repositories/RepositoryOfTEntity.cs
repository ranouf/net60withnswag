using SK.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace SK.EntityFramework.Repositories
{
    public class Repository<TEntity> : Repository<TEntity, Guid>, IRepository<TEntity>
      where TEntity : class, IEntity<Guid>
    {
        public Repository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
