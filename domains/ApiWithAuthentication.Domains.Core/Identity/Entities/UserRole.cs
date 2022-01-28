using Microsoft.AspNetCore.Identity;
using System;

namespace ApiWithAuthentication.Domains.Core.Identity.Entities
{
    public class UserRole : IdentityUserRole<Guid>
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
