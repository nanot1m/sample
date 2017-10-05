using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Vostok.ImageStore.Controllers
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

        public async Task<byte[]> DownloadAsync(string name)
        {
            byte[] content;
            if (images.TryGetValue(name, out content))
            {
                return content;
            }

            throw new ImageNotFoundException();
        }

        public async Task UploadAsync(string name, byte[] content)
        {
            if (images.TryAdd(name, content))
            {
                return;
            }

            throw new ImageUploadConflictException();
        }

        public async Task<string[]> SearchByName(string name)
        {
            var searchRegex = new Regex($".*{name}.*", RegexOptions.IgnoreCase);

            var result = new List<string>();
            foreach (var key in images.Select(x => x.Key))
            {
                if (searchRegex.IsMatch(key))
                {
                    result.Add(key);
                }
            }

            return result.OrderBy(x => x, StringComparer.InvariantCultureIgnoreCase).ToArray();
        }
    }
}