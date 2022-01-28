using ApiWithAuthentication.Domains.Core;
using Microsoft.AspNetCore.Authorization;

namespace ApiWithAuthentication.Servers.API.Attributes
{
    public class AuthorizeAdministratorsAttribute : AuthorizeAttribute
    {
        public AuthorizeAdministratorsAttribute()
        {
            Roles = Constants.Roles.Administrator;
        }
    }
}
