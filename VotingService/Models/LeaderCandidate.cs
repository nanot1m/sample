namespace Vostok.Sample.VotingService.Models
{
    public class LeaderCandidate : Candidate
    {
        public LeaderCandidate()
        {
        }

        public LeaderCandidate(string userId, string groupId, string imageId, string thumbId, int participationsCount, int votesCount, double ratio)
            : this(new Candidate(userId, groupId, imageId, thumbId), participationsCount, votesCount, ratio)
        {
        }

        public LeaderCandidate(LeaderCandidate leaderCandidate)
            : this(leaderCandidate, leaderCandidate.ParticipationsCount, leaderCandidate.VotesCount, leaderCandidate.Ratio)
        {
        }

        public LeaderCandidate(Candidate candidate, int participationsCount, int votesCount, double ratio)
            : base(candidate)
        {
            ParticipationsCount = participationsCount;
            VotesCount = votesCount;
            Ratio = ratio;
        }

        public int ParticipationsCount { get; set; }
        public int VotesCount { get; set; }
        public double Ratio { get; set; }

        public new LeaderCandidate Clone()
        {
            return new LeaderCandidate(this);
        }
    }
}