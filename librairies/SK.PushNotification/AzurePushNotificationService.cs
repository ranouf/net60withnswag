using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SK.Extensions;
using SK.PushNotification.Configuration;
using SK.PushNotification.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SK.PushNotification
{
    public enum Platform
    {
        IOS = 1,
        //Android = 2, // Currently only IOS is handled
    }

    public class AzurePushNotificationService : IPushNotificationService
    {
        private readonly PushNotificationSettings _pushNotificationSettings;
        private readonly ILogger<AzurePushNotificationService> _logger;
        private readonly NotificationHubClient _notificationHubClient;

        public AzurePushNotificationService(
            IOptions<PushNotificationSettings> pushNotificationSettings,
            ILogger<AzurePushNotificationService> logger
        )
        {
            _pushNotificationSettings = pushNotificationSettings.Value;
            _logger = logger;
            _notificationHubClient = NotificationHubClient.CreateClientFromConnectionString(
                _pushNotificationSettings.NotificationHubConnectionString,
                _pushNotificationSettings.NotificationHubName
            );
        }

        public async Task<PushNotificationResult> SendPushNotificationAsync(IDictionary<string, object> data, IList<string> tags)
        {
            var result = new PushNotificationResult();

            //result.Update(await SendAndroidNotificationAsync(data, tags)); // Currently only IOS is handled
            result.Update(await SendAppleNotificationAsync(data, tags));

            _logger.LogInformation($"PushNotifications sent: Failure: {result.Failure}, Success: {result.Success}, Data: {data.ToJson()}, Tags: {tags.ToJson()}");

            return result;
        }

        public async Task RegisterDeviceAsync(string handle, string installationId, Platform platform, IList<string> tags)
        {
            if (tags == null)
            {
                tags = new List<string>();
            }

            if (!tags.Contains(installationId))
            {
                tags.Add(installationId);
            }

            var installation = new Installation
            {
                InstallationId = installationId,
                PushChannel = handle,
                Tags = tags,

                Platform = platform switch
                {
                    //Platform.Android => NotificationPlatform.Fcm,
                    Platform.IOS => NotificationPlatform.Apns,
                    _ => throw new ArgumentException($"{nameof(Platform)} argument is not valid."),
                }
            };
            await _notificationHubClient.CreateOrUpdateInstallationAsync(installation);
        }

        public async Task RevokeDeviceAsync(string installationId)
        {
            await _notificationHubClient.DeleteInstallationAsync(installationId);
        }

        #region Private

        private async Task<PushNotificationResult> SendAndroidNotificationAsync(IDictionary<string, object> data, IList<string> tags)
        {
            // 1) Generate Payload
            var payload = PushNotificationHelper.ConvertToAndroidPayload(data);

            // 2) Send the payload
            var outcome = await _notificationHubClient.SendFcmNativeNotificationAsync(payload, tags);
            _logger.LogInformation($"PushNotifications sent to Android: Failure: {outcome.Failure}, Success: {outcome.Success}, Data: {data.ToJson()}, Tags: {tags.ToJson()}");
            return new PushNotificationResult
            (
                outcome.Success,
                outcome.Failure
            );
        }

        private async Task<PushNotificationResult> SendAppleNotificationAsync(IDictionary<string, object> data, IList<string> tags)
        {
            // 1) Generate Payload
            var payload = PushNotificationHelper.ConvertToApplePayload(data);

            // 2) Send the payload
            var outcome = await _notificationHubClient.SendAppleNativeNotificationAsync(payload, tags);
            _logger.LogInformation($"PushNotifications sent to Apple: Failure: {outcome.Failure}, Success: {outcome.Success}, Data: {data.ToJson()}, Tags: {tags.ToJson()}");
            return new PushNotificationResult
            (
                outcome.Success,
                outcome.Failure
            );
        }
        #endregion
    }
}
