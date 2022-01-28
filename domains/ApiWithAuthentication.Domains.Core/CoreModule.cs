using SK.Events;
using SK.Smtp;
using SK.Storage;
using Autofac;
using System.Reflection;

namespace ApiWithAuthentication.Domains.Core
{
    public class CoreModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var core = typeof(CoreModule).GetTypeInfo().Assembly;
            builder.RegisterAssemblyTypes(core)
                   .Where(t => t.Name.EndsWith("Manager"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
            builder.RegisterModule<EventsModule>();
            builder.RegisterModule<SmtpModule>();
            builder.RegisterModule<StorageModule>();
        }
    }
}
