using ApiWithAuthentication.Librairies.Common;
using ApiWithAuthentication.Servers.API.Attributes;
using ApiWithAuthentication.Servers.API.Controllers.Dtos.Paging;
using ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos;
using ApiWithAuthentication.Servers.API.Filters.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ApiWithAuthentication.Servers.API.Controllers.Identity
{
    [Route(Constants.Api.V1.User.Url)]
    [AuthorizeAdministrators]
    public class UserController : AuthentifiedBaseController
    {

        public UserController(
            ILogger<UserController> logger,
            IMapper mapper
        ) : base(mapper, logger)
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.InternalServerError)]
        [AuthorizeAdministratorAndManagers]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] Guid id)
        {
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<UserDto, Guid?>), 200)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [AuthorizeAdministrators]
        public async Task<IActionResult> GetUsersAsync([FromQuery] PagedRequestDto dto)
        {
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.InternalServerError)]
        [AuthorizeAdministrators]
        public async Task<IActionResult> InviteUserAsync([FromBody] InviteUserRequestDto dto)
        {
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.InternalServerError)]
        [AuthorizeAdministrators]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateUserAsync([FromRoute] Guid id, [FromBody] UpdateUserRequestDto dto)
        {
            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.InternalServerError)]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid id)
        {
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.InternalServerError)]
        [Route(Constants.Api.V1.User.Lock)]
        public async Task<IActionResult> LockUserAsync([FromRoute] Guid id)
        {
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.InternalServerError)]
        [Route(Constants.Api.V1.User.Unlock)]
        public async Task<IActionResult> UnlockUserAsync([FromRoute] Guid id)
        {
            return Ok();
        }
    }
}
