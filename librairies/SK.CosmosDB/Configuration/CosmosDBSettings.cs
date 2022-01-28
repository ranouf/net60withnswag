using System;
using System.ComponentModel.DataAnnotations;

namespace SK.CosmosDB.Configuration
{
    public class CosmosDBSettings
    {
        [Required]
        public Uri Endpoint { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public string DatabaseId { get; set; }
        public int MaxConnectionLimit { get; set; }
        public int Throughput { get; set; } = 1000;
        public int MaxRetry { get; set; } = 5;

    }
}
