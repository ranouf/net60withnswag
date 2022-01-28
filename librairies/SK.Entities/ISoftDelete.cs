using System;

namespace SK.Entities
{
    public interface ISoftDelete : IDeleteAudited
    {
        bool IsDeleted { get; set; }
    }
    public interface ISoftDelete<TUser> : ISoftDelete, IDeleteAudited<TUser>
    {
    }
}
