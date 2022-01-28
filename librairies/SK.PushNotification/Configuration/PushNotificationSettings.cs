using System.ComponentModel.DataAnnotations;

namespace SK.PushNotification.Configuration
{
    public class PushNotificationSettings
    {
        [Required]
        public string NotificationHubConnectionString { get; set; }
        [Required]
        public string NotificationHubName { get; set; }
    }
}
