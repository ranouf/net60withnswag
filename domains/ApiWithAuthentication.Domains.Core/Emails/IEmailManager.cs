using ApiWithAuthentication.Domains.Core.Identity.Entities;
using System.Threading.Tasks;

namespace ApiWithAuthentication.Domains.Core.Emails
{
    public interface IEmailManager
    {
        Task SendPasswordForgottenEmailAsync(User user, string token);
        Task SendInviteUserEmailAsync(User user, string token);
        Task SendConfirmEmailAsync(User user, string token);
    }
}