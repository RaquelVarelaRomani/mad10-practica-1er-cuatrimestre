using System;

namespace Es.Udc.DotNet.PracticaMAD.Model
{
    public class SellerRating
    {
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
            var other = obj as SellerRating;
            if (other == null) return false;
            return ratingId == other.ratingId;
        }

        public override int GetHashCode()
        {
            return ratingId.GetHashCode();
        }

        public override string ToString()
        {
            return $"SellerRating [ratingId={ratingId}, sellerId={sellerId}, productId={productId}, buyerUserId={buyerUserId}, score={score}]";
        }
    }
}

