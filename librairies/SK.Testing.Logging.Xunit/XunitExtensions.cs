using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace SK.Testing.Logging.Xunit
{
    public static class XunitExtensions
    {
        public static ILoggingBuilder AddXunitLogger(this ILoggingBuilder builder, ITestOutputHelper output)
        {
            builder.Services.AddSingleton<ILoggerProvider>(new XunitLoggerProvider(output));
            return builder;
        }

        public static WebApplicationFactory<TStartup> WithTestLogging<TStartup>(this WebApplicationFactory<TStartup> factory, ITestOutputHelper output) where TStartup : class
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(builder =>
                {
                    builder.Services.AddSingleton<ILoggerProvider>(new XunitLoggerProvider(output));
                    builder.Services.AddSingleton(output);
                });
            });
        }
    }
}
