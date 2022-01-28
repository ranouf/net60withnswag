using SK.Entities;
using System;
using System.Collections.Generic;

namespace SK.Paging
{
    public class PagedResult<TEntity> : PagedResult<TEntity, Guid>
        where TEntity : IEntity
    {
        public PagedResult(int totalCount, IList<TEntity> items, bool hasNext)
            : base(totalCount, items, hasNext)
        {

        }
    }

    public class PagedResult<TEntity, TPrimaryKey>
        where TEntity : IEntity<TPrimaryKey>
    {
        public int TotalCount { get; set; } 
        public IList<TEntity> Items { get; set; }
        public bool HasNext { get; set; }

        public PagedResult(int totalCount, IList<TEntity> items, bool hasNext)
        {
            TotalCount = totalCount;
            Items = items;
            HasNext = hasNext;
        }
    }
}
