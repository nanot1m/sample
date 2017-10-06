using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Vostok.Sample.VotingService.Controllers
{
    [Route("BallotBox")]
    public class BallotBoxController : Controller
    {
        private readonly CandidatesRepository repository;

        public BallotBoxController(CandidatesRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        public async Task AddAsync([FromBody] Ballot ballot)
        {
            foreach (var candidate in ballot.Candidates)
            {
                await repository.UpdateAsync(candidate.Name, candidate.VoteResult).ConfigureAwait(false);
            }
        }
    }
}