using System;

namespace SK.Entities
{
    public interface IDeleteAudited<TUser> : IDeleteAudited<Guid?, TUser>
    {
    }

    public interface IDeleteAudited<TPrimaryKey, TUser> : IDeleteAudited
    {
        TPrimaryKey DeletedByUserId { get; set; }
        TUser DeletedByUser { get; set; }
    }

    public interface IDeleteAudited
    {
        DateTimeOffset? DeletedAt { get; set; }
    }
}
