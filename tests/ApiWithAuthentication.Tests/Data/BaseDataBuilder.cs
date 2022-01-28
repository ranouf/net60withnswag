using ApiWithAuthentication.Domains.Infrastructure.SqlServer;
using Xunit.Abstractions;

namespace ApiWithAuthentication.Tests.Data
{
    public abstract class BaseDataBuilder
    {
        public readonly SKSamplesDbContext Context;
        public readonly ITestOutputHelper Output;

        public BaseDataBuilder(SKSamplesDbContext context, ITestOutputHelper output)
        {
            Context = context;
            Output = output;
        }

        public abstract void Seed();
    }
}
