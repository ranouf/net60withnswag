using SK.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ApiWithAuthentication.Domains.Core.Identity.Entities
{
    public class Role : IdentityRole<Guid>, IEntity
    {
        public ICollection<UserRole> UserRoles { get; set; }

        public Role() { }

        public Role(string name)
        {
            Name = name;
            NormalizedName = name.ToUpper();
        }
    }
}
