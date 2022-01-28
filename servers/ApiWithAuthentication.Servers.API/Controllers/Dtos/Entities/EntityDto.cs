using System;

namespace ApiWithAuthentication.Servers.API.Controllers.Dtos.Entities
{
    public class EntityDto : EntityDto<Guid>
    {
    }

    public class EntityDto<T> : IEntityDto<T>
    {
        public T Id { get; set; }
    }
}
