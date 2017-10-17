using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using Vostok.Sample.ImageStore.Client;

namespace Vostok.Sample.ImageFilter.Controllers
{
    [Route("ImageFilter")]
    public class ImageFilterController : Controller
    {
        private readonly ImageStoreClient imageStoreClient;

        public ImageFilterController(ImageStoreClient imageStoreClient)
        {
            this.imageStoreClient = imageStoreClient;
        }

        [HttpPost("{id}/BlackWhite")]
        public async Task<ActionResult> ApplyBlackWhite(string id)
        {
            return await ApplyFilter(id, x => x.BlackWhite()).ConfigureAwait(false);
        }

        [HttpPost("{id}/Contrast/{*amount}")]
        public async Task<ActionResult> ApplyContrast(int amount, string id)
        {
            return await ApplyFilter(id, x => x.Contrast(amount)).ConfigureAwait(false);
        }

        [HttpPost("{id}/Invert")]
        public async Task<ActionResult> ApplyInvert(string id)
        {
            return await ApplyFilter(id, x => x.Invert()).ConfigureAwait(false);
        }

        [HttpPost("{id}/Brightness/{*amount}")]
        public async Task<ActionResult> ApplyBrightness(int amount, string id)
        {
            return await ApplyFilter(id, x => x.Brightness(amount)).ConfigureAwait(false);
        }

        [HttpPost("{id}/Pixelate/{*size}")]
        public async Task<ActionResult> ApplyPixelate(int size, string id)
        {
            return await ApplyFilter(id, x => x.Pixelate(size)).ConfigureAwait(false);
        }

        [HttpPost("{id}/OilPaint")]
        public async Task<ActionResult> ApplyOilPaint(string id)
        {
            return await ApplyFilter(id, x => x.OilPaint()).ConfigureAwait(false);
        }

        [HttpPost("{id}/Vignette")]
        public async Task<ActionResult> ApplyVignette(string id)
        {
            return await ApplyFilter(id, x => x.Vignette()).ConfigureAwait(false);
        }

        [HttpPost("{id}/Glow")]
        public async Task<ActionResult> ApplyGlow(string id)
        {
            return await ApplyFilter(id, x => x.Glow()).ConfigureAwait(false);
        }

        private async Task<ActionResult> ApplyFilter(string sourceId, Action<IImageProcessingContext<Rgba32>> filter)
        {
            var source = await imageStoreClient.DownloadAsync(sourceId).ConfigureAwait(false);
            if (source == null)
                return NotFound();

            using (var image = Image.Load(source))
            {
                image.Mutate(filter);
                var outputStream = new MemoryStream();
                image.SaveAsJpeg(outputStream);
                var resultId = await imageStoreClient.UploadAsync(outputStream.ToArray()).ConfigureAwait(false);
                return Ok(resultId);
            }
        }
    }
}