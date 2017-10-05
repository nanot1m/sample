using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace bluewater.Controllers
{
    [Route("Thumbs")]
    public class ThumbsController
    {
        [HttpPost("Generate")]
        public async Task<byte[]> Generate(byte[] source)
        {
            return source;
        }
    }
}