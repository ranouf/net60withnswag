using ApiWithAuthentication.Domains.Infrastructure.SqlServer;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SK.Extensions;
using SK.Testing.Logging.InMemory;
using SK.Testing.Logging.Xunit;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace ApiWithAuthentication.Tests
{
    public class TestWebAppFactory<TEntryPoint> : WebApplicationFactory<Program>
        where TEntryPoint : Program
    {
        public ITestOutputHelper Output { get; set; }
        public List<string> Logs { get; private set; } = new List<string>();

        public TestWebAppFactory([NotNull] ITestOutputHelper output)
        {
            Output = output;
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                containerBuilder.RegisterModule<TestsModule>();
            });

            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .ConfigureServices(services =>
                {
                    services.Remove<DbContextOptions<SKSamplesDbContext>>();
                    services.AddDbContext<SKSamplesDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("SK.Sample");
                    });
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    var env = hostContext.HostingEnvironment;
                    configApp.AddJsonFile("appsettings.json", optional: true);
                    configApp.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
                    configApp.AddEnvironmentVariables();
                    //configApp.AddApplicationInsightsSettings(developerMode: !env.IsProduction());
                    //configApp.AddCommandLine(args);
                })
                .ConfigureTestContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterModule<TestsModule>();
                })
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddXunitLogger(Output);
                    builder.AddInMemoryLogger(Logs);
                });
        }
    }
}