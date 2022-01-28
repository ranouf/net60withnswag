using ApiWithAuthentication.Domains.Core.Identity.Entities;
using SK.Entities;
using System;

namespace ApiWithAuthentication.Domains.Core.Items.Entities
{
    public class Item : Entity, IEntity, IAudited<User>, ISoftDelete<User>
    {
        public string Name { get; private set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public virtual User CreatedByUser { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? UpdatedByUserId { get; set; }
        public virtual User UpdatedByUser { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Guid? DeletedByUserId { get; set; }
        public virtual User DeletedByUser { get; set; }

        public Item(string name)
        {
            Name = name;
        }

        public Item Update(string name)
        {
            if (Name != name) Name = name;
            return this;
        }

    }
}
