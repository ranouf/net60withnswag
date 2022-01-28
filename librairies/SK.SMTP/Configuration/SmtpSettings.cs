using System.ComponentModel.DataAnnotations;

namespace SK.Smtp.Configuration
{
    public class SmtpSettings
    {
        [Required]
        public string ProjectName { get; set; }
        [Required]
        public string Server { get; set; }
        [Required]
        public int Port { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string DefaultFrom { get; set; }
        [Required]
        public bool EnableSsl { get; set; }
    }
}
