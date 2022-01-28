
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiWithAuthentication.Servers.API.Controllers
{
    [ApiController]
    public abstract class BaseController : Controller
    {
        public IMapper Mapper { get; }
        public ILogger Logger { get; }

        public BaseController(IMapper mapper, ILogger logger)
        {
            Mapper = mapper;
            Logger = logger;
        }
    }
}
