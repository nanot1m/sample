using System;
using System.Threading.Tasks;
using Vostok.Clusterclient;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Topology;
using Vostok.Clusterclient.Transport.Http;
using Vostok.Logging;

namespace Vostok.Sample.ImageFilter.Client
{
    public class ImageFilterClient
    {
        private readonly ClusterClient cluster;

        public ImageFilterClient(ILog log, Uri host)
        {
            cluster = new ClusterClient(
                log,
                config =>
                {
                    config.ClusterProvider = new FixedClusterProvider(host);
                    config.Transport = new VostokHttpTransport(log);
                });
        }
        
        public async Task<string> ApplyBlackWhiteAsync(string id)
        {
            return await MakeRequest(Request.Post($"ImageFilter/{id}/BlackWhite"));
        }
        
        public async Task<string> ApplyContrastAsync(string id, int amount)
        {
            if (amount < -100 || amount > 100)
                throw new ArgumentOutOfRangeException("Contrast amount must be between -100 and 100.");
            return await MakeRequest(Request.Post($"ImageFilter/{id}/Contrast/{amount}"));
        }
        
        public async Task<string> ApplyInvertAsync(string id)
        {
            return await MakeRequest(Request.Post($"ImageFilter/{id}/Invert"));
        }
        
        public async Task<string> ApplyBrightnessAsync(string id, int amount)
        {
            if (amount < -100 || amount > 100)
                throw new ArgumentOutOfRangeException("Brightness amount must be between -100 and 100.");
            return await MakeRequest(Request.Post($"ImageFilter/{id}/Brightness/{amount}"));
        }
        
        public async Task<string> ApplyPixelateAsync(string id, int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException("Pixel size must be greater than zero.");
            return await MakeRequest(Request.Post($"ImageFilter/{id}/Pixelate/{size}"));
        }
        
        public async Task<string> ApplyOilPaintAsync(string id)
        {
            return await MakeRequest(Request.Post($"ImageFilter/{id}/OilPaint"));
        }
        
        public async Task<string> ApplyVignetteAsync(string id)
        {
            return await MakeRequest(Request.Post($"ImageFilter/{id}/Vignette"));
        }
        
        public async Task<string> ApplyGlowAsync(string id)
        {
            return await MakeRequest(Request.Post($"ImageFilter/{id}/Glow"));
        }
        
        public async Task<string> ApplyResizeAsync(string id, int width, int height)
        {
            if (width < 0 || height < 0 || width + height == 0)
                throw new ArgumentOutOfRangeException("Width and height must be greater than zero. One of them can be zero (to preserve aspect ratio), but not both.");
            return await MakeRequest(Request.Post($"ImageFilter/{id}/Resize/{width}x{height}"));
        }

        private async Task<string> MakeRequest(Request request)
        {
            var result = await cluster.SendAsync(request).ConfigureAwait(false);
            if (result.Response.Code == ResponseCode.NotFound)
                return null;
            result.Response.EnsureSuccessStatusCode();
            return result.Response.Content.ToString();
        }
    }
}