using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using ApiWithAuthentication.Tests.Data;
using System.Net.Http;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace ApiWithAuthentication.Tests.Controllers
{
    public class BaseTest
    {
        public ITestOutputHelper Output { get; }
        public TestServer Server { get; }
        public List<string> Logs { get; }
        public HttpClient Client { get; }
        public static IFormFile Logo
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = assembly
                    .GetManifestResourceNames()
                    .First(str => str.EndsWith("Assets.Logo.png"));

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    var ms = new MemoryStream();
                    try
                    {
                        stream.CopyTo(ms);
                        return new FormFile(ms, 0, ms.Length, resourceName, resourceName)
                        {
                            Headers = new HeaderDictionary(),
                            ContentType = "application/png"
                        };
                    }
                    finally
                    {
                        //ms.Dispose();
                    }
                }
            }
        }

        public BaseTest(ITestOutputHelper output)
        {
            Output = output;
            var factory = new TestWebAppFactory<Program>(output);

            // Seed Data
            var serviceProvider = factory.Services;
            using var scope = serviceProvider.CreateScope();
            TestDbInitializer.Seed(scope.ServiceProvider, Output);

            // HttpClient
            Client = factory.CreateClient();

            // Test Server
            Server = factory.Server;

            // Logs
            Logs = factory.Logs;
        }
    }
}
