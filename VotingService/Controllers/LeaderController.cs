using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Vostok.Sample.VotingService.Controllers
{
    [Route("Leader")]
    public class LeaderController : Controller
    {
        private readonly CandidatesRepository repository;

        public LeaderController(CandidatesRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<string> GetAsync()
        {
            var leader = await repository.GetWithMaxRatioAsync().ConfigureAwait(false);
            return leader.Name;
        }
    }
}