using System.ComponentModel.DataAnnotations;

namespace SK.Authentication.Configuration
{
    public class AuthenticationSettings
    {
        [Required]
        public string Issuer { get; set; }
        [Required]
        public string Audience { get; set; }
        [Required]
        public string SecretKey { get; set; }
        [Required]
        public int ExpirationDurationInDays { get; set; }
    }
}
