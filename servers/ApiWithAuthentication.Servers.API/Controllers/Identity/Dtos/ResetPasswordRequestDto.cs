using ApiWithAuthentication.Servers.API.Controllers.Dtos;
using System.ComponentModel.DataAnnotations;

namespace ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos
{
    public class ResetPasswordRequestDto : IDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords don't match.")]
        public string NewPasswordConfirmation { get; set; }
    }
}
