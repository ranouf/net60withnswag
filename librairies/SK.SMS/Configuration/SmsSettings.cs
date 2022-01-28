using System.ComponentModel.DataAnnotations;

namespace SK.Sms.Configuration
{
    public class SmsSettings
    {
        [Required]
        public string AccountSid { get; set; }
        [Required]
        public string AuthToken { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }
}
