using ApiWithAuthentication.Domains.Core.Emails;
using ApiWithAuthentication.Domains.Core.Identity.Configuration;
using ApiWithAuthentication.Domains.Core.Identity.Entities;
using ApiWithAuthentication.Domains.Core.Identity.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SK.EntityFramework.Repositories;
using SK.EntityFramework.UnitOfWork;
using SK.Events;
using SK.Exceptions;
using SK.Extensions;
using SK.Paging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ApiWithAuthentication.Domains.Core.Identity
{
    public class UserManager : IUserManager
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IRoleManager _roleManager;
        private readonly IEmailManager _emailManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User> _userRepository;
        private readonly IDomainEvents _domainEvents;
        private readonly IdentitySettings _identitySettings;
        private readonly ILogger<UserManager> _logger;

        public UserManager(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IRoleManager roleManager,
            IEmailManager emailManager,
            IUnitOfWork unitOfWork,
            IDomainEvents domainEvents,
            IOptions<IdentitySettings> identitySettings,
            ILogger<UserManager> logger
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailManager = emailManager;
            _unitOfWork = unitOfWork;
            _userRepository = _unitOfWork.GetRepository<User>();
            _domainEvents = domainEvents;
            _identitySettings = identitySettings.Value;
            _logger = logger;
        }

        public async Task<bool> CanSignInAsync(User user)
        {
            return await _signInManager.CanSignInAsync(user);
        }

        public async Task<User> FindByIdAsync(Guid id, bool includeDeleted = false)
        {
            var result = await FindByAsync(u => u.Id == id, includeDeleted);
            if (result != null)
            {
                _logger.LogInformation($"User found: {result.ToJson()}");
            }
            else
            {
                _logger.LogInformation($"User not found with id: {id}");
            }
            return result;
        }

        public async Task<User> FindByEmailAsync(string email, bool includeDeleted = false)
        {
            var result = await FindByAsync(u => u.Email == email, includeDeleted);
            if (result != null)
            {
                _logger.LogInformation($"User found: {result.ToJson()}");
            }
            else
            {
                _logger.LogInformation($"User not found with email: {email}");
            }
            return result;
        }

        public Task<PagedResult<User>> GetAllAsync(string filter, int? maxResultCount, int? SkipCount)
        {
            var query = _userRepository.GetAll()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Select(u => u);

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(u => u.FullName.Contains(filter) || u.Email.Contains(filter));
            }

            query = query.OrderBy(o => o.FullName);

            return query.ToPagedResultAsync(maxResultCount, SkipCount);
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            var role = await _roleManager.FindByNameAsync(Constants.Roles.User);
            var result = await CreateAsync(user, password, role);
            await SendEmailConfirmationAsync(result);

            await _domainEvents.RaiseAsync(
                new UserRegisteredEvent { User = result }
            );
            return result;
        }

        public async Task<User> ReSendEmailConfirmationAsync(User user)
        {
            var result = await SendEmailConfirmationAsync(user);

            await _domainEvents.RaiseAsync(
                new UserResendEmailConfirmationEvent { User = result }
            );
            return result;
        }

        public async Task<User> CreateAsync(User user, string password, Role role)
        {
            var identityResult = await _userManager.CreateAsync(user, password);
            if (!identityResult.Succeeded)
            {
                throw new LocalException(identityResult.Errors.First().Description);
            }

            await AddToRoleAsync(user, role);

            return await FindByEmailAsync(user.Email);
        }

        public async Task DeleteAsync(User user)
        {
            var identityResult = await _userManager.DeleteAsync(user);

            if (!identityResult.Succeeded)
            {
                throw new LocalException(identityResult.Errors.First().Description);
            }

            var userToDelete = await FindByIdAsync(user.Id, includeDeleted: true);

            await _domainEvents.RaiseAsync(
                new UserDeletedEvent { User = userToDelete }
            );
        }

        public async Task AllowUserToLoginAsync(User user, bool allow)
        {
            var identityResult = await _userManager.SetLockoutEnabledAsync(user, !allow);
            if (!identityResult.Succeeded)
            {
                throw new LocalException(identityResult.Errors.First().Description);
            }

            IEvent @event;

            if (allow)
            {
                user = await FindByIdAsync(user.Id);
                @event = new UserUnlockedEvent { User = user };
            }
            else
            {
                identityResult = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                if (!identityResult.Succeeded)
                {
                    throw new LocalException(identityResult.Errors.First().Description);
                }

                user = await FindByIdAsync(user.Id);
                @event = new UserLockedEvent { User = user };
            }
            await _domainEvents.RaiseAsync(@event);
        }

        public Task<bool> CheckPasswordAsync(User user, string password)
        {
            return _userManager.CheckPasswordAsync(user, password);
        }

        public async Task ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            var identityResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!identityResult.Succeeded)
            {
                throw new LocalException(identityResult.Errors.First().Description);
            }
            user.GenerateNewSecurityStamp();
            await UpdateAsync(user);
        }

        public async Task<User> UpdateAsync(User user)
        {
            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
            {
                throw new LocalException(identityResult.Errors.First().Description);
            }

            var result = await FindByIdAsync(user.Id);
            await _domainEvents.RaiseAsync(
                new UserUpdatedEvent { User = result }
            );

            return result;
        }

        public async Task PasswordForgottenAsync(User user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            _logger.LogInformation($"PasswordResetToken is '{token}'");
            await _emailManager.SendPasswordForgottenEmailAsync(user, token);
        }

        public async Task ResetPasswordAsync(User user, string token, string newPassword)
        {
            var identityResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!identityResult.Succeeded)
            {
                throw new LocalException(identityResult.Errors.First().Description);
            }
        }

        public async Task<User> InviteAsync(User user, Role role)
        {
            var identityResult = await _userManager.CreateAsync(user);
            if (!identityResult.Succeeded)
            {
                throw new LocalException(identityResult.Errors.First().Description);
            }
            var result = await FindByEmailAsync(user.Email);

            await AddToRoleAsync(result, role);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(result);
            _logger.LogInformation($"EmailConfirmationToken is '{token}'");
            await _emailManager.SendInviteUserEmailAsync(result, token);

            await _domainEvents.RaiseAsync(
                new UserInvitedEvent { User = result }
            );
            return result;
        }

        public async Task<User> ConfirmInvitationEmailAsync(User user, string password, string token)
        {
            var identityResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!identityResult.Succeeded)
            {
                throw new LocalException(identityResult.Errors.First().Description);
            }

            identityResult = await _userManager.RemovePasswordAsync(user);
            if (!identityResult.Succeeded)
            {
                throw new LocalException(identityResult.Errors.First().Description);
            }

            identityResult = await _userManager.AddPasswordAsync(user, password);
            if (!identityResult.Succeeded)
            {
                throw new LocalException(identityResult.Errors.First().Description);
            }

            var result = await FindByIdAsync(user.Id);

            await _domainEvents.RaiseAsync(
                new InvitationEmailConfirmedEvent { User = result }
            );

            return result;
        }

        public async Task ConfirmRegistrationEmailAsync(User user, string token)
        {
            var identityResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!identityResult.Succeeded)
            {
                throw new LocalException(identityResult.Errors.First().Description);
            }

            var result = await FindByIdAsync(user.Id);

            await _domainEvents.RaiseAsync(
                new RegistrationEmailConfirmedEvent { User = result }
            );
        }

        #region Private
        private async Task<User> FindByAsync(Expression<Func<User, bool>> where, bool includeDeleted)
        {
            var result = await _userRepository.GetAll()
                .Include(u => u.CreatedByUser)
                .Include(u => u.UpdatedByUser)
                .Include(u => u.DeletedByUser)
                .Include(u => u.InvitedByUser)
                .Include(u => u.UserRoles)
                .ThenInclude(u => u.Role)
                .IgnoreQueryFilters(includeDeleted)
                .FirstOrDefaultAsync(where);
            return result;
        }

        private async Task AddToRoleAsync(User user, Role role)
        {
            var newUser = await _userManager.FindByEmailAsync(user.Email);
            var identityResult = await _userManager.AddToRoleAsync(newUser, role.Name);
            if (!identityResult.Succeeded)
            {
                throw new LocalException(identityResult.Errors.First().Description);
            }
        }

        private async Task<User> SendEmailConfirmationAsync(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _logger.LogInformation($"EmailConfirmationToken is '{token}'");
            await _emailManager.SendConfirmEmailAsync(user, token);
            return user;
        }
        #endregion
    }
}
