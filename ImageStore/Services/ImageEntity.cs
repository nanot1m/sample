using System.ComponentModel.DataAnnotations;

namespace Vostok.Sample.ImageStore.Services
{
    public class ImageEntity
    {
        [Key]
        public string Id { get; set; }
        public byte[] Content { get; set; }
    }
}