using SK.CosmosDb.Models;
using SK.CosmosDB.Repositories;
using SK.Entities;
using System.Threading.Tasks;

namespace SK.CosmosDB.UnitOfWork
{
    public interface IUnitOfWork<TCosmosDatabase> : IUnitOfWork where TCosmosDatabase : ICosmosDatabase
    {
        ICosmosDatabase CosmosDatabase { get; }
    }
}