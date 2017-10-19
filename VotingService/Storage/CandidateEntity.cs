using System.ComponentModel.DataAnnotations;

namespace Vostok.Sample.VotingService.Storage
{
    public class CandidateEntity
    {
        [Key]
        public string UserId { get; set; }

        [Key]
        public string GroupId { get; set; }

        [Key]
        public string ImageId { get; set; }

        public string ThumbId { get; set; }

        public int ParticipationsCount { get; set; }

        public int VotesCount { get; set; }

        public double Ratio { get; set; }
    }
}