using Autofac;
using System;

namespace SK.Sms
{
    public class SmsModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SmsService>().As<ISmsService>();
        }
    }
}