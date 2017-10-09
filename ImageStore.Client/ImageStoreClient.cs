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
            cluster = new ClusterClient(log, config =>
            {
                config.ClusterProvider = new FixedClusterProvider(host);
                config.Transport = new VostokHttpTransport(log);
            });
        }

        public async Task<byte[]> DownloadAsync(string name)
        {
            var request = Request.Get($"Image/{name}");
            var result = await cluster.SendAsync(request).ConfigureAwait(false);
            return result.Response.Content.ToArray();
        }
    }
}
