using ApiWithAuthentication.Domains.Core.Identity;
using ApiWithAuthentication.Domains.Core.Identity.Entities;
using ApiWithAuthentication.Domains.Infrastructure.SqlServer;
using Xunit.Abstractions;

namespace ApiWithAuthentication.Tests.Data
{
    public class TestUserDataBuilder : BaseDataBuilder
    {
        private readonly IUserManager _userManager;
        private readonly IRoleManager _roleManager;

        public const string AdministratorEmail = "administrator@sidekickinteractive.com";
        public const string AdministratorFirstname = "John";
        public const string AdministratorLastname = "Smith";
        public const string ManagerEmail = "manager@sidekickinteractive.com";
        public const string UserEmail = "user@sidekickinteractive.com";
        public const string Password = "Password123#";

        public TestUserDataBuilder(
            SKSamplesDbContext context,
            IUserManager userManager,
            IRoleManager roleManager,
            ITestOutputHelper output) : base(context, output)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public override void Seed()
        {
            var roles = new Role[]
            {
                new Role(Domains.Core.Constants.Roles.Administrator),
                new Role(Domains.Core.Constants.Roles.Manager),
                new Role(Domains.Core.Constants.Roles.User)
            };

            foreach (var role in roles)
            {
                _roleManager.CreateAsync(role).Wait();
            }
            Output.WriteLine($"{roles.Length} Roles have been created.");

            var users = new User[]
            {
                new User(AdministratorEmail, AdministratorFirstname, AdministratorLastname, emailConfirmed: true),
                new User(ManagerEmail, "Jack", "Wiliams", emailConfirmed: true),
                new User(UserEmail, "Donald", "Duck", emailConfirmed: true)
            };

            for (int i = 0; i < users.Length; i++)
            {
                var user = users[i];
                _userManager.CreateAsync(user, Password, roles[i]).Wait();
            }
            Output.WriteLine($"{users.Length} Users have been created.");
        }
    }
}
