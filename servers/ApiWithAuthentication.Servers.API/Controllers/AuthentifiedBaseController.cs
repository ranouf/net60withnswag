
using SK.Session;
using ApiWithAuthentication.Domains.Core.Identity;
using ApiWithAuthentication.Domains.Core.Identity.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ApiWithAuthentication.Servers.API.Controllers
{
    [ApiController]
    public abstract class AuthentifiedBaseController : BaseController
    {
        internal readonly IUserManager _userManager;
        private User _currentUser;
        internal IUserSession Session { get; set; }
        internal async Task<User> GetCurrentUserAsync()
        {
            if (_currentUser == null && Session.UserId.HasValue)
            {
                _currentUser = await _userManager.FindByIdAsync(Session.UserId.Value);
            }
            return _currentUser;
        }

        public AuthentifiedBaseController(
            IUserSession session,
            IUserManager userManager,
            IMapper mapper,
            ILogger logger
        ) : base(mapper, logger)
        {
            _userManager = userManager;
            Session = session;
        }
    }
}
