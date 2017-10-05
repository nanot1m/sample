using System.Threading.Tasks;

namespace Vostok.ImageStore.Controllers
{
    public interface IImagesRepository
    {
        Task<byte[]> DownloadAsync(string name);

        Task UploadAsync(string name, byte[] content);

        Task<string[]> SearchByName(string name);
    }
}