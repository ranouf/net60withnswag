using System.IO;
using System.Threading.Tasks;

namespace SK.Storage
{
    public interface IStorageService
    {
        /// <summary>
        /// Upload file to Storage
        /// </summary>
        /// <param name="stream">Stream to upload</param>
        /// <param name="fileName">File name</param>
        /// <returns>String containing Absolute Uri</returns>
        Task<string> UploadAsync(Stream stream, string fileName);

        /// <summary>
        /// Download file from Storage
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Stream containing downloaded file</returns>
        Task<Stream> DownloadAsync(string fileName);
        
        /// <summary>
        /// Remove file from Storage
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(string fileName);
    }
}
