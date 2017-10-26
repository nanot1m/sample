using Microsoft.AspNetCore.Mvc;

namespace Vostok.Sample.ImageStore.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet]
        public string Index()
        {
            return "ImageStore";
        }
    }
}