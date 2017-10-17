using System;
using System.Threading.Tasks;
using Vostok.Clusterclient;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Topology;
using Vostok.Clusterclient.Transport.Http;
using Vostok.Logging;

namespace Vostok.Sample.ImageStore.Client
{
    public class ImageStoreClient
    {
        private readonly ClusterClient cluster;

        public ImageStoreClient(ILog log, Uri host)
        {
            cluster = new ClusterClient(
                log,
                config =>
                {
                    config.ClusterProvider = new FixedClusterProvider(host);
                    config.Transport = new VostokHttpTransport(log);
                });
        }

        public async Task<byte[]> DownloadAsync(string id)
        {
            var request = Request.Get($"Image/{id}");
            var result = await cluster.SendAsync(request).ConfigureAwait(false);
            if (result.Response.Code == ResponseCode.NotFound)
                return null;
            result.Response.EnsureSuccessStatusCode();
            return result.Response.Content.ToArray();
        }

        public async Task<string> UploadAsync(byte[] content)
        {
            var request = Request
                .Put("Image")
                .WithContent(content);
            var result = await cluster.SendAsync(request).ConfigureAwait(false);
            result.Response.EnsureSuccessStatusCode();
            return result.Response.Content.ToString();
        }

        public async Task<bool> RemoveAsync(string id)
        {
            var request = Request.Delete($"Image/{id}");
            var result = await cluster.SendAsync(request).ConfigureAwait(false);
            if (result.Response.Code == ResponseCode.NotFound)
                return false;
            result.Response.EnsureSuccessStatusCode();
            return true;
        }
    }
}