using ApiWithAuthentication.Tests.Data;
using ApiWithAuthentication.Tests.Extensions;
using ApiWithAuthentication.Servers.API.Controllers.Dtos.Paging;
using ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Common = ApiWithAuthentication.Librairies.Common;

namespace ApiWithAuthentication.Tests.Controllers.Identity
{
    [Collection(Constants.TEST_COLLECTION)]
    public class Users_Tests : BaseTest
    {
        private const string Email = "newuser@sidekickinteractive.com";
        private const string Password = "Password123#";
        private const string NewFirstname = "Peter";
        private const string NewLastname = "Parker";

        public Users_Tests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task Should_Not_Get_Users_As_Anonymous()
        {
            Client.AuthenticateAsAnonymous();
            var response = await Client.GetAsync(
                Common.Constants.Api.V1.User.Url,
                Output,
                new PagedRequestDto
                {
                    MaxResultCount = 2,
                    SkipCount = 0
                }
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Should_Update_User_As_Administrator()
        {
            // As Admin
            await Client.AuthenticateAsAdministratorAsync(Output);

            // Get Roles
            var response = await Client.GetAsync(
                Common.Constants.Api.V1.Role.Url,
                Output
            );
            var rolesDto = await response.ConvertToAsync<IEnumerable<RoleDto>>(Output);

            // Get Manager
            response = await Client.AuthenticateAsManagerAsync(Output);
            var loginResponseDto = await response.ConvertToAsync<LoginResponseDto>(Output);

            // As Admin
            await Client.AuthenticateAsAdministratorAsync(Output);

            // Update User
            response = await Client.PutByIdAsync(
                Common.Constants.Api.V1.User.Url,
                Output,
                loginResponseDto.CurrentUser.Id,
                new UpdateUserRequestDto
                {
                    Firstname = NewFirstname,
                    Lastname = NewLastname,
                    RoleId = rolesDto.First(r => r.Name == Domains.Core.Constants.Roles.Administrator).Id
                }
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var dto = await response.ConvertToAsync<UserDto>(Output);

            Assert.Equal(NewFirstname, dto.Firstname);
            Assert.Equal(NewLastname, dto.Lastname);
            Assert.Equal(Domains.Core.Constants.Roles.Administrator, dto.RoleName);

            // Set to default
            response = await Client.PutByIdAsync(
                Common.Constants.Api.V1.User.Url,
                Output,
                loginResponseDto.CurrentUser.Id,
                new UpdateUserRequestDto
                {
                    Firstname = TestUserDataBuilder.AdministratorFirstname,
                    Lastname = TestUserDataBuilder.AdministratorLastname,
                    RoleId = rolesDto.First(r => r.Name == Domains.Core.Constants.Roles.Manager).Id
                }
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Get_Two_Users_Order_By_LastName_As_Administrator()
        {
            await Client.AuthenticateAsAdministratorAsync(Output);
            var response = await Client.GetAsync(
                Common.Constants.Api.V1.User.Url,
                Output,
                new PagedRequestDto
                {
                    MaxResultCount = 2,
                    SkipCount = 0
                }
            );
            var dto = await response.ConvertToAsync<PagedResultDto<UserDto, Guid?>>(Output);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(dto);
            Assert.Equal(3, dto.TotalCount);
            Assert.Equal(2, dto.Items.Count);
            Assert.True(dto.HasNext);
        }

        [Fact]
        public async Task Should_Get_By_User_Id_As_Administrator()
        {
            await Client.AuthenticateAsAdministratorAsync(Output);
            var response = await Client.GetAsync(
                Common.Constants.Api.V1.User.Url,
                Output,
                new PagedRequestDto()
            );
            var usersDto = await response.ConvertToAsync<PagedResultDto<UserDto, Guid?>>(Output);
            var administrator = usersDto.Items.First(u => u.RoleName == Domains.Core.Constants.Roles.Administrator);

            await Client.AuthenticateAsAdministratorAsync(Output);
            response = await Client.GetByIdAsync(
                Common.Constants.Api.V1.User.Url,
                Output,
                administrator.Id
            );
            var userDto = await response.ConvertToAsync<UserDto>(Output);

            Assert.Equal(administrator.Id, userDto.Id);
            Assert.Equal(administrator.Firstname, userDto.Firstname);
            Assert.Equal(administrator.Lastname, userDto.Lastname);
            Assert.Equal(administrator.Email, userDto.Email);
        }

        [Fact]
        public async Task Should_Not_Lock_Myself_As_Administrator()
        {
            // As Admin
            var response = await Client.AuthenticateAsAdministratorAsync(Output);
            var dto = await response.ConvertToAsync<LoginResponseDto>(Output);

            // Not Lock
            response = await Client.PutByIdAsync(
                Common.Constants.Api.V1.User.Lock,
                Output,
                dto.CurrentUser
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Should_Lock_And_Unlock_User_As_Administrator()
        {
            // As User
            var response = await Client.AuthenticateAsUserAsync(Output);
            var dto = await response.ConvertToAsync<LoginResponseDto>(Output);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // As Admin
            await Client.AuthenticateAsAdministratorAsync(Output);

            // Lock User
            response = await Client.PutByIdAsync(
                Common.Constants.Api.V1.User.Lock,
                Output,
                dto.CurrentUser
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // As locked user
            response = await Client.AuthenticateAsAsync(
                TestUserDataBuilder.UserEmail,
                TestUserDataBuilder.Password,
                Output
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            // As Admin
            await Client.AuthenticateAsAdministratorAsync(Output);

            // Unlock User
            response = await Client.PutByIdAsync(
                Common.Constants.Api.V1.User.Unlock,
                Output,
                dto.CurrentUser
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // As User
            response = await Client.AuthenticateAsAsync(
                TestUserDataBuilder.UserEmail,
                TestUserDataBuilder.Password,
                Output
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Not_Delete_Myself_As_Administrator()
        {
            // As Admin
            var response = await Client.AuthenticateAsAdministratorAsync(Output);
            var dto = await response.ConvertToAsync<LoginResponseDto>(Output);

            // Not Lock
            response = await Client.DeleteAsync(
                Common.Constants.Api.V1.User.Url,
                Output,
                dto.CurrentUser
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Should_Invite_Confirm_Login_And_Delete_As_Administrator()
        {
            // As Admin
            await Client.AuthenticateAsAdministratorAsync(Output);

            // Roles
            var response = await Client.GetAsync(
                Common.Constants.Api.V1.Role.Url,
                Output
            );
            var rolesDto = await response.ConvertToAsync<RoleDto[]>(Output);

            // Create new User
            var newUser = new InviteUserRequestDto
            {
                Email = Email,
                Firstname = "FirstName",
                Lastname = "LastName",
                RoleId = rolesDto.First(r => r.Name == Domains.Core.Constants.Roles.Administrator).Id
            };
            response = await Client.PostAsync(
                Common.Constants.Api.V1.User.Url,
                Output,
                newUser
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var userDto = await response.ConvertToAsync<UserDto>(Output);
            Assert.Equal(Email, userDto.Email);
            Assert.Equal("FirstName", userDto.Firstname);
            Assert.Equal("LastName", userDto.Lastname);
            Assert.Equal(Domains.Core.Constants.Roles.Administrator, userDto.RoleName);
            Assert.False(string.IsNullOrEmpty(userDto.RoleName));
            Assert.False(string.IsNullOrEmpty(userDto.InvitedBy));

            // Get token from Logs
            var log = Logs.Last(l => l.Contains("EmailConfirmationToken"));
            var token = Regex.Matches(log, @"(?<=\')(.*?)(?=\')").First().Value;

            // Confirm invitation email
            response = await Client.PutAsync(
                Common.Constants.Api.V1.Authentication.ConfirmInvitationEmail,
                Output,
                new ConfirmInvitationEmailRequestDto
                {
                    Token = token,
                    Password = Password,
                    PasswordConfirmation = Password,
                    Email = Email
                }
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Login
            response = await Client.AuthenticateAsAsync(Email, Password, Output);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var dto = await response.ConvertToAsync<LoginResponseDto>(Output);

            // As Admin
            await Client.AuthenticateAsAdministratorAsync(Output);

            // Delete User
            response = await Client.DeleteAsync(
                Common.Constants.Api.V1.User.Url,
                Output,
                dto.CurrentUser
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Not Login
            response = await Client.AuthenticateAsAsync(Email, Password, Output);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
