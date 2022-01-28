using MimeKit;
using System;
using System.Threading.Tasks;

namespace SK.Smtp.SmtpClients
{
    public interface ISmtpClient : IAsyncDisposable
    {
        bool IsConnected { get; }
        Task ConnectAsync(string host, int port, bool useSsl);
        Task AuthenticateAsync(string username, string password);
        Task SendAsync(MimeMessage message);
    }
}
