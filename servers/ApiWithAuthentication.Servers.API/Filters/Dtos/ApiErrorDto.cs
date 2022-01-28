using Newtonsoft.Json;

namespace ApiWithAuthentication.Servers.API.Filters.Dtos
{
    public class ApiErrorDto
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "isError")]
        public bool IsError { get; set; }
        [JsonProperty(PropertyName = "details")]
        public string Details { get; set; }

        public ApiErrorDto(string message, string details = "")
        {
            Message = message;
            IsError = true;
            Details = details;
        }
    }
}
