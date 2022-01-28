using ApiWithAuthentication.Domains.Core.Items.Entities;
using ApiWithAuthentication.Servers.API.Controllers.Items.Dtos;
using AutoMapper;

namespace ApiWithAuthentication.Servers.API.Controllers.Dtos
{
    public class ItemsProfile : Profile
    {
        public ItemsProfile()
        {
            CreateMap<Item, ItemDto>();
        }
    }
}
