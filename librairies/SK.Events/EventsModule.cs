using Autofac;

namespace SK.Events
{
    public class EventsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DomainEvents>().As<IDomainEvents>();
        }
    }
}
