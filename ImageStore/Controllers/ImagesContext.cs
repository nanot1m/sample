using Microsoft.EntityFrameworkCore;

namespace Vostok.Sample.ImageStore.Controllers
{
    public class ImagesContext : DbContext
    {
        public ImagesContext(DbContextOptions<ImagesContext> options)
            : base(options)
        {
        }

        public DbSet<ImageEntity> Images { get; set; }
    }
}