using SK.Paging;
using ApiWithAuthentication.Servers.API.Controllers.Dtos.Paging;
using AutoMapper;

namespace ApiWithAuthentication.Servers.API.Controllers
{
    public class ControllerProfile : Profile
    {
        public ControllerProfile()
        {
            CreateMap(typeof(PagedResult<>), typeof(PagedResultDto<>));
        }
    }
}
