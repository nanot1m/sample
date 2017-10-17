using System.Threading.Tasks;

namespace Vostok.Sample.ImageStore.Services
{
    public interface IImagesRepository
    {
        Task<byte[]> DownloadAsync(string id);

        Task<string> UploadAsync(byte[] content);

        Task<bool> RemoveAsync(string id);
    }
}