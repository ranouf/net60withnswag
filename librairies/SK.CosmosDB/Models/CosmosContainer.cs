using SK.Entities;
using Microsoft.Azure.Cosmos;

namespace SK.CosmosDb.Models
{
    public class CosmosContainer<T> where T : IEntity<string>
    {
        public string Name { get; }
        public IndexingPolicy IndexingPolicy { get; }
        public Container Container { get; set; }

        public CosmosContainer(string name, IndexingPolicy indexingPolicy = null)
        {
            Name = name;
            IndexingPolicy = indexingPolicy;
        }
    }
}
