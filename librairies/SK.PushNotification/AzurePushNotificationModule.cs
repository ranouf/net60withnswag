using Autofac;

namespace SK.PushNotification
{
    public class AzurePushNotificationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AzurePushNotificationService>().As<IPushNotificationService>();
        }
    }
}
