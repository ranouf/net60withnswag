using ApiWithAuthentication.Domains.Core.Identity.Entities;
using SK.Paging;
using System;
using System.Threading.Tasks;

namespace ApiWithAuthentication.Domains.Core.Identity
{
    public interface IUserManager
    {
        Task AllowUserToLoginAsync(User user, bool allow);
        Task<User> FindByIdAsync(Guid id, bool includeDeleted = false);
        Task<User> FindByEmailAsync(string email, bool includeDeleted = false);
        Task<PagedResult<User>> GetAllAsync(string filter, int? maxResultCount, int? SkipCount);
        Task<User> InviteAsync(User user, Role role);
        Task<User> CreateAsync(User user, string password, Role role);
        Task<User> RegisterAsync(User user, string password);
        Task DeleteAsync(User user);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task ChangePasswordAsync(User user, string currentPassword, string newPassword);
        Task<User> UpdateAsync(User user);
        Task PasswordForgottenAsync(User user);
        Task ResetPasswordAsync(User user, string token, string newPassword);
        Task<bool> CanSignInAsync(User user);
        Task ConfirmRegistrationEmailAsync(User user, string token);
        Task<User> ConfirmInvitationEmailAsync(User user, string password, string token);
        Task<User> ReSendEmailConfirmationAsync(User user);
    }
}