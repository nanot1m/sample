using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vostok.Sample.VotingService.Models;
using Vostok.Sample.VotingService.Storage;

namespace Vostok.Sample.VotingService.Controllers
{
    [Route("Leaderboard")]
    public class LeaderboardController : Controller
    {
        private readonly CandidatesRepository repository;

        public LeaderboardController(CandidatesRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync(int count = 10)
        {
            var leaders = await repository.GetLeadersAsync(count).ConfigureAwait(false);
            return Json(
                leaders.Select(
                        e => new LeaderCandidate
                        {
                            UserId = e.UserId,
                            GroupId = e.GroupId,
                            ImageId = e.ImageId,
                            ThumbId = e.ThumbId,
                            ParticipationsCount = e.ParticipationsCount,
                            VotesCount = e.VotesCount,
                            Ratio = e.Ratio
                        })
                    .ToArray());
        }
    }
}