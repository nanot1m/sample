using System;

namespace Vostok.Sample.VotingService.Models
{
    public class CandidateKey : UserKey, IEquatable<CandidateKey>
    {
        public CandidateKey()
        {
        }

        public CandidateKey(string userId, string groupId, string imageId)
            : this(new UserKey(userId, groupId), imageId)
        {
        }

        public CandidateKey(CandidateKey candidateKey)
            : this(candidateKey, candidateKey.ImageId)
        {
        }

        public CandidateKey(UserKey userKey, string imageId)
            : base(userKey)
        {
            ImageId = imageId;
        }

        public string ImageId { get; set; }

        public bool Equals(CandidateKey other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return base.Equals(other) && string.Equals(ImageId, other.ImageId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((CandidateKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ (ImageId != null ? ImageId.GetHashCode() : 0);
            }
        }

        public new CandidateKey Clone()
        {
            return new CandidateKey(this);
        }
    }
}