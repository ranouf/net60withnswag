using ApiWithAuthentication.Servers.API.Controllers.Dtos.Entities;

namespace ApiWithAuthentication.Servers.API.Controllers.Items.Dtos
{
    public class ItemDto : EntityDto, IEntityDto
    {
        public string Name { get; set; }
    }
}
