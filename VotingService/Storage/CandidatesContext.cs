using Microsoft.EntityFrameworkCore;

namespace Vostok.Sample.VotingService.Storage
{
    public class CandidatesContext : DbContext
    {
        public CandidatesContext(DbContextOptions<CandidatesContext> options)
            : base((DbContextOptions) options)
        {
        }

        public DbSet<CandidateEntity> Candidates { get; set; }
    }
}