using SK.CosmosDb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SK.CosmosDB.Repositories
{
    public interface IRepository
    {
        ICosmosDatabase CosmosDatabase { get; }
        IList<Task<(int success, int failed)>> Tasks { get; }
    }
}
