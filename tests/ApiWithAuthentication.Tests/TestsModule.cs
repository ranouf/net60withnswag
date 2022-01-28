using Autofac;
using SK.Smtp;
using SK.Smtp.SmtpClients;

namespace ApiWithAuthentication.Tests
{
    public class TestsModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<TestSmtpModule>();
        }
    }
}
