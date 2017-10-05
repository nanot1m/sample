using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace bluewater.Controllers
{
    [Route("Candidates")]
    public class CandidatesController : Controller
    {
        [ThreadStatic] private static Random random;
        private static Random Random => random ?? (random = new Random());

        private readonly FakeCandidatesRepository candidatesRepository = new FakeCandidatesRepository();

        [HttpGet("Select")]
        public async Task<Candidates> Select()
        {
            var candidates = await candidatesRepository.SelectCandidatesAsync().ConfigureAwait(false);

            var index1 = Random.Next(candidates.Length - 1);
            int index2;
            do
            {
                index2 = Random.Next(candidates.Length - 1);
            } while (index1 != index2);

            return new Candidates {Id1 = candidates[index1], Id2 = candidates[index2]};
        }

        [HttpGet("Add/{imageId}")]
        public async Task AddCandidate(Guid imageId)
        {
            await candidatesRepository.WriteCandidateAsync(imageId).ConfigureAwait(false);
        }

        [HttpGet("Remove/{imageId}")]
        public async Task RemoveCandidate(Guid imageId)
        {
            await candidatesRepository.RemoveCandidateAsync(imageId).ConfigureAwait(false);
        }

        public class Candidates
        {
            public Guid Id1 { get; set; }
            public Guid Id2 { get; set; }
        }

        private class FakeCandidatesRepository
        {
            public async Task<Guid[]> SelectCandidatesAsync()
            {
                return new Guid[0];
            }

            public async Task WriteCandidateAsync(Guid id)
            {
            }

            public async Task RemoveCandidateAsync(Guid id)
            {
            }
        }
    }
}