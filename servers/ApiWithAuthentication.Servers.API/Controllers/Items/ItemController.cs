using ApiWithAuthentication.Domains.Core.Identity;
using ApiWithAuthentication.Domains.Core.Items;
using ApiWithAuthentication.Domains.Core.Items.Entities;
using ApiWithAuthentication.Librairies.Common;
using ApiWithAuthentication.Servers.API.Controllers.Dtos.Paging;
using ApiWithAuthentication.Servers.API.Controllers.Items.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SK.Extensions;
using SK.Paging;
using SK.Session;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ApiWithAuthentication.Servers.API.Controllers.Items
{
    [ApiController]
    [Route(Constants.Api.V1.Items.Url)]
    [Authorize]
    public class ItemController : AuthentifiedBaseController
    {
        private readonly IItemManager _itemManager;

        public ItemController(
            IItemManager itemManager,
            IUserSession session,
            IUserManager userManager,
            IMapper mapper,
            ILogger<ItemController> logger
        ) : base(session, userManager, mapper, logger)
        {
            _itemManager = itemManager;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ItemDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetItemByIdAsync([FromRoute] Guid id)
        {
            Logger.LogInformation($"{nameof(GetItemByIdAsync)}, id:{id}.");
            var result = await _itemManager.FindByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return new ObjectResult(Mapper.Map<Item, ItemDto>(result));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<ItemDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetItemsAsync([FromQuery] PagedRequestDto dto)
        {
            Logger.LogInformation($"{nameof(GetItemsAsync)}, dto:{dto.ToJson()}.");
            var result = await _itemManager.GetItems(
                dto.Filter,
                dto.MaxResultCount,
                dto.SkipCount
            );
            return new ObjectResult(Mapper.Map<PagedResult<Item>, PagedResultDto<ItemDto>>(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ItemDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateItemAsync([FromBody] UpsertItemRequest dto)
        {
            Logger.LogInformation($"{nameof(CreateItemAsync)}, dto:{dto.ToJson()}.");
            var result = await _itemManager.CreateItemAsync(
                dto.ToItem()
            );
            return new ObjectResult(Mapper.Map<Item, ItemDto>(result));
        }

        [HttpPut]
        [ProducesResponseType(typeof(ItemDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateItemAsync([FromRoute] Guid id, [FromBody] UpsertItemRequest dto)
        {
            Logger.LogInformation($"{nameof(UpdateItemAsync)}, id:{id}, dto:{dto.ToJson()}.");
            var item = await _itemManager.FindByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            var result = await _itemManager.UpdateItemAsync(
                item.Update(
                    dto.Name
                )
            );
            return new ObjectResult(Mapper.Map<Item, ItemDto>(result));
        }

        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteItemAsync([FromRoute] Guid id)
        {
            Logger.LogInformation($"{nameof(UpdateItemAsync)}, id:{id}.");
            var item = await _itemManager.FindByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            await _itemManager.DeleteItemAsync(
                item
            );
            return Ok();
        }
    }
}