using SK.Paging;
using ApiWithAuthentication.Servers.API.Controllers.Dtos.Paging;
using AutoMapper;

namespace ApiWithAuthentication.Servers.API.Controllers.Dtos
{
    public class DtosProfile : Profile
    {
        public DtosProfile()
        {
            CreateMap(typeof(PagedResult<>), typeof(PagedResultDto<>));
            CreateMap(typeof(PagedResult<,>), typeof(PagedResultDto<,>));
            CreateMap(typeof(PagedResult<>), typeof(PagedResultDto<,>));
        }
    }
}
