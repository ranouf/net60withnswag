using ApiWithAuthentication.Servers.API.Filters;
using Autofac;
using Microsoft.AspNetCore.Http;

namespace ApiWithAuthentication.Servers.API
{
    public class APIModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().InstancePerLifetimeScope();
            builder.RegisterType<ApiExceptionFilter>();
        }
    }
}
