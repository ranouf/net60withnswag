using ApiWithAuthentication.Servers.API.SignalR;
using SK.Authentication;
using SK.Events;

using SK.Session;
using SK.Settings;
using ApiWithAuthentication.Domains.Core;
using AllInOne.Domain.Infrastructure;
using ApiWithAuthentication.Servers.API.Filters;
using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ApiWithAuthentication.Servers.API
{
    public class APIModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().InstancePerLifetimeScope();
            builder.RegisterType<ApiExceptionFilter>();
            builder.RegisterType<SettingsValidator>();

            builder.RegisterModule<CoreModule>();
            builder.RegisterModule<InfrastructureModule>();
            builder.RegisterModule<AuthenticationModule>();
            builder.RegisterModule<HttpContextSessionModule>();

            // SignalR
            builder.RegisterType<ConnectionService>().As<IConnectionService>().SingleInstance();
            IEnumerable<Assembly> assemblies = GetAssemblies();
            builder.RegisterAssemblyTypes(assemblies.ToArray())
                .AsClosedTypesOf(typeof(IEventHandler<>))
                .InstancePerLifetimeScope();
        }

        private static Assembly[] GetAssemblies()
        {
            var assemblies = new List<Assembly>();
            var libraries = DependencyContext.Default.RuntimeLibraries.Where(rl => rl.Name.StartsWith("AllInOne"));
            foreach (var library in libraries)
            {
                var assembly = Assembly.Load(new AssemblyName(library.Name));
                assemblies.Add(assembly);
            }
            return assemblies.ToArray();
        }
    }
}
