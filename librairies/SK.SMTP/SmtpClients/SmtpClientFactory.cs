namespace SK.Smtp.SmtpClients
{
    public class SmtpClientFactory : ISmtpClientFactory
    {
        public ISmtpClient CreateSmtpClient()
        {
            return new SmtpClient();
        }
    }
}
