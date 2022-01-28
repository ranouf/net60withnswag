
using ApiWithAuthentication.Domains.Core;
using ApiWithAuthentication.Domains.Core.Identity;
using ApiWithAuthentication.Domains.Core.Identity.Configuration;
using ApiWithAuthentication.Domains.Core.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWithAuthentication.Domains.Infrastructure.SqlServer
{
    public static class DbInitializer
    {
        public async static Task InitializeAsync(IServiceProvider services, ILogger logger)
        {
            var context = services.GetRequiredService<SKSamplesDbContext>();
            if (context.Database.IsRelational())
            {
                await services.MigrateDatabaseAsync(logger);
                await services.SeedRolesAsync(logger);
                await services.SeedUsersAsync(logger);
            }
        }

        #region Private
        private static async Task MigrateDatabaseAsync(this IServiceProvider services, ILogger logger)
        {
            try
            {
                logger.LogInformation("Start database migration.");
                var context = services.GetRequiredService<SKSamplesDbContext>();
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migration has been done.");
            }
            catch (Exception ex)
            {
                logger.LogError("Database migration has failed.", ex);
                throw;
            }
        }

        private static async Task SeedRolesAsync(this IServiceProvider services, ILogger logger)
        {
            try
            {
                logger.LogInformation("Start seeding roles.");
                var context = services.GetRequiredService<SKSamplesDbContext>();
                var roles = new string[] {
                    Constants.Roles.Administrator,
                    Constants.Roles.Manager,
                    Constants.Roles.User
                };
                var roleManager = services.GetRequiredService<IRoleManager>();
                foreach (var role in roles)
                {
                    if (!context.Roles.Any(r => r.Name == role))
                    {
                        await roleManager.CreateAsync(new Role(role));
                    }
                }
                await context.SaveChangesAsync();
                logger.LogInformation("Roles have been seeded.");
            }
            catch (Exception ex)
            {
                logger.LogError("Roles seeding has failed.", ex);
                throw;
            }
        }

        private static async Task SeedUsersAsync(this IServiceProvider services, ILogger logger)
        {
            try
            {
                logger.LogInformation("Start seeding users.");
                var context = services.GetRequiredService<SKSamplesDbContext>();
                var defaultUserAccountsSettings = services.GetRequiredService<IOptions<IdentitySettings>>().Value;

                var roleManager = services.GetRequiredService<IRoleManager>();
                var userManager = services.GetRequiredService<IUserManager>();
                foreach (var userAccount in defaultUserAccountsSettings.UserAccounts)
                {
                    if (!context.Users.Any(u => u.Email == userAccount.Email))
                    {
                        var role = await roleManager.FindByNameAsync(userAccount.RoleName);
                        await userManager.CreateAsync(
                            new User(
                                userAccount.Email,
                                userAccount.Firstname,
                                userAccount.Lastname,
                                emailConfirmed: true
                            ),
                            userAccount.Password,
                            role
                        );
                    }
                }
                await context.SaveChangesAsync();
                logger.LogInformation("Users have been seeded.");
            }
            catch (Exception ex)
            {
                logger.LogError("Users seeding has failed.", ex);
                throw;
            }
        }
        #endregion
    }
}
