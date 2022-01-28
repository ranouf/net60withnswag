using ApiWithAuthentication.Servers.API.Controllers.Dtos;
using ApiWithAuthentication.Servers.API.Controllers.Dtos.Entities;
using Newtonsoft.Json;
using SK.Extensions;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace ApiWithAuthentication.Tests.Extensions
{
    public enum Format
    {
        Json = 1,
        FormData = 2,
    }
    public static class HttpExtensions
    {
        public static async Task<T> ConvertToAsync<T>(this HttpResponseMessage response, ITestOutputHelper output)
        {
            var content = await response.Content.ReadAsStringAsync();
            output.WriteLine($"CONVERT TO '{typeof(T).GetName()}' - {content}");
            return JsonConvert.DeserializeObject<T>(content);
        }

        public static Task<string> ConvertToStringAsync(this HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync();
        }

        public static async Task<HttpResponseMessage> GetByIdAsync<TPrimaryKey>(
           this HttpClient client,
           string path,
           ITestOutputHelper output,
           TPrimaryKey id,
           IDto dto = null
        )
        {
            var builder = BuildPathWithId(client, path, id);
            output.WriteLine($"METHOD GET, url:'{builder.Uri.PathAndQuery}'");
            return await client.GetAsync(builder.Path, output, dto);
        }

        public static async Task<HttpResponseMessage> PostByIdAsync<TPrimaryKey>(
           this HttpClient client,
           string path,
           ITestOutputHelper output,
           TPrimaryKey id,
           IDto dto = null
        )
        {
            var builder = BuildPathWithId(client, path, id);
            output.WriteLine($"METHOD POST, url:'{builder.Uri.PathAndQuery}'");
            return await client.PostAsync(builder.Path, output, dto);
        }

        public static async Task<HttpResponseMessage> GetAsync(
            this HttpClient client,
            string path,
            ITestOutputHelper output,
            IDto dto = null
        )
        {
            var builder = BuildPath(client, path);
            if (dto != null)
            {
                builder.Query = dto.ToQueryString();
            }
            output.WriteLine($"METHOD GET, url:'{builder.Uri.PathAndQuery}'");
            return await client.GetAsync(builder.Uri.PathAndQuery);
        }

        public static async Task<HttpResponseMessage> PostAsync(
            this HttpClient client,
            string path,
            ITestOutputHelper output,
            IDto dto
        )
        {
            var builder = BuildPath(client, path);
            output.WriteLine($"METHOD POST, url:'{builder.Uri.PathAndQuery}' dto:'{dto.ToJson()}'");
            return await client.PostAsync(builder.Uri.PathAndQuery, dto.ToStringContent());
        }

        public static async Task<HttpResponseMessage> PutByIdAsync<TPrimaryKey>(
            this HttpClient client,
            string path,
            ITestOutputHelper output,
            TPrimaryKey id,
            IDto dto
        )
        {
            var builder = BuildPathWithEntity(client, path, id);
            output.WriteLine($"METHOD PUT, url:'{builder.Uri.PathAndQuery}' dto:'{dto.ToJson()}'");
            return await client.PutAsync(builder.Uri.PathAndQuery, dto.ToStringContent());
        }

        public static async Task<HttpResponseMessage> PutByIdAsync<TPrimaryKey>(
            this HttpClient client,
            string path,
            ITestOutputHelper output,
            IEntityDto<TPrimaryKey> dto
        )
        {
            var builder = BuildPathWithEntity(client, path, dto);
            output.WriteLine($"METHOD PUT, url:'{builder.Uri.PathAndQuery}' dto:'{dto.ToJson()}'");
            return await client.PutAsync(builder.Uri.PathAndQuery, dto.ToStringContent());
        }

        public static async Task<HttpResponseMessage> PutAsync(
            this HttpClient client,
            string path,
            ITestOutputHelper output
        )
        {
            var builder = BuildPath(client, path);
            output.WriteLine($"METHOD PUT, url:'{builder.Uri.PathAndQuery}'");
            return await client.PutAsync(builder.Uri.PathAndQuery, null);
        }

        public static async Task<HttpResponseMessage> PutAsync<T>(
            this HttpClient client,
            string path,
            ITestOutputHelper output,
            T dto
        )
        {
            var builder = BuildPath(client, path);
            output.WriteLine($"METHOD PUT, url:'{builder.Uri.PathAndQuery}' dto:'{dto.ToJson()}'");
            return await client.PutAsync(builder.Uri.PathAndQuery, dto.ToStringContent());
        }

        public static async Task<HttpResponseMessage> PutAsync<T>(
            this HttpClient client,
            string path,
            ITestOutputHelper output,
            T dto,
            Format format = Format.Json
        )
        {
            var builder = BuildPath(client, path);
            output.WriteLine($"METHOD PUT, url:'{builder.Uri.PathAndQuery}' dto:'{dto.ToJson()}'");
            if (format == Format.Json)
            {
                return  await client.PutAsync(builder.Uri.PathAndQuery, dto.ToStringContent());
            }
            return await client.PutAsync(builder.Uri.PathAndQuery, dto.ToFormData());
        }

        public static async Task<HttpResponseMessage> DeleteAsync<TPrimaryKey>(
            this HttpClient client,
            string path,
            ITestOutputHelper output,
            IEntityDto<TPrimaryKey> dto
        )
        {
            var builder = BuildPathWithEntity(client, path, dto);
            output.WriteLine($"METHOD DELETE, url:'{builder.Uri.PathAndQuery}'");
            return await client.DeleteAsync(builder.Uri.PathAndQuery);
        }

        public static async Task<HttpResponseMessage> DeleteByIdAsync<TPrimaryKey>(
            this HttpClient client,
            string path,
            ITestOutputHelper output,
            TPrimaryKey id
        )
        {
            var builder = BuildPathWithEntity(client, path, id);
            output.WriteLine($"METHOD DELETE, url:'{builder.Uri.PathAndQuery}'");
            return await client.DeleteAsync(builder.Uri.PathAndQuery);
        }

        public static async Task<HttpResponseMessage> DeleteAsync(
            this HttpClient client,
            ITestOutputHelper output,
            string path
        )
        {
            var builder = BuildPath(client, path);
            output.WriteLine($"METHOD DELETE, url:'{builder.Uri.PathAndQuery}'");
            return await client.DeleteAsync(builder.Uri.PathAndQuery);
        }

        #region Private

        private static UriBuilder BuildPathWithEntity<TPrimaryKey>(HttpClient client, string path, TPrimaryKey id)
        {
            return BuildPathWithId(client, path, id);
        }

        private static UriBuilder BuildPathWithEntity<TPrimaryKey>(HttpClient client, string path, IEntityDto<TPrimaryKey> dto)
        {
            return BuildPathWithId(client, path, dto.Id);
        }

        private static UriBuilder BuildPathWithId<TPrimaryKey>(HttpClient client, string path, TPrimaryKey id)
        {
            var match = Regex.Match(path, "{id([^}]+)}", RegexOptions.IgnoreCase); // ex: /api/v1/user/{id:guid}/lock => {id:guid}

            return match.Success
                ? BuildPath(client, path.Replace(match.Value, id.ToString()))
                : BuildPath(client, $"{path}/{id}");
        }

        private static UriBuilder BuildPath(HttpClient client, string path)
        {
            return new UriBuilder(client.BaseAddress)
            {
                Path = path
            };
        }
        #endregion
    }
}
