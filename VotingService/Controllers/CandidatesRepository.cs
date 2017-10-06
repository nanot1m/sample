using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Vostok.Sample.VotingService.Controllers
{
    public class CandidatesRepository
    {
        private readonly CandidatesContext context;

        public CandidatesRepository(CandidatesContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(string name)
        {
            context.Add(new CandidateEntity {Name = name});
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task RemoveAsync(string name)
        {
            context.Remove(new CandidateEntity {Name = name});
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<CandidateEntity[]> SelectRandomAsync(int count)
        {
            var rawSqlString = $"SELECT TOP {count} * FROM Candidates ORDER BY NEWID()";
            return await context.Candidates.FromSql(rawSqlString).ToArrayAsync().ConfigureAwait(false);
        }

        public async Task<CandidateEntity> GetWithMaxRatioAsync()
        {
            return await context.Candidates.OrderByDescending(x => x.Ratio).FirstAsync().ConfigureAwait(false);
        }

        public async Task UpdateAsync(string name, bool voteResult)
        {
            var current = await context.FindAsync<CandidateEntity>(name);
            current.ParticipationsCount++;
            if (voteResult)
                current.VotesCount++;
            current.Ratio = (double) current.VotesCount / current.ParticipationsCount;
            context.Update(current);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}