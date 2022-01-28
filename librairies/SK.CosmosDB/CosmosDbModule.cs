using Autofac;
using SK.CosmosDB.Repositories;

namespace SK.CosmosDB
{
    public class CosmosDbModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));
        }
    }
}
