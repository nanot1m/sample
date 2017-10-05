using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Vostok.ImageStore.Controllers
{
    public class ImagesContext : DbContext
    {
        public ImagesContext(DbContextOptions<ImagesContext> options)
            : base(options)
        {
        }

        public DbSet<ImageEntity> Images { get; set; }
//        public DbSet<ImageThumbEntity> ImageThumbs { get; set; }
    }

    public class ImageEntity
    {
        [Key]
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }

//    public class ImageThumbEntity
//    {
//        [Key]
//        public string Name { get; set; }
//        public byte[] Content { get; set; }
//    }
}