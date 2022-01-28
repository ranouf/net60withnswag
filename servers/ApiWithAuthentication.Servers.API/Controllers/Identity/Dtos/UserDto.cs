using ApiWithAuthentication.Servers.API.Controllers.Dtos;
using ApiWithAuthentication.Servers.API.Controllers.Dtos.Entities;
using System;

namespace ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos
{
    public class UserDto : EntityDto<Guid?>, IEntityDto<Guid?>, IAuditedDto, IDeleteAuditedDto, IComparable, IDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ProfileImageUrl { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public bool IsLocked { get; set; }
        public string RoleName { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string DeletedAt { get; set; }
        public string InvitedBy { get; set; }

        public int CompareTo(object obj)
        {
            if (!(obj is UserDto user))
            {
                throw new InvalidCastException($"Not able to cast as '{typeof(UserDto).Name}'.");
            }
            return FullName.CompareTo(user.FullName);
        }
    }
}
