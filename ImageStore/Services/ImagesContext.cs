using Microsoft.EntityFrameworkCore;

namespace Vostok.Sample.ImageStore.Services
{
    public class ImagesContext : DbContext
    {
        public ImagesContext(string connectionString)
            : base(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options)
        {
        }

        public DbSet<ImageEntity> Images { get; set; }
    }
}