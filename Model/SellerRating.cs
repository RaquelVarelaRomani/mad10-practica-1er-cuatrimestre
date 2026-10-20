using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    [Table("SellerRating")]
    public partial class SellerRating
    {
        [Key]
        public long ratingId { get; set; }
        public long sellerId { get; set; }
        public long productId { get; set; }
        public long buyerUserId { get; set; }
        public int score { get; set; }
        public string comment { get; set; }
        public DateTime ratingDate { get; set; }

        public virtual UserProfile Seller { get; set; }
        public virtual Product Product { get; set; }
        public virtual UserProfile BuyerUser { get; set; }

        public override bool Equals(object obj)
        {
            SellerRating target = obj as SellerRating;
            if (target == null)
            {
                return false;
            }

            return (this.ratingId == target.ratingId)
                && (this.sellerId == target.sellerId)
                && (this.productId == target.productId)
                && (this.buyerUserId == target.buyerUserId)
                && (this.score == target.score)
                && (this.comment == target.comment)
                && (this.ratingDate == target.ratingDate);
        }

        public override int GetHashCode()
        {
            return this.ratingId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format(
                "[ ratingId = {0} | sellerId = {1} | productId = {2} | buyerUserId = {3} | score = {4} | comment = {5} | ratingDate = {6} ]",
                this.ratingId, this.sellerId, this.productId, this.buyerUserId, this.score, this.comment, this.ratingDate);
        }
    }
}

