using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vostok.Clusterclient;
using Vostok.Clusterclient.Model;
using Vostok.Clusterclient.Topology;
using Vostok.Clusterclient.Transport.Http;
using Vostok.Logging;

namespace bluewater.Controllers
{
    [Route("Images")]
    public class ImagesController : Controller
    {
        private readonly FakeImageThumbsServiceClient imageThumbsServiceClient;
        private readonly FakeCandidatesServiceClient candidatesServiceClient;
        private readonly FakeImagesRepository imagesRepository;
        private readonly FakeImageThumbsRepository imageThumbsRepository;

        public ImagesController(ILog log)
        {
            imageThumbsServiceClient = new FakeImageThumbsServiceClient(log);
            candidatesServiceClient = new FakeCandidatesServiceClient(log);
            imagesRepository = new FakeImagesRepository();
            imageThumbsRepository = new FakeImageThumbsRepository();
        }

        [HttpGet("Get/{userId}/{imageId}")]
        public async Task<byte[]> GetImage(Guid userId, Guid imageId)
        {
            return await imagesRepository.ReadAsync(userId, imageId).ConfigureAwait(false);
        }

        [HttpGet("GetThumb/{userId}/{imageId}")]
        public async Task<byte[]> GetImageThumb(Guid userId, Guid imageId)
        {
            return await imageThumbsRepository.ReadAsync(userId, imageId).ConfigureAwait(false);
        }

        [HttpGet("GetUserId/{imageId}")]
        public async Task<Guid> GetUserId(Guid imageId)
        {
            return await imagesRepository.GetUserIdAsync(imageId).ConfigureAwait(false);
        }

        [HttpPost("Upload/{userId}")]
        public async Task<Guid> UploadImage(Guid userId, byte[] source)
        {
            var imageId = await imagesRepository.WriteAsync(userId, source).ConfigureAwait(false);

            var thumb = await imageThumbsServiceClient.GenerateAsync(source).ConfigureAwait(false);
            await imageThumbsRepository.WriteAsync(userId, thumb).ConfigureAwait(false);

            await candidatesServiceClient.AddCandidateAsync(imageId);

            return imageId;
        }

        [HttpDelete("Delete/{userId}/{imageId}")]
        public async Task<IActionResult> DeleteImage(Guid userId, Guid imageId)
        {
            await candidatesServiceClient.RemoveCandidateAsync(imageId);

            await imagesRepository.RemoveAsync(userId, imageId).ConfigureAwait(false);

            await imageThumbsRepository.RemoveAsync(userId, imageId).ConfigureAwait(false);

            return Ok();
        }

        private class FakeImageThumbsServiceClient
        {
            private readonly ClusterClient cluster;

            public FakeImageThumbsServiceClient(ILog log)
            {
                cluster = new ClusterClient(log, config =>
                {
                    config.ClusterProvider = new FixedClusterProvider(new Uri("http://localhost:33333"));
                    config.Transport = new VostokHttpTransport(log);
                });
            }

            public async Task<byte[]> GenerateAsync(byte[] source)
            {
                var request = Vostok.Clusterclient.Model.Request.Post("Thumbs/Generate").WithContent(new Content(source));
                var result = await cluster.SendAsync(request).ConfigureAwait(false);
                return result.Response.Content.ToArray();
            }
        }

        private class FakeCandidatesServiceClient
        {
            private readonly ClusterClient cluster;

            public FakeCandidatesServiceClient(ILog log)
            {
                cluster = new ClusterClient(log, config =>
                {
                    config.ClusterProvider = new FixedClusterProvider(new Uri("http://localhost:33333"));
                    config.Transport = new VostokHttpTransport(log);
                });
            }

            public async Task AddCandidateAsync(Guid id)
            {
                var request = Vostok.Clusterclient.Model.Request.Put("Candidates/Add").WithContent(JsonConvert.SerializeObject(id));
                await cluster.SendAsync(request).ConfigureAwait(false);
            }

            public async Task RemoveCandidateAsync(Guid id)
            {
                var request = Vostok.Clusterclient.Model.Request.Put("Candidates/Remove").WithContent(JsonConvert.SerializeObject(id));
                await cluster.SendAsync(request).ConfigureAwait(false);
            }
        }

        private class FakeImagesRepository
        {
            public async Task<Guid> GetUserIdAsync(Guid imageId)
            {
                return Guid.NewGuid();
            }

            public async Task<byte[]> ReadAsync(Guid userId, Guid imageId)
            {
                return new byte[0];
            }

            public async Task<Guid> WriteAsync(Guid userId, byte[] bytes)
            {
                return Guid.NewGuid();
            }

            public async Task RemoveAsync(Guid userId, Guid imageId)
            {
            }
        }

        private class FakeImageThumbsRepository
        {
            public async Task<byte[]> ReadAsync(Guid userId, Guid imageThumbId)
            {
                return new byte[0];
            }

            public async Task<Guid> WriteAsync(Guid userId, byte[] bytes)
            {
                return Guid.NewGuid();
            }

            public async Task RemoveAsync(Guid userId, Guid imageThumbId)
            {
            }
        }
    }
}
