using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using SK.Extensions;
using SK.PushNotification.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SK.PushNotification
{
    public class TestPushNotificationService : IPushNotificationService
    {
        private readonly ILogger<TestPushNotificationService> _loggerService;

        public TestPushNotificationService(ILogger<TestPushNotificationService> loggerService)
        {
            _loggerService = loggerService;
        }

        public async Task RegisterDeviceAsync(string handle, string installationId, Platform platform, IList<string> tags)
        {
             _loggerService.LogInformation($"Register Device with handle:'{handle}', installationId:'{installationId}', platform:{platform}' and tags '{tags.ToJson()}'.");
            await Task.CompletedTask;
        }

        public async Task RevokeDeviceAsync(string installationId)
        {
            _loggerService.LogInformation($"Revoke Device with installationId:'{installationId}'");
            await Task.CompletedTask;
        }

        public async Task<PushNotificationResult> SendPushNotificationAsync(IDictionary<string, object> data, IList<string> tags)
        {
            await SendAppleNotificationAsync(data, tags);
            await SendAndroidNotificationAsync(data, tags);
            return new PushNotificationResult
            (
                0,
                0
            );
        }

        private async Task<NotificationOutcome> SendAppleNotificationAsync(IDictionary<string, object> data, IList<string> tags)
        {
            var payload = PushNotificationHelper.ConvertToApplePayload(data);
            _loggerService.LogInformation($"Sending a PushNotification (APNS) with tags '{tags.ToJson()}' and payload '{payload}'.");
            return await Task.FromResult(new NotificationOutcome());
        }

        private async Task<NotificationOutcome> SendAndroidNotificationAsync(IDictionary<string, object> data, IList<string> tags)
        {
            var payload = PushNotificationHelper.ConvertToAndroidPayload(data);
            _loggerService.LogInformation($"Sending a PushNotification (GCM) with tags '{tags.ToJson()}' and payload '{payload}'.");
            return await Task.FromResult(new NotificationOutcome());
        }
    }
}
