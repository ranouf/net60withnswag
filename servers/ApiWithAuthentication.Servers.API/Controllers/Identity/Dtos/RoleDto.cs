using ApiWithAuthentication.Servers.API.Controllers.Dtos;
using System;

namespace ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos
{
    public class RoleDto : IDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
