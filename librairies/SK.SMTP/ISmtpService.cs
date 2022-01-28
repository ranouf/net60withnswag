using System.Threading.Tasks;

namespace SK.Smtp
{
    public interface ISmtpService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}