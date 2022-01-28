namespace SK.Smtp.SmtpClients
{
    public interface ISmtpClientFactory
    {
        ISmtpClient CreateSmtpClient();
    }
}