using ApiWithAuthentication.Servers.API.Controllers.Dtos;
using System;
using System.ComponentModel.DataAnnotations;

namespace ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos
{
    public class InviteUserRequestDto: IDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }
        [Required]
        public Guid RoleId { get; set; }
    }
}
