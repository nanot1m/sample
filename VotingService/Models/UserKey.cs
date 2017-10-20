using System;

namespace Vostok.Sample.VotingService.Models
{
    public class UserKey : IEquatable<UserKey>
    {
        public UserKey()
        {
        }

        public UserKey(string userId, string groupId)
        {
            UserId = userId;
            GroupId = groupId;
        }

        public UserKey(UserKey userKey)
            : this(userKey.UserId, userKey.GroupId)
        {
        }

        public string UserId { get; set; }
        public string GroupId { get; set; }

        public bool Equals(UserKey other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return string.Equals(UserId, other.UserId) && string.Equals(GroupId, other.GroupId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((UserKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((UserId != null ? UserId.GetHashCode() : 0)*397) ^ (GroupId != null ? GroupId.GetHashCode() : 0);
            }
        }

        public UserKey Clone()
        {
            return new UserKey(this);
        }
    }
}