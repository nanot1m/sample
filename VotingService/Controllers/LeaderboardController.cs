using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vostok.Sample.VotingService.Storage;

namespace Vostok.Sample.VotingService.Controllers
{
    [Route("Leaderboard")]
    public class LeaderboardController : Controller
    {
        private readonly ICandidatesRepository repository;

        public LeaderboardController(ICandidatesRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync(int count = 10)
        {
            var leaders = await repository.GetLeadersAsync(count).ConfigureAwait(false);
            return Json(leaders);
        }
    }
}