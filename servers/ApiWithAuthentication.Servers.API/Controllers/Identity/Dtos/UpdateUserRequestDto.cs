using ApiWithAuthentication.Servers.API.Controllers.Dtos;
using System;
using System.ComponentModel.DataAnnotations;

namespace ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos
{
    public class UpdateUserRequestDto: IDto
    {
        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }
        public string Description { get; set; }

        [Required]
        public Guid RoleId { get; set; }
    }
}
