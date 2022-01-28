using Microsoft.AspNetCore.Hosting;
using System.Reflection;

namespace ApiWithAuthentication.Tests.Extensions
{
    public static class IWebHostBuilderExtensions
    {

        public static IWebHostBuilder BasedOn<TStartup>(this IWebHostBuilder builder) where TStartup : class
        {
            // Set the "right" application after the startup's been registerted
            // source: https://github.com/aspnet/AspNetCore/issues/3334#issuecomment-405746266
            return builder.UseSetting(WebHostDefaults.ApplicationKey, typeof(TStartup).GetTypeInfo().Assembly.GetName().Name);
        }
    }
}
