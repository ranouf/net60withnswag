using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SK.Entities
{
    public interface IEntity : IEntity<Guid>
    {
    }

    public interface IEntity<TPrimaryKey>
    {
        [JsonProperty("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        TPrimaryKey Id { get; set; }
    }
}
