using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Vostok.ImageStore.Controllers
{
    public class ImagesRepository : IImagesRepository
    {
        private readonly ImagesContext context;

        public ImagesRepository(ImagesContext context)
        {
            this.context = context;
        }

        public async Task<byte[]> DownloadAsync(string name)
        {
            var entity = await context.FindAsync<ImageEntity>(name).ConfigureAwait(false);
            return entity.Content;
        }

        public async Task UploadAsync(string name, byte[] content)
        {
            context.Add(new ImageEntity {Name = name, Content = content});
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<string[]> SearchByName(string name)
        {
            return await context.Images.Where(x => x.Name.StartsWith(name)).Select(x => x.Name).ToArrayAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(string name)
        {
            context.Remove(new ImageEntity {Name = name});
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}