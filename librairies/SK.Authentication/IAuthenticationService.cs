using System.Security.Claims;

namespace SK.Authentication
{
    public interface IAuthenticationService
    {
        string GenerateToken(ClaimsIdentity claimsIdentity);
    }
}
