namespace ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos
{
    public class ConfirmInvitationEmailRequestDto : ConfirmRegistrationEmailRequestDto
    {
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}
