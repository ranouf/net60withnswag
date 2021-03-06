using ApiWithAuthentication.Domains.Core.Items.Entities;
using ApiWithAuthentication.Servers.API.Controllers.Dtos;
using System.ComponentModel.DataAnnotations;

namespace ApiWithAuthentication.Servers.API.Controllers.Items.Dtos
{
    public class UpsertItemRequest : IDto
    {
        [Required]
        public string Name { get; set; }
        public Item ToItem()
        {
            return new Item(Name);
        }
    }
}
