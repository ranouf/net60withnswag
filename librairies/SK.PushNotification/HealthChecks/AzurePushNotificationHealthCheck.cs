using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SK.PushNotification.Configuration;
using SK.Settings.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SK.Smtp.HealthChecks
{
    public class AzurePushNotificationHealthCheck: SettingsHealthCheck<PushNotificationSettings>
    {
        public AzurePushNotificationHealthCheck(
            IOptions<PushNotificationSettings> options
        ): base(options)
        {

        }

        public async override Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // Settings Validation
            var result = await base.CheckHealthAsync(context, cancellationToken);
            if (result.Status == HealthStatus.Unhealthy)
            {
                return result;
            }

            // Push Notification Client Validation
            try
            {
                new NotificationHubClient(
                    Settings.NotificationHubConnectionString,
                    Settings.NotificationHubName
                );
            }
            catch (Exception e)
            {
                return await Task.FromResult(HealthCheckResult.Unhealthy(
                    description: $"PushNotification initialization failed with Error: '{e.Message}'",
                    exception: e
                ));
            }
            return await Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
