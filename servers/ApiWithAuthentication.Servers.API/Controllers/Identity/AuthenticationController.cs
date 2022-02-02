using ApiWithAuthentication.Librairies.Common;
using ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos;
using ApiWithAuthentication.Servers.API.Filters.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace ApiWithAuthentication.Servers.API.Controllers.Identity
{
    [Route(Constants.Api.V1.Authentication.Url)]
    [ApiController]
    public class AuthenticationController : AuthentifiedBaseController
    {

        public AuthenticationController(
          IMapper mapper,
          ILogger<AuthenticationController> logger
        ) : base(mapper, logger)
        {
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.InternalServerError)]
        [Route(Constants.Api.V1.Authentication.Register)]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegistrationRequestDto dto)
        {
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.InternalServerError)]
        [Route(Constants.Api.V1.Authentication.ResendEmailConfirmation)]
        public async Task<IActionResult> ResendEmailConfirmationAsync([FromBody] ResendEmailConfirmationRequestDto dto)
        {
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.Unauthorized)]
        [Route(Constants.Api.V1.Authentication.ConfirmRegistrationEmail)]
        public async Task<IActionResult> ConfirmRegistrationEmailAsync([FromBody] ConfirmRegistrationEmailRequestDto dto)
        {
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.Unauthorized)]
        [Route(Constants.Api.V1.Authentication.ConfirmInvitationEmail)]
        public async Task<IActionResult> ConfirmInvitationEmailAsync([FromBody] ConfirmInvitationEmailRequestDto dto)
        {
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(LoginResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.Unauthorized)]
        [Route(Constants.Api.V1.Authentication.Login)]
        public async Task<IActionResult> LoginUserAsync([FromBody] LoginRequestDto dto)
        {
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [Route(Constants.Api.V1.Authentication.PasswordForgotten)]
        public async Task<IActionResult> PasswordForgottenAsync([FromBody] PasswordForgottenRequestDto dto)
        {
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [Route(Constants.Api.V1.Authentication.ResetPassword)]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequestDto dto)
        {
            return Ok();
        }
    }
}
