using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiWithAuthentication.Domains.Core.Identity.Configuration
{
    public class UserAccount
    {
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string RoleName { get; set; }
    }

    public class IdentitySettings
    {
        [Required]
        public bool EnableConfirmEmailOnRegistration { get; set; }

        [Required]
        [MinLength(1)]
        public List<UserAccount> UserAccounts { get; set; }
    }
}
