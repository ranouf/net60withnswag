using Autofac;

namespace SK.Session
{
    public class HttpContextSessionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HttpContextSession>().As<IUserSession>();
        }
    }
}
