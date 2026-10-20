using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    [Table("UserProfile")]
    public partial class UserProfile
    {
        public UserProfile()
        {
            this.Products = new HashSet<Product>();
            this.Comments = new HashSet<Comment>();
            this.CommentReports = new HashSet<CommentReport>();
            this.ReceivedRatings = new HashSet<SellerRating>();
            this.GivenRatings = new HashSet<SellerRating>();
        }

        [Key]
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

        public override bool Equals(object obj)
        {
            UserProfile target = obj as UserProfile;
            if (target == null)
            {
                return false;
            }

            return (this.usrId == target.usrId)
                && (this.loginName == target.loginName)
                && (this.enPassword == target.enPassword)
                && (this.firstName == target.firstName)
                && (this.lastName == target.lastName)
                && (this.email == target.email)
                && (this.language == target.language)
                && (this.country == target.country);
        }

        public override int GetHashCode()
        {
            return this.usrId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(
                "[ usrId = {0} | loginName = {1} | enPassword = {2} | firstName = {3} | lastName = {4} | email = {5} | language = {6} | country = {7} ]",
                this.usrId, this.loginName, this.enPassword, this.firstName, this.lastName, this.email, this.language, this.country);
        }
    }
}

