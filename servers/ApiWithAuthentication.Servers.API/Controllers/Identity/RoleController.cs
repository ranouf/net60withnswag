using ApiWithAuthentication.Domains.Core.Identity;
using ApiWithAuthentication.Domains.Core.Identity.Entities;
using ApiWithAuthentication.Servers.API.Attributes;
using ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos;
using ApiWithAuthentication.Servers.API.Filters.Dtos;
using ApiWithAuthentication.Librairies.Common;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SK.Extensions;
using SK.Session;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ApiWithAuthentication.Servers.API.Controllers.Identity
{
    [Route(Constants.Api.V1.Role.Url)]
    [AuthorizeAdministrators]
    public class RoleController : AuthentifiedBaseController
    {
        private readonly IRoleManager _roleManager;

        public RoleController(
            IUserSession session,
            IUserManager userManager,
            IRoleManager roleManager,
            ILogger<UserController> logger,
            IMapper mapper
        ) : base(session, userManager, mapper, logger)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), 200)]
        [ProducesResponseType(typeof(ApiErrorDto), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            Logger.LogInformation($"{nameof(GetAllRolesAsync)}, currentUser:{currentUser.ToJson()}");
            var result = await _roleManager.GetAllAsync();
            return new ObjectResult(Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDto>>(result));
        }
    }
}
