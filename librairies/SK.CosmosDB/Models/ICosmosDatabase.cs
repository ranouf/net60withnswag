using SK.Entities;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace SK.CosmosDb.Models
{
    public interface ICosmosDatabase
    {
        CosmosClient CosmosClient { get; }
        Task EnsureCreatedAsync();
        CosmosContainer<TEntity> GetCosmosContainer<TEntity>() where TEntity : IEntity<string>;
    }
}