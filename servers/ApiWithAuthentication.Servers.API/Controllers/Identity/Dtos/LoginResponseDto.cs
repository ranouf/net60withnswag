using ApiWithAuthentication.Servers.API.Controllers.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos
{
    public class LoginResponseDto : IDto
    {
        public string Token { get; set; }
        public UserDto CurrentUser { get; set; }
    }
}
