using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vostok.ImageStore.Client;

namespace Vostok.ThumbnailGenerator.Controllers
{
    [Route("Thumbnail")]
    public class ThumbnailController : Controller
    {
        private readonly ImageStoreClient imageStoreClient;

        public ThumbnailController(ImageStoreClient imageStoreClient)
        {
            this.imageStoreClient = imageStoreClient;
        }

        [HttpGet("{*name}")]
        public async Task<byte[]> GenerateAsync(string name)
        {
            return await imageStoreClient.DownloadAsync(name).ConfigureAwait(false);
        }
    }
}