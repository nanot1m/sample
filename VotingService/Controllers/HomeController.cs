using Microsoft.AspNetCore.Mvc;

namespace Vostok.Sample.VotingService.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet]
        public string Index()
        {
            return "VotingService";
        }
    }
}