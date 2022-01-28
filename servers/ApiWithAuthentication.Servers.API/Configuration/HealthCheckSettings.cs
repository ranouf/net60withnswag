using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace ApiWithAuthentication.Servers.API.Configuration
{
    public class HealthCheckSettings 
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Uri { get; set; }
        [Required]
        public string HealthCheckDatabaseConnectionString { get; set; }
        [Required]
        public int EvaluationTimeinSeconds { get; set; }
        [Required]
        public int MinimumSecondsBetweenFailureNotifications { get; set; }

        public static HealthCheckSettings FromConfiguration(IConfiguration configuration)
        {
            if (configuration["HealthCheck:Name"] != null)
            {
                var result = new HealthCheckSettings();
                configuration.Bind("HealthCheck", result);
                return result;
            }
            return null;
        }
    }
}
