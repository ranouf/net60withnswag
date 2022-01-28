using ApiWithAuthentication.Domains.Core.Identity.Entities;
using ApiWithAuthentication.Servers.API.Controllers.Identity.Dtos;
using ApiWithAuthentication.Servers.API.Extensions;
using AutoMapper;

namespace ApiWithAuthentication.Servers.API.Controllers.Identity
{
    public class IdentityProfile : Profile
    {
        public IdentityProfile()
        {
            CreateMap<User, UserDto>()
                .AddFullAuditedBy()
                .ForMember(
                    dest => dest.InvitedBy,
                    opts => opts.MapFrom(src => src.InvitedByUser.FullName)
                );
            CreateMap<Role, RoleDto>();
        }
    }
}
