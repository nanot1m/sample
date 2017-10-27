namespace Vostok.Sample.VotingService.Client.Models
{
    public class BallotCandidate : CandidateKey
    {
        public BallotCandidate()
        {
        }

        public BallotCandidate(CandidateKey candidateKey, bool vote)
            : base(candidateKey)
        {
            Vote = vote;
        }

        public bool Vote { get; set; }
    }
}