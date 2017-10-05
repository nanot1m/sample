using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Vostok.ImageStore.Controllers
{
    [Route("Image")]
    public class ImageController : Controller
    {
        private readonly IImagesRepository imagesRepository;

        public ImageController(IImagesRepository imagesRepository)
        {
            this.imagesRepository = imagesRepository;
        }

        [HttpGet("{*name}")]
        public async Task<byte[]> DownloadAsync(string name)
        {
            return await imagesRepository.DownloadAsync(name).ConfigureAwait(false);
        }

        [HttpPut("{*name}")]
        public async Task UploadAsync(string name, [FromBody] byte[] image)
        {
            await imagesRepository.UploadAsync(name, image).ConfigureAwait(false);
        }

        [HttpGet]
        public async Task<string[]> SearchAsync(string name)
        {
            return await imagesRepository.SearchByName(name).ConfigureAwait(false);
        }

        [HttpDelete("{*name}")]
        public async Task DeleteAsync(string name)
        {
            await imagesRepository.DeleteAsync(name).ConfigureAwait(false);
        }
    }
}