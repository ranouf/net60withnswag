using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SK.Authentication.Helper
{
    public static class JWTHelper
    {
        public static SigningCredentials GetSigningCredentials(string secretKey)
        {
            var encodedKey = Encoding.ASCII.GetBytes(secretKey);
            var symecticSecurityKey = new SymmetricSecurityKey(encodedKey);
            return new SigningCredentials(symecticSecurityKey, SecurityAlgorithms.HmacSha256Signature);
        }
    }
}
