using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Vostok.Sample.ImageFilter.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet]
        public async Task<object> Index()
        {
            return "ImageFilter";
        }
    }
}