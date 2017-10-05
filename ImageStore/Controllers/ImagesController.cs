using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Vostok.ImageStore.Controllers
{
    [Route("Images")]
    public class ImagesController : Controller
    {
        private readonly IImagesRepository imagesRepository;

        public ImagesController(IImagesRepository imagesRepository)
        {
            this.imagesRepository = imagesRepository;
        }

        [HttpGet("{*name}")]
        public async Task<ImageContent> DownloadAsync(string name)
        {
            var content = await imagesRepository.DownloadAsync(name).ConfigureAwait(false);
            return new ImageContent {Content = content};
        }

        [HttpPut("{*name}")]
        public async Task<Image> UploadAsync(string name, [FromBody] ImageContent image)
        {
            await imagesRepository.UploadAsync(name, image.Content).ConfigureAwait(false);
            return new Image {Name = name};
        }

        [HttpGet]
        public async Task<Image[]> SearchAsync(string name)
        {
            var names = await imagesRepository.SearchByName(name).ConfigureAwait(false);
            return names.Select(x => new Image {Name = x}).ToArray();
        }
    }
}