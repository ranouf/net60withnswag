using Moq;

namespace SK.Smtp.SmtpClients
{
    public class TestSmtpClientFactory : ISmtpClientFactory
    {
        public ISmtpClient CreateSmtpClient()
        {
            return new Mock<ISmtpClient>().Object;
        }
    }
}
