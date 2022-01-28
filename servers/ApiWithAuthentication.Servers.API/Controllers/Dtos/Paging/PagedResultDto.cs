using ApiWithAuthentication.Servers.API.Controllers.Dtos.Entities;
using System;
using System.Collections.Generic;

namespace ApiWithAuthentication.Servers.API.Controllers.Dtos.Paging
{
    public class PagedResultDto<TEntity> : PagedResultDto<TEntity, Guid>
        where TEntity : IEntityDto
    {
    }

    public class PagedResultDto<TEntity, TPrimaryKey>
        where TEntity : IEntityDto<TPrimaryKey>
    {
        public int TotalCount { get; set; }
        public IList<TEntity> Items { get; set; }
        public bool HasNext { get; set; }
    }
}
