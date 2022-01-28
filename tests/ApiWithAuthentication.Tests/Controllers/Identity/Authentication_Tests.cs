using ApiWithAuthentication.Tests.Data;
using ApiWithAuthentication.Tests.Extensions;
using ApiWithAuthentication.Servers.API.Controllers.Dtos.Paging;
using ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos;
using System;
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
    public class Authentication_Tests : BaseTest
    {
        private const string NewEmail = "newregistration@sidekickinteractive.com";
        private const string NewFirstname = "Firstname";
        private const string NewLastname = "Lastname";
        private const string NewPassword = "Password321#";

        public Authentication_Tests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Should_Register_And_Not_Login_As_Anonymous()
        {
            // As Anonymous
            Client.AuthenticateAsAnonymous();

            // Register 
            var response = await Client.PostAsync(
                Common.Constants.Api.V1.Authentication.Register,
                Output,
                new RegistrationRequestDto
                {
                    Email = NewEmail,
                    Firstname = NewFirstname,
                    Lastname = NewLastname,
                    Password = NewPassword,
                    PasswordConfirmation = NewPassword
                }
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await Client.AuthenticateAsAsync(NewEmail, NewPassword, Output);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            // As Admin
            await Client.AuthenticateAsAdministratorAsync(Output);

            // Get list of users
            await Client.AuthenticateAsAdministratorAsync(Output);
            response = await Client.GetAsync(
                Common.Constants.Api.V1.User.Url,
                Output,
                new PagedRequestDto { }
            );
            var dto = await response.ConvertToAsync<PagedResultDto<UserDto, Guid?>>(Output);
            var currentUser = dto.Items.First(u => u.Email == NewEmail);

            // Delete User
            response = await Client.DeleteAsync(
                Common.Constants.Api.V1.User.Url,
                Output,
                currentUser
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Register_Confirm_And_Login_As_Anonymous()
        {
            // As Anonymous
            Client.AuthenticateAsAnonymous();

            // Register 
            var response = await Client.PostAsync(
                Common.Constants.Api.V1.Authentication.Register,
                Output,
                new RegistrationRequestDto
                {
                    Email = NewEmail,
                    Firstname = NewFirstname,
                    Lastname = NewLastname,
                    Password = NewPassword,
                    PasswordConfirmation = NewPassword
                }
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Get token from Logs
            var log = Logs.Last(l => l.Contains("EmailConfirmationToken"));
            var token = Regex.Matches(log, @"(?<=\')(.*?)(?=\')").First().Value;

            // Confirm invitation email
            response = await Client.PutAsync(
                Common.Constants.Api.V1.Authentication.ConfirmRegistrationEmail,
                Output,
                new ConfirmRegistrationEmailRequestDto
                {
                    Token = token,
                    Email = NewEmail
                }
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Authentication as the new User
            response = await Client.AuthenticateAsAsync(NewEmail, NewPassword, Output);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var dto = await response.ConvertToAsync<LoginResponseDto>(Output);
            Assert.Equal(NewEmail, dto.CurrentUser.Email);
            Assert.Equal(NewFirstname, dto.CurrentUser.Firstname);
            Assert.Equal(NewLastname, dto.CurrentUser.Lastname);

            // As Admin
            await Client.AuthenticateAsAdministratorAsync(Output);

            // Delete User
            response = await Client.DeleteAsync(
                Common.Constants.Api.V1.User.Url,
                Output,
                dto.CurrentUser
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Register_Resend_Confirm_And_Login_As_Anonymous()
        {
            // As Anonymous
            Client.AuthenticateAsAnonymous();

            // Register 
            var response = await Client.PostAsync(
                Common.Constants.Api.V1.Authentication.Register,
                Output,
                new RegistrationRequestDto
                {
                    Email = NewEmail,
                    Firstname = NewFirstname,
                    Lastname = NewLastname,
                    Password = NewPassword,
                    PasswordConfirmation = NewPassword
                }
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Register 
            response = await Client.PostAsync(
                Common.Constants.Api.V1.Authentication.ResendEmailConfirmation,
                Output,
                new ResendEmailConfirmationRequestDto
                {
                    Email = NewEmail
                }
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Get token from Logs
            var log = Logs.Last(l => l.Contains("EmailConfirmationToken"));
            var token = Regex.Matches(log, @"(?<=\')(.*?)(?=\')").First().Value;

            // Confirm invitation email
            response = await Client.PutAsync(
                Common.Constants.Api.V1.Authentication.ConfirmRegistrationEmail,
                Output,
                new ConfirmRegistrationEmailRequestDto
                {
                    Token = token,
                    Email = NewEmail
                }
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Authentication as the new User
            response = await Client.AuthenticateAsAsync(NewEmail, NewPassword, Output);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var dto = await response.ConvertToAsync<LoginResponseDto>(Output);
            Assert.Equal(NewEmail, dto.CurrentUser.Email);
            Assert.Equal(NewFirstname, dto.CurrentUser.Firstname);
            Assert.Equal(NewLastname, dto.CurrentUser.Lastname);

            // As Admin
            await Client.AuthenticateAsAdministratorAsync(Output);

            // Delete User
            response = await Client.DeleteAsync(
                Common.Constants.Api.V1.User.Url,
                Output,
                dto.CurrentUser
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Should_Reset_Password_As_Anonymous()
        {
            // As Anonymous
            Client.AuthenticateAsAnonymous();

            // Password forgotten 
            var response = await Client.PostAsync(
                Common.Constants.Api.V1.Authentication.PasswordForgotten,
                Output,
                new PasswordForgottenRequestDto
                {
                    Email = TestUserDataBuilder.AdministratorEmail
                }
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Get token from Logs
            var log = Logs.Last(l => l.Contains("PasswordResetToken"));
            var token = Regex.Matches(log, @"(?<=\')(.*?)(?=\')").First().Value;

            // Reset password 
            response = await Client.PutAsync(
                Common.Constants.Api.V1.Authentication.ResetPassword,
                Output,
                new ResetPasswordRequestDto
                {
                    Token = token,
                    NewPassword = NewPassword,
                    NewPasswordConfirmation = NewPassword,
                    Email = TestUserDataBuilder.AdministratorEmail
                }
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Authentication as Administrator using original Password
            response = await Client.AuthenticateAsAsync(
                TestUserDataBuilder.AdministratorEmail,
                TestUserDataBuilder.Password,
                Output
            );
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            // Authentication as Administrator using new Password
            response = await Client.AuthenticateAsAsync(
                TestUserDataBuilder.AdministratorEmail,
                NewPassword,
                Output
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Restore original Password
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
        }
    }
}
