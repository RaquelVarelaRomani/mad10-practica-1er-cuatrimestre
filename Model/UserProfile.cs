using System;
using System.Collections.Generic;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    public class UserProfile
    {
        public long usrId { get; set; }
        public string loginName { get; set; }
        public string enPassword { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string language { get; set; }
        public string country { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<CommentReport> CommentReports { get; set; }
        public virtual ICollection<SellerRating> ReceivedRatings { get; set; }
        public virtual ICollection<SellerRating> GivenRatings { get; set; }

        public UserProfile()
        {
            Products = new HashSet<Product>();
            Comments = new HashSet<Comment>();
            CommentReports = new HashSet<CommentReport>();
            ReceivedRatings = new HashSet<SellerRating>();
            GivenRatings = new HashSet<SellerRating>();
        }

        public override bool Equals(object obj)
        {
            var other = obj as UserProfile;
            if (other == null) return false;
            return usrId == other.usrId || (loginName != null && loginName == other.loginName);
        }

        public override int GetHashCode()
        {
            return loginName != null ? loginName.GetHashCode() : usrId.GetHashCode();
        }

        public override string ToString()
        {
            return $"UserProfile [usrId={usrId}, loginName={loginName}, firstName={firstName}, lastName={lastName}, email={email}]";
        }
    }
}

