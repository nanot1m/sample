using Microsoft.EntityFrameworkCore;

namespace Vostok.Sample.VotingService.Storage
{
    public class CandidatesContext : DbContext
    {
        public CandidatesContext(string connectionString)
            : base(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options)
        {
        }

        public DbSet<CandidateEntity> Candidates { get; set; }
    }
}