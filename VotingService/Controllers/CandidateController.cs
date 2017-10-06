using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Vostok.Sample.VotingService.Controllers
{
    [Route("Candidate")]
    public class CandidateController : Controller
    {
        private readonly CandidatesRepository repository;

        public CandidateController(CandidatesRepository repository)
        {
            this.repository = repository;
        }

        [HttpPut("{*name}")]
        public async Task AddAsync(string name)
        {
            await repository.AddAsync(name).ConfigureAwait(false);
        }

        [HttpDelete("{*name}")]
        public async Task DeleteAsync(string name)
        {
            await repository.RemoveAsync(name).ConfigureAwait(false);
        }

        [HttpGet("{*count}")]
        public async Task<string[]> SelectRandomAsync(int count)
        {
            var candidates = await repository.SelectRandomAsync(count).ConfigureAwait(false);
            return candidates.Select(x => x.Name).ToArray();
        }
    }
}