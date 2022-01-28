using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace SK.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<T> ConvertToAsync<T>(this HttpResponseMessage response) where T : class
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public static Task<string> ConvertToStringAsync(this HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync();
        }
    }
}
