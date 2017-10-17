using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace Vostok.Sample.ImageStore.Services
{
    public class InMemoryImagesRepository : IImagesRepository
    {
        private readonly ConcurrentDictionary<string, byte[]> images = new ConcurrentDictionary<string, byte[]>(StringComparer.InvariantCultureIgnoreCase);

        public InMemoryImagesRepository()
        {
            images["pliner/1.txt"] = Encoding.UTF8.GetBytes("pliner/1.txt");
            images["pliner/2.txt"] = Encoding.UTF8.GetBytes("pliner/2.txt");
            images["pliner/3.txt"] = Encoding.UTF8.GetBytes("pliner/3.txt");
        }

        public async Task<byte[]> DownloadAsync(string id)
        {
            if (images.TryGetValue(id, out var content))
                return content;
            return null;
        }

        public async Task<string> UploadAsync(byte[] content)
        {
            var id = ImageIdGenerator.Generate(content);
            images[id] = content;
            return id;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            return images.TryRemove(id, out _);
        }
    }
}