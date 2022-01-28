using System;

namespace SK.Entities
{
    public interface IAudited<TUser> : IAudited<Guid?, TUser>, ICreationAudited<TUser>, IUpdateAudited<TUser>
    {
    }

    public interface IAudited<TPrimaryKey, TUser> : ICreationAudited<TPrimaryKey, TUser>, IUpdateAudited<TPrimaryKey, TUser>
    {
    }

    public interface IAuditedAt : ICreationAudited, IUpdateAudited
    {
    }
}
