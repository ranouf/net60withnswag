using System;

namespace SK.Entities
{
    public interface ICreationAudited<TUser> : ICreationAudited<Guid?, TUser>
    {
    }

    public interface ICreationAudited<TPrimaryKey, TUser> : ICreationAudited
    {
        TPrimaryKey CreatedByUserId { get; set; }
        TUser CreatedByUser { get; set; }
    }

    public interface ICreationAudited
    {
        DateTimeOffset? CreatedAt { get; set; }
    }
}
