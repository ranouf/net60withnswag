using ApiWithAuthentication.Servers.API.Controllers.Dtos;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos
{
    public class ChangeProfileRequestDto : IDto
    {
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}
