using ApiWithAuthentication.Librairies.Common;
using ApiWithAuthentication.Servers.API.Controllers.Dtos.Paging;
using ApiWithAuthentication.Servers.API.Controllers.Items.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        public ItemController(
            IMapper mapper,
            ILogger<ItemController> logger
        ) : base(mapper, logger)
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof(ItemDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetItemByIdAsync([FromRoute] Guid id)
        {
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<ItemDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetItemsAsync([FromQuery] PagedRequestDto dto)
        {
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(ItemDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateItemAsync([FromBody] UpsertItemRequest dto)
        {
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(typeof(ItemDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateItemAsync([FromRoute] Guid id, [FromBody] UpsertItemRequest dto)
        {
            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteItemAsync([FromRoute] Guid id)
        {
            return Ok();
        }
    }
}