using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Vostok.Sample.VotingService.Storage
{
    public class CandidatesRepository
    {
        private readonly CandidatesContext context;

        public CandidatesRepository(CandidatesContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(CandidateEntity entity)
        {
            context.Add(entity);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveAsync(CandidateEntity entity)
        {
            context.Remove(entity);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<CandidateEntity[]> SelectRandomAsync(string userId, string groupId, int count)
        {
            var rawSqlString = $"SELECT TOP {count} * FROM Candidates WHERE GroupId=? AND NOT (UserId=?) ORDER BY NEWID()";
            return await context.Candidates.FromSql(rawSqlString, groupId, userId).ToArrayAsync().ConfigureAwait(false);
        }

        public async Task<List<CandidateEntity>> GetLeadersAsync(int count)
        {
            return await context.Candidates.OrderByDescending(x => x.Ratio).Take(count).ToListAsync().ConfigureAwait(false);
        }

        public async Task UpdateAsync(string userId, string groupId, string imageId, bool vote)
        {
            var current = await context.FindAsync<CandidateEntity>(userId, groupId, imageId).ConfigureAwait(false);
            current.ParticipationsCount++;
            if (vote)
                current.VotesCount++;
            current.Ratio = (double) current.VotesCount/current.ParticipationsCount;
            context.Update(current);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}