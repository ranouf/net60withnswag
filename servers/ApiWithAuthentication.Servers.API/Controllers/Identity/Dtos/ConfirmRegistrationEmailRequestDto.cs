using ApiWithAuthentication.Servers.API.Controllers.Dtos;

namespace ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos
{
    public class ConfirmRegistrationEmailRequestDto : IDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
