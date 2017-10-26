using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vostok.Sample.VotingService.Client.Models;

namespace Vostok.Sample.VotingService.Storage
{
    public class CandidatesRepository : ICandidatesRepository
    {
        private readonly CandidatesContext context;

        public CandidatesRepository(CandidatesContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(Candidate candidate)
        {
            context.Add(new CandidateEntity
            {
                UserId = candidate.UserId,
                GroupId = candidate.GroupId,
                ImageId = candidate.ImageId,
                ThumbId = candidate.ThumbId
            });
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveAsync(CandidateKey candidateKey)
        {
            context.Remove(new CandidateEntity
            {
                UserId = candidateKey.UserId,
                GroupId = candidateKey.GroupId,
                ImageId = candidateKey.ImageId
            });
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Candidate[]> SelectRandomAsync(UserKey askingUserKey, int count)
        {
            var rawSqlString = $"SELECT TOP {count} * FROM Candidates WHERE GroupId=? AND NOT (UserId=?) ORDER BY NEWID()";
            return (await context.Candidates.FromSql(rawSqlString, askingUserKey.GroupId, askingUserKey.UserId).ToListAsync().ConfigureAwait(false))
                .Select(x => new Candidate
                {
                    UserId = x.UserId,
                    GroupId = x.GroupId,
                    ImageId = x.ImageId
                }).ToArray();
        }

        public async Task<LeaderCandidate[]> GetLeadersAsync(string groupId, int count)
        {
            return (await context.Candidates.Where(x => x.GroupId == groupId).OrderByDescending(x => x.Ratio).Take(count).ToListAsync().ConfigureAwait(false))
                .Select(x => new LeaderCandidate
                {
                    UserId = x.UserId,
                    GroupId = x.GroupId,
                    ImageId = x.ImageId,
                    ThumbId = x.ThumbId,
                    Ratio = x.Ratio,
                    VotesCount = x.VotesCount,
                    ParticipationsCount = x.ParticipationsCount
                })
                .ToArray();
        }

        public async Task VoteAsync(CandidateKey candidateKey, bool vote)
        {
            var current = await context.FindAsync<CandidateEntity>(candidateKey.UserId, candidateKey.GroupId, candidateKey.ImageId).ConfigureAwait(false);
            if (current == null)
                return;

            current.ParticipationsCount++;
            if (vote)
                current.VotesCount++;
            current.Ratio = (double) current.VotesCount/current.ParticipationsCount;
            context.Update(current);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}