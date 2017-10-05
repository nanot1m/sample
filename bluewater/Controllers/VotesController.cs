using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace bluewater.Controllers
{
    [Route("Votes")]
    public class VotesController
    {
        private readonly FakeVotesRepository votesRepository = new FakeVotesRepository();

        [HttpGet("Add/{imageId}")]
        public async Task Add(Guid imageId)
        {
            await votesRepository.AddAsync(imageId).ConfigureAwait(false);
        }

        [HttpGet("GetLeader")]
        public async Task<Guid> GetLeader(Guid imageId)
        {
            return await votesRepository.GetLeaderAsync().ConfigureAwait(false);
        }

        private class FakeVotesRepository
        {
            public async Task AddAsync(Guid id)
            {
            }

            public async Task<Guid> GetLeaderAsync()
            {
                return Guid.NewGuid();
            }
        }
    }
}