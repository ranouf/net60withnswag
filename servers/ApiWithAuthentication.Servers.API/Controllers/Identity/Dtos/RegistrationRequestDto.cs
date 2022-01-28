using ApiWithAuthentication.Servers.API.Controllers.Dtos;
using System.ComponentModel.DataAnnotations;

namespace ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos
{
    public class RegistrationRequestDto : IDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string PasswordConfirmation { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }
    }
}
