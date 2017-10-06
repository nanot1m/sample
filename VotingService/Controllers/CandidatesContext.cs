using Microsoft.EntityFrameworkCore;

namespace Vostok.Sample.VotingService.Controllers
{
    public class CandidatesContext : DbContext
    {
        public CandidatesContext(DbContextOptions<CandidatesContext> options)
            : base(options)
        {
        }

        public DbSet<CandidateEntity> Candidates { get; set; }
    }
}