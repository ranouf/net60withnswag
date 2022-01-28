using SK.Storage.Configuration;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Threading.Tasks;

namespace SK.Storage
{
    public class AzureStorageService : IStorageService
    {
        private readonly AzureStorageSettings _azureSettings;

        public AzureStorageService(IOptions<AzureStorageSettings> azureSettings)
        {
            _azureSettings = azureSettings.Value;
        }

        public async Task<string> UploadAsync(Stream stream, string fileName)
        {
            var blockBlobClient = await GetBlobClientAsync(fileName, createIfNotExists: true);
            await blockBlobClient.UploadAsync(stream);
            return blockBlobClient.Uri.AbsoluteUri;
        }

        public async Task<Stream> DownloadAsync(string fileName)
        {
            var blockBlobClient = await GetBlobClientAsync(fileName);
            Stream fileStream = new MemoryStream();
            await blockBlobClient.DownloadToAsync(fileStream);
            return fileStream;
        }

        public async Task<bool> RemoveAsync(string fileName)
        {
            var blockBlobClient = await GetBlobClientAsync(fileName);
            var response = await blockBlobClient.DeleteIfExistsAsync();
            return response.Value;

        }

        #region Private
        private async Task<BlobClient> GetBlobClientAsync(string fileName, bool createIfNotExists = false)
        {
            var blobServiceClient = new BlobServiceClient(_azureSettings.ConnectionString);
            var container = blobServiceClient.GetBlobContainerClient(
                _azureSettings.Container
            );
            if (createIfNotExists)
            {
                try
                {
                    await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
                }
                catch (System.Exception e)
                {
                    throw;
                }
            }
            var result = container.GetBlobClient(fileName);
            return result;

        }
        #endregion
    }
}
