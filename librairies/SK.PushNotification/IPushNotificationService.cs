using System.Collections.Generic;
using System.Threading.Tasks;

namespace SK.PushNotification
{
    public class PushNotificationResult
    {
        public long Success { get; private set; }
        public long Failure { get; internal set; }

        internal PushNotificationResult()
        {
        }

        internal PushNotificationResult(long success, long failure)
        {
            Success = success;
            Failure = failure;
        }

        internal PushNotificationResult Update(PushNotificationResult result)
        {
            Success = result.Success;
            Failure = result.Failure;
            return this;
        }
    }

    public interface IPushNotificationService
    {
        Task RegisterDeviceAsync(string handle, string installationId, Platform platform, IList<string> tags);
        Task RevokeDeviceAsync(string installationId);
        Task<PushNotificationResult> SendPushNotificationAsync(IDictionary<string, object> data, IList<string> tags);
    }
}
