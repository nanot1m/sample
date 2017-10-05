using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Vostok.ImageStore.Controllers
{
    [Route("Thumbnail")]
    public class ThumbsController : Controller
    {
        [HttpGet("{imageId}")]
        public async Task<byte[]> GetThumbAsync(Guid imageId)
        {
            return Array.Empty<byte>();
        }
    }
}