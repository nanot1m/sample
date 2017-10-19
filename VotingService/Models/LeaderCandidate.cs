namespace Vostok.Sample.VotingService.Models
{
    public class LeaderCandidate : Candidate
    {
        public int ParticipationsCount { get; set; }
        public int VotesCount { get; set; }
        public double Ratio { get; set; }
    }
}