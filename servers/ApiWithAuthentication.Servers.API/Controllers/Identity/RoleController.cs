using ApiWithAuthentication.Librairies.Common;
using ApiWithAuthentication.Servers.API.Attributes;
using ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos;
using ApiWithAuthentication.Servers.API.Filters.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ApiWithAuthentication.Servers.API.Controllers.Identity
{
    [Route(Constants.Api.V1.Role.Url)]
    [AuthorizeAdministrators]
    public class RoleController : AuthentifiedBaseController
    {

        public RoleController(
            ILogger<UserController> logger,
            IMapper mapper
        ) : base(mapper, logger)
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), 200)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            return Ok();
        }
    }
}
