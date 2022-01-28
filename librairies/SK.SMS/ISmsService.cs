using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace SK.Sms
{
    public interface ISmsService
    {
        void Initialize();
        Task<bool> SendAsync(string body, string to);
        Task<MessageResource> SendAsync(string body, string to, string from = null);
    }
}