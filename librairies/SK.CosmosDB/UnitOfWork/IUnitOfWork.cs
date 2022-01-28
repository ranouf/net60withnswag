using SK.CosmosDB.Repositories;
using SK.Entities;
using System.Threading.Tasks;

namespace SK.CosmosDB.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity<string>;
        Task<(int success, int failed)> SaveChangesAsync();
    }
}