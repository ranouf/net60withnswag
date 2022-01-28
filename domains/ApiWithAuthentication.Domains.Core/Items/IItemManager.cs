using SK.Paging;
using ApiWithAuthentication.Domains.Core.Items.Entities;
using System;
using System.Threading.Tasks;

namespace ApiWithAuthentication.Domains.Core.Items
{
    public interface IItemManager
    {
        Task<Item> CreateItemAsync(Item item);
        Task DeleteItemAsync(Item item);
        Task<Item> FindByIdAsync(Guid id);
        Task<PagedResult<Item>> GetItems(string filter, int? maxResultCount, int? SkipCount);
        Task<Item> UpdateItemAsync(Item item);
    }
}