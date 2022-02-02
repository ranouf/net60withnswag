using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiWithAuthentication.Servers.API.Controllers
{
    [ApiController]
    public abstract class AuthentifiedBaseController : BaseController
    {
        public AuthentifiedBaseController(
            IMapper mapper,
            ILogger logger
        ) : base(mapper, logger)
        {
        }
    }
}
