using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vostok.Sample.ImageStore.Client;

namespace Vostok.Sample.ThumbnailGenerator.Controllers
{
    // todo (spaceorc 17.10.2017) написать клиент
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
            // todo (spaceorc 17.10.2017) написать генерацию thumbnail-ов
            return await imageStoreClient.DownloadAsync(name).ConfigureAwait(false);
        }
    }
}