using ApiWithAuthentication.Servers.API.Controllers.Dtos;
using System.ComponentModel.DataAnnotations;

namespace ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos
{
    public class ChangePasswordRequestDto : IDto
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords don't match.")]
        public string NewPasswordConfirmation { get; set; }
    }
}
