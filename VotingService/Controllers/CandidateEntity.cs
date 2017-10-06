using System.ComponentModel.DataAnnotations;

namespace Vostok.Sample.VotingService.Controllers
{
    public class CandidateEntity
    {
        [Key]
        public string Name { get; set; }

        public int ParticipationsCount { get; set; }

        public int VotesCount { get; set; }

        public double Ratio { get; set; }
    }
}