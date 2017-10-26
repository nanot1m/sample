using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vostok.Sample.VotingService.Client.Models;

namespace Vostok.Sample.VotingService.Storage
{
    public class InMemoryCandidatesRepository : ICandidatesRepository
    {
        private readonly ConcurrentDictionary<CandidateKey, LeaderCandidate> candidates = new ConcurrentDictionary<CandidateKey, LeaderCandidate>();

        public async Task AddAsync(Candidate candidate)
        {
            var leaderCandidate = new LeaderCandidate(candidate, 0, 0, 0);
            var key = new CandidateKey(candidate);
            candidates.AddOrUpdate(key, leaderCandidate, (_, prev) => prev);
        }

        public async Task RemoveAsync(CandidateKey candidateKey)
        {
            candidates.TryRemove(candidateKey, out _);
        }

        private static void Shuffle<T>(IList<T> list)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < list.Count; i++)
            {
                var j = random.Next(i, list.Count);
                var tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }

        public async Task<Candidate[]> SelectRandomAsync(UserKey askingUserKey, int count)
        {
            var leaderCandidates = candidates.Values.Where(x => x.UserId != askingUserKey.UserId && x.GroupId == askingUserKey.GroupId).ToList();
            Shuffle(leaderCandidates);
            return leaderCandidates.Take(count).Select(x => new Candidate(x)).ToArray();

        }

        public async Task<LeaderCandidate[]> GetLeadersAsync(string groupId, int count)
        {
            return candidates.Values.Where(x => x.GroupId == groupId).OrderByDescending(x => x.Ratio).Take(count).Select(x => x.Clone()).ToArray();
        }

        public async Task VoteAsync(CandidateKey candidateKey, bool vote)
        {
            candidates.TryGetValue(candidateKey, out var current);
            if (current == null)
                return;

            var newValue = current.Clone();
            newValue.ParticipationsCount++;
            if (vote)
                newValue.VotesCount++;
            newValue.Ratio = (double)newValue.VotesCount / newValue.ParticipationsCount;

            candidates[candidateKey] = newValue;
        }
    }
}