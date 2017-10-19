using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vostok.Sample.VotingService.Models;
using Vostok.Sample.VotingService.Storage;

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

        [HttpPut]
        public async Task AddAsync(Candidate candidate)
        {
            await repository.AddAsync(
                    new CandidateEntity
                    {
                        UserId = candidate.UserId,
                        GroupId = candidate.GroupId,
                        ImageId = candidate.ImageId,
                        ThumbId = candidate.ThumbId
                    })
                .ConfigureAwait(false);
        }

        [HttpDelete]
        public async Task DeleteAsync(CandidateKey candidateKey)
        {
            await repository.RemoveAsync(
                    new CandidateEntity
                    {
                        UserId = candidateKey.UserId,
                        GroupId = candidateKey.GroupId,
                        ImageId = candidateKey.ImageId
                    })
                .ConfigureAwait(false);
        }
    }
}