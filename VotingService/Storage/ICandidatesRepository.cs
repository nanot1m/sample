using System.Threading.Tasks;
using Vostok.Sample.VotingService.Client.Models;

namespace Vostok.Sample.VotingService.Storage
{
    public interface ICandidatesRepository
    {
        Task AddAsync(Candidate candidate);
        Task RemoveAsync(CandidateKey candidateKey);
        Task<Candidate[]> SelectRandomAsync(UserKey askingUserKey, int count);
        Task<LeaderCandidate[]> GetLeadersAsync(string groupId, int count);
        Task VoteAsync(CandidateKey candidateKey, bool vote);
    }
}