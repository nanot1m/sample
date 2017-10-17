using System.Threading.Tasks;

namespace Vostok.Sample.ImageStore.Services
{
    public class ImagesRepository : IImagesRepository
    {
        private readonly ImagesContext context;

        public ImagesRepository(ImagesContext context)
        {
            this.context = context;
        }

        public async Task<byte[]> DownloadAsync(string id)
        {
            var entity = await context.FindAsync<ImageEntity>(id).ConfigureAwait(false);
            return entity?.Content;
        }

        public async Task<string> UploadAsync(byte[] content)
        {
            var id = ImageIdGenerator.Generate(content);
            context.Add(new ImageEntity {Id = id, Content = content});
            await context.SaveChangesAsync().ConfigureAwait(false);
            return id;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            context.Remove(new ImageEntity {Id = id});
            return await context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }
    }
}