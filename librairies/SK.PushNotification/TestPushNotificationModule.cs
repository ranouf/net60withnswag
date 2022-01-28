using Autofac;

namespace SK.PushNotification
{
    public class TestPushNotificationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TestPushNotificationService>().As<IPushNotificationService>();
        }
    }
}
