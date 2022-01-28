using ApiWithAuthentication.Tests.Data;
using ApiWithAuthentication.Tests.Extensions;
using ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Common = ApiWithAuthentication.Librairies.Common;

namespace ApiWithAuthentication.Tests.Controllers.Identity
{
    [Collection(Constants.TEST_COLLECTION)]
    public class Account_Tests : BaseTest
    {
        private const string NewFirstname = "Peter";
        private const string NewLastname = "Parker";
        private const string NewPassword = "Password123#!";

        public Account_Tests(ITestOutputHelper output) : base(output)
        {

        }

        [Fact]
        public async Task Should_Change_Password_As_Administrator()
        {
            await Client.AuthenticateAsAdministratorAsync(Output);
            var response = await Client.PutAsync(
                Common.Constants.Api.V1.Account.Password,
                Output,
                new ChangePasswordRequestDto
                {
                    CurrentPassword = TestUserDataBuilder.Password,
                    NewPassword = NewPassword,
                    NewPasswordConfirmation = NewPassword
                }
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await Client.AuthenticateAsAsync(
                TestUserDataBuilder.AdministratorEmail,
                NewPassword,
                Output
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await Client.PutAsync(
                Common.Constants.Api.V1.Account.Password,
                Output,
                new ChangePasswordRequestDto
                {
                    CurrentPassword = NewPassword,
                    NewPassword = TestUserDataBuilder.Password,
                    NewPasswordConfirmation = TestUserDataBuilder.Password
                }
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await Client.AuthenticateAsAsync(
                TestUserDataBuilder.AdministratorEmail,
                TestUserDataBuilder.Password,
                Output
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Update_Profile_As_Administrator()
        {
            await Client.AuthenticateAsAdministratorAsync(Output);
            var response = await Client.PutAsync(
                Common.Constants.Api.V1.Account.Profile,
                Output,
                new ChangeProfileRequestDto
                {
                    Firstname = NewFirstname,
                    Lastname = NewLastname,
                    ProfileImage = Logo
                },
                Format.FormData
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var dto = await response.ConvertToAsync<UserDto>(Output);

            Assert.Equal(NewFirstname, dto.Firstname);
            Assert.Equal(NewLastname, dto.Lastname);

            response = await Client.PutAsync(
                Common.Constants.Api.V1.Account.Profile,
                Output,
                new ChangeProfileRequestDto
                {
                    Firstname = TestUserDataBuilder.AdministratorFirstname,
                    Lastname = TestUserDataBuilder.AdministratorLastname
                },
                Format.FormData
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Get_Profile_As_Administrator()
        {
            await Client.AuthenticateAsAdministratorAsync(Output);
            var response = await Client.GetAsync(
                Common.Constants.Api.V1.Account.Profile,
                Output
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var dto = await response.ConvertToAsync<UserDto>(Output);
            Assert.Equal(TestUserDataBuilder.AdministratorFirstname, dto.Firstname);
            Assert.Equal(TestUserDataBuilder.AdministratorLastname, dto.Lastname);
        }
    }
}
