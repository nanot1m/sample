using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vostok.Sample.VotingService.Models;
using Vostok.Sample.VotingService.Storage;

namespace Vostok.Sample.VotingService.Controllers
{
    [Route("Ballot")]
    public class BallotController : Controller
    {
        private readonly CandidatesRepository repository;

        public BallotController(CandidatesRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync(UserKey userKey, int count = 2)
        {
            var entities = await repository.SelectRandomAsync(userKey.UserId, userKey.GroupId, count).ConfigureAwait(false);
            return Json(
                entities.Select(
                        e => new Candidate
                        {
                            UserId = e.UserId,
                            GroupId = e.GroupId,
                            ImageId = e.ImageId,
                            ThumbId = e.ThumbId
                        })
                    .ToArray());
        }

        [HttpPost]
        public async Task AddAsync([FromBody] Ballot ballot)
        {
            foreach (var candidate in ballot.Candidates)
                await repository.UpdateAsync(candidate.UserId, candidate.GroupId, candidate.ImageId, candidate.Vote).ConfigureAwait(false);
        }
    }
}