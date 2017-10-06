using System;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace Vostok.Sample.ImageFilter.Controllers
{
    [Route("ImageFilter")]
    public class ImageFilterController : Controller
    {
        [HttpPost("BlackWhite")]
        public byte[] ApplyBlackWhite([FromBody] byte[] source)
        {
            return ApplyFilter(source, x => x.BlackWhite());
        }

        [HttpPost("Contrast/{*amount}")]
        public byte[] ApplyContrast(int amount, [FromBody] byte[] source)
        {
            return ApplyFilter(source, x => x.Contrast(amount));
        }

        [HttpPost("Invert")]
        public byte[] ApplyInvert([FromBody] byte[] source)
        {
            return ApplyFilter(source, x => x.Invert());
        }

        [HttpPost("Brightness/{*amount}")]
        public byte[] ApplyBrightness(int amount, [FromBody] byte[] source)
        {
            return ApplyFilter(source, x => x.Brightness(amount));
        }

        [HttpPost("Pixelate/{*size}")]
        public byte[] ApplyPixelate(int size, [FromBody] byte[] source)
        {
            return ApplyFilter(source, x => x.Pixelate(size));
        }

        [HttpPost("OilPaint")]
        public byte[] ApplyOilPaint([FromBody] byte[] source)
        {
            return ApplyFilter(source, x => x.OilPaint());
        }

        [HttpPost("Vignette")]
        public byte[] ApplyVignette([FromBody] byte[] source)
        {
            return ApplyFilter(source, x => x.Vignette());
        }

        [HttpPost("Glow")]
        public byte[] ApplyGlow([FromBody] byte[] source)
        {
            return ApplyFilter(source, x => x.Glow());
        }

        private static byte[] ApplyFilter(byte[] source, Action<IImageProcessingContext<Rgba32>> filter)
        {
            using (var image = Image.Load(source))
            {
                image.Mutate(filter);
                return image.SavePixelData();
            }
        }
    }
}