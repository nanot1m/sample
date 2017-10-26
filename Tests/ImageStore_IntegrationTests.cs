using System;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging;
using Vostok.Logging.Logs;
using Vostok.Sample.ImageStore.Client;

namespace Tests
{
    public class ImageStore_IntegrationTests
    {
        private ILog log;
        private ImageStoreClient imageStoreClient;

        [SetUp]
        public void SetUp()
        {
            log = new ConsoleLog();
            imageStoreClient = new ImageStoreClient(log, new Uri("http://localhost:33334"));
        }

        [Test]
        public async Task Upload_Then_Download_ShouldBeSameContent()
        {
            var imageId = await imageStoreClient.UploadAsync(Encoding.ASCII.GetBytes("lalala"));
            var downloadedBytes = await imageStoreClient.DownloadAsync(imageId);
            Encoding.ASCII.GetString(downloadedBytes).Should().Be("lalala");
        }

        [Test]
        public async Task Download_UnknownImageId_ShouldReturnNull()
        {
            var downloadedBytes = await imageStoreClient.DownloadAsync("unknownImageId");
            downloadedBytes.Should().BeNull();
        }

        [Test]
        public async Task Upload_Then_Remove_Then_Download_ShouldReturnNull()
        {
            var imageId = await imageStoreClient.UploadAsync(Encoding.ASCII.GetBytes("lalala"));
            await imageStoreClient.RemoveAsync(imageId);
            var downloadedBytes = await imageStoreClient.DownloadAsync(imageId);
            downloadedBytes.Should().BeNull();
        }

        [Test]
        public async Task Upload_Then_Remove_ShouldReturnTrue()
        {
            var imageId = await imageStoreClient.UploadAsync(Encoding.ASCII.GetBytes("lalala"));
            var removed = await imageStoreClient.RemoveAsync(imageId);
            removed.Should().BeTrue();
        }

        [Test]
        public async Task Remove_UnknownImageId_ShouldReturnFalse()
        {
            var removed = await imageStoreClient.RemoveAsync("unknownImageId");
            removed.Should().BeFalse();
        }

        [Test]
        public async Task Upload_SameBytes_ShowReturnSameIds()
        {
            var content = Encoding.ASCII.GetBytes("lalala");
            var imageId1 = await imageStoreClient.UploadAsync(content);
            var imageId2 = await imageStoreClient.UploadAsync(content);
            imageId2.Should().Be(imageId1);
        }

        [Test]
        public async Task Upload_DifferentBytes_ShowReturnDifferentIds()
        {
            var content1 = Encoding.ASCII.GetBytes("lalala");
            var content2 = Encoding.ASCII.GetBytes("bububu");
            var imageId1 = await imageStoreClient.UploadAsync(content1);
            var imageId2 = await imageStoreClient.UploadAsync(content2);
            imageId2.Should().NotBe(imageId1);
        }
    }
}
