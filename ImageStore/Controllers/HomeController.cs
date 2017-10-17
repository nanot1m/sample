using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Vostok.Sample.ImageStore.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet]
        public async Task<object> Index()
        {
            return "ImageStore";
        }
    }
}