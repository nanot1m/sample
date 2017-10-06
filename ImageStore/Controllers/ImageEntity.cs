using System.ComponentModel.DataAnnotations;

namespace Vostok.Sample.ImageStore.Controllers
{
    public class ImageEntity
    {
        [Key]
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}