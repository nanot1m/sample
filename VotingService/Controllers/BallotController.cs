using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vostok.Sample.VotingService.Client.Models;
using Vostok.Sample.VotingService.Storage;

namespace Vostok.Sample.VotingService.Controllers
{
    [Route("Ballot")]
    public class BallotController : Controller
    {
        private readonly ICandidatesRepository repository;

        public BallotController(ICandidatesRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync(UserKey userKey, int count = 2)
        {
            var candidates = await repository.SelectRandomAsync(userKey, count).ConfigureAwait(false);
            return Json(candidates);
        }

        [HttpPost]
        public async Task VoteAsync([FromBody] Ballot ballot)
        {
            foreach (var candidate in ballot.Candidates)
                await repository.VoteAsync(candidate, candidate.Vote).ConfigureAwait(false);
        }
    }
}