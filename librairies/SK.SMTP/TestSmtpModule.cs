using SK.Smtp.SmtpClients;
using Autofac;

namespace SK.Smtp
{
    public class TestSmtpModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SmtpService>().As<ISmtpService>();
            builder.RegisterType<TestSmtpClientFactory>().As<ISmtpClientFactory>();
        }
    }
}
