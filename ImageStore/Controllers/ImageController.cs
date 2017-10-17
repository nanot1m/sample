using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vostok.Sample.ImageStore.Services;

namespace Vostok.Sample.ImageStore.Controllers
{
    [Route("Image")]
    public class ImageController : Controller
    {
        private readonly IImagesRepository imagesRepository;

        public ImageController(IImagesRepository imagesRepository)
        {
            this.imagesRepository = imagesRepository;
        }

        [HttpGet("{*id}")]
        public async Task<ActionResult> DownloadAsync(string id)
        {
            var bytes = await imagesRepository.DownloadAsync(id).ConfigureAwait(false);
            if (bytes == null)
                return NotFound();
            return File(bytes, MediaTypeNames.Application.Octet);
        }

        [HttpPut]
        public async Task<ActionResult> UploadAsync()
        {
            using (var memoryStream = new MemoryStream())
            {
                await Request.Body.CopyToAsync(memoryStream).ConfigureAwait(false);
                return Content(await imagesRepository.UploadAsync(memoryStream.ToArray()).ConfigureAwait(false));
            }
        }

        [HttpDelete("{*id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (await imagesRepository.RemoveAsync(id).ConfigureAwait(false))
                return Ok();
            return NotFound();
        }
    }
}