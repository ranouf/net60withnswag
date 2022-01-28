using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;

namespace SK.PushNotification.Helpers
{
    public static class PushNotificationHelper
    {
        public static string ConvertToApplePayload(IDictionary<string, object> data)
        {
            dynamic notification = new ExpandoObject();
            notification.aps = new ExpandoObject();
            notification.aps.alert = new ExpandoObject();
            foreach (var item in data)
            {
                ((IDictionary<string, object>)notification.aps.alert)[item.Key] = item.Value;
            }
            var result = JsonConvert.SerializeObject(notification);
            return result;
        }

        public static string ConvertToAndroidPayload(IDictionary<string, object> data)
        {
            dynamic notification = new ExpandoObject();
            notification.data = new ExpandoObject();
            foreach (var item in data)
            {
                ((IDictionary<string, object>)notification.data)[item.Key] = item.Value;
            }
            var result = JsonConvert.SerializeObject(notification);
            return result;
        }
    }
}
