using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vostok.Sample.VotingService.Client.Models;
using Vostok.Sample.VotingService.Storage;

namespace Vostok.Sample.VotingService.Controllers
{
    [Route("Candidate")]
    public class CandidateController : Controller
    {
        private readonly ICandidatesRepository repository;

        public CandidateController(ICandidatesRepository repository)
        {
            this.repository = repository;
        }

        [HttpPut]
        public async Task AddAsync(Candidate candidate)
        {
            await repository.AddAsync(candidate).ConfigureAwait(false);
        }

        [HttpDelete]
        public async Task DeleteAsync(CandidateKey candidateKey)
        {
            await repository.RemoveAsync(candidateKey).ConfigureAwait(false);
        }
    }
}