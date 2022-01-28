using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SK.EntityFramework.Repositories;
using SK.EntityFramework.UnitOfWork;
using SK.Paging;
using ApiWithAuthentication.Domains.Core.Items.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWithAuthentication.Domains.Core.Items
{
    public class ItemManager : IItemManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Item> _itemRepository;
        private readonly ILogger<ItemManager> _logger;

        public ItemManager(
            IUnitOfWork unitOfWork,
            ILogger<ItemManager> logger
        )
        {
            _unitOfWork = unitOfWork;
            _itemRepository = unitOfWork.GetRepository<Item>();
            _logger = logger;
        }

        public async Task<PagedResult<Item>> GetItems(string filter, int? maxResultCount, int? SkipCount)
        {
            var query = _itemRepository.GetAll();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(e => e.Name.Contains(filter));
            }
            var result = await query.ToPagedResultAsync(maxResultCount, SkipCount);
            _logger.LogInformation($"Got {result.TotalCount} items returned.", result);
            return result;
        }

        public async Task<Item> FindByIdAsync(Guid id)
        {
            var result = await _itemRepository.GetAll()
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();
            if (result == null)
            {
                _logger.LogInformation($"No item found for id:'{id}'");
            }
            else
            {
                _logger.LogInformation($"Item found for id:'{id}'", result);
            }
            return result;
        }

        public async Task<Item> CreateItemAsync(Item item)
        {
            var result = _itemRepository.Insert(item);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Item with id:{result.Id}' has been created.", result);
            return result;
        }

        public async Task<Item> UpdateItemAsync(Item item)
        {
            var result = _itemRepository.Update(item);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Item with id:{result.Id}' has been updated.", result);
            return result;
        }

        public async Task DeleteItemAsync(Item item)
        {
            _itemRepository.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Item with id:{item.Id}' has been deleted.", item);
        }
    }
}
