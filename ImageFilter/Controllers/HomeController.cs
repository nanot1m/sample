using Microsoft.AspNetCore.Mvc;

namespace Vostok.Sample.ImageFilter.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet]
        public string Index()
        {
            return "ImageFilter";
        }
    }
}