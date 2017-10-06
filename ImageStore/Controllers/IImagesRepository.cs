using System.Threading.Tasks;

namespace Vostok.Sample.ImageStore.Controllers
{
    public interface IImagesRepository
    {
        Task<byte[]> DownloadAsync(string name);

        Task UploadAsync(string name, byte[] content);

        Task<string[]> SearchByName(string name);

        Task RemoveAsync(string name);
    }
}