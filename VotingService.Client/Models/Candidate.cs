namespace Vostok.Sample.VotingService.Client.Models
{
    public class Candidate : CandidateKey
    {
        public Candidate()
        {
        }

        public Candidate(string userId, string groupId, string imageId, string thumbId)
            : this(new CandidateKey(userId, groupId, imageId), thumbId)
        {
        }

        public Candidate(Candidate candidate)
            : this(candidate, candidate.ThumbId)
        {
        }

        public Candidate(CandidateKey candidateKey, string thumbId)
            : base(candidateKey)
        {
            ThumbId = thumbId;
        }

        public string ThumbId { get; set; }

        public new Candidate Clone()
        {
            return new Candidate(this);
        }
    }
}