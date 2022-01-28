using SK.EntityFramework.UnitOfWork;
using ApiWithAuthentication.Domains.Infrastructure.SqlServer;
using Autofac;
using System.Reflection;

namespace AllInOne.Domain.Infrastructure
{
    public class InfrastructureModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetEntryAssembly())
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces();

            //builder.RegisterType<AllInOneDbContext>().As<DbContext>().InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork<SKSamplesDbContext>>().As<IUnitOfWork>().InstancePerLifetimeScope();
        }
    }
}
