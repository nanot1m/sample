using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Logging;
using Vostok.Logging.Logs;
using Vostok.Sample.ImageFilter.Client;
using Vostok.Sample.ImageStore.Client;

namespace Tests
{
    public class ImageFilter_IntegrationTests
    {
        private ILog log;
        private ImageStoreClient imageStoreClient;
        private ImageFilterClient imageFilterClient;
        private string testDataDir;

        [SetUp]
        public void SetUp()
        {
            log = new ConsoleLog();
            imageStoreClient = new ImageStoreClient(log, new Uri("http://localhost:33334"));
            imageFilterClient = new ImageFilterClient(log, new Uri("http://localhost:33337"));
            testDataDir = FileHelper.PatchDirectoryName("TestData");
        }

        [Test]
        public async Task BlackWhite()
        {
            await RunTest(s => imageFilterClient.ApplyBlackWhiteAsync(s));
        }

        [Test]
        public async Task Brightness()
        {
            await RunTest(s => imageFilterClient.ApplyBrightnessAsync(s, 50));
        }

        [Test]
        public async Task Contrast()
        {
            await RunTest(s => imageFilterClient.ApplyContrastAsync(s, 50));
        }

        [Test]
        public async Task Glow()
        {
            await RunTest(s => imageFilterClient.ApplyGlowAsync(s));
        }

        [Test]
        public async Task Invert()
        {
            await RunTest(s => imageFilterClient.ApplyInvertAsync(s));
        }

        [Test]
        public async Task OilPaint()
        {
            await RunTest(s => imageFilterClient.ApplyOilPaintAsync(s));
        }

        [Test]
        public async Task Pixelate()
        {
            await RunTest(s => imageFilterClient.ApplyPixelateAsync(s, 5));
        }

        [Test]
        public async Task Resize()
        {
            await RunTest(s => imageFilterClient.ApplyResizeAsync(s, 100, 100));
        }

        [Test]
        public async Task Vignette()
        {
            await RunTest(s => imageFilterClient.ApplyVignetteAsync(s));
        }

        private async Task RunTest(Func<string, Task<string>> applyFilterAsync)
        {
            var filterName = TestContext.CurrentContext.Test.MethodName;
            var sourceImageFileName = Path.Combine(testDataDir, "image.jpg");
            var approvedImageFileName = Path.Combine(testDataDir, "image." + filterName + ".approved.jpg");
            var filteredImageFileName = Path.Combine(testDataDir, "image." + filterName + ".actual.jpg");
            var sourceId = await imageStoreClient.UploadAsync(File.ReadAllBytes(sourceImageFileName));
            var filteredId = await applyFilterAsync(sourceId);
            var filteredBytes = await imageStoreClient.DownloadAsync(filteredId);
            File.WriteAllBytes(filteredImageFileName, filteredBytes);
            if (!File.Exists(approvedImageFileName))
                File.WriteAllBytes(approvedImageFileName, filteredBytes);
            else
                FileAssert.AreEqual(approvedImageFileName, filteredImageFileName);
        }
    }
}