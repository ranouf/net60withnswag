using System;

namespace SK.Entities
{
    public interface IUpdateAudited<TUser> : IUpdateAudited<Guid?, TUser>
    {
    }

    public interface IUpdateAudited<TPrimaryKey, TUser> : IUpdateAudited
    {
        TPrimaryKey UpdatedByUserId { get; set; }
        TUser UpdatedByUser { get; set; }
    }

    public interface IUpdateAudited
    {
        DateTimeOffset? UpdatedAt { get; set; }
    }
}
