using ApiWithAuthentication.Librairies.Common;
using Microsoft.AspNetCore.Authorization;

namespace ApiWithAuthentication.Servers.API.Attributes
{
    public class AuthorizeAdministratorAndManagersAttribute : AuthorizeAttribute
    {
        public AuthorizeAdministratorAndManagersAttribute()
        {
            Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}";
        }
    }
}
