using ApiWithAuthentication.Domains.Core.Identity;
using ApiWithAuthentication.Domains.Core.Items;
using ApiWithAuthentication.Domains.Infrastructure.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit.Abstractions;

namespace ApiWithAuthentication.Tests.Data
{
    public static class TestDbInitializer
    {
        public static void Seed(IServiceProvider services, ITestOutputHelper output)
        {
            try
            {
                var context = services.GetRequiredService<SKSamplesDbContext>();
                if (context.Database.EnsureCreated())
                {
                    var itemManager = services.GetRequiredService<IItemManager>();

                    new TestItemDataBuilder(context, itemManager, output).Seed();

                    var userManager = services.GetRequiredService<IUserManager>();
                    var roleManager = services.GetRequiredService<IRoleManager>();

                    new TestUserDataBuilder(context, userManager, roleManager, output).Seed();

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                output.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
