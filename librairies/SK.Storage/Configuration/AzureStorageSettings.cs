using System.ComponentModel.DataAnnotations;

namespace SK.Storage.Configuration
{
    public class AzureStorageSettings
    {
        [Required]
        public string ConnectionString { get; set; }
        [Required]
        public string Container { get; set; }
    }
}
