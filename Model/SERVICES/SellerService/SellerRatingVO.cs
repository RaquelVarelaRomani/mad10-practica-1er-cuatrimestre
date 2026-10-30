using System;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    /// <summary>
    /// Value Object con los detalles de una valoracion realizada sobre un vendedor.
    /// </summary>
    [Serializable]
    public class SellerRatingVO
    {
        #region Properties

        public long RatingId { get; private set; }
        public long SellerId { get; private set; }
        public string SellerLoginName { get; private set; }
        public long ProductId { get; private set; }
        public string ProductName { get; private set; }
        public long BuyerUserId { get; private set; }
        public string BuyerLoginName { get; private set; }
        public int Score { get; private set; }
        public string Comment { get; private set; }
        public DateTime RatingDate { get; private set; }

        #endregion

        public SellerRatingVO(
            long ratingId,
            long sellerId,
            string sellerLoginName,
            long productId,
            string productName,
            long buyerUserId,
            string buyerLoginName,
            int score,
            string comment,
            DateTime ratingDate)
        {
            RatingId = ratingId;
            SellerId = sellerId;
            SellerLoginName = sellerLoginName;
            ProductId = productId;
            ProductName = productName;
            BuyerUserId = buyerUserId;
            BuyerLoginName = buyerLoginName;
            Score = score;
            Comment = comment;
            RatingDate = ratingDate;
        }

        public override bool Equals(object obj)
        {
            SellerRatingVO target = obj as SellerRatingVO;

            if (target == null)
            {
                return false;
            }

            return RatingId == target.RatingId
                && SellerId == target.SellerId
                && SellerLoginName == target.SellerLoginName
                && ProductId == target.ProductId
                && ProductName == target.ProductName
                && BuyerUserId == target.BuyerUserId
                && BuyerLoginName == target.BuyerLoginName
                && Score == target.Score
                && Comment == target.Comment
                && RatingDate == target.RatingDate;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + RatingId.GetHashCode();
                hash = (hash * 23) + SellerId.GetHashCode();
                hash = (hash * 23) + (SellerLoginName != null ? SellerLoginName.GetHashCode() : 0);
                hash = (hash * 23) + ProductId.GetHashCode();
                hash = (hash * 23) + (ProductName != null ? ProductName.GetHashCode() : 0);
                hash = (hash * 23) + BuyerUserId.GetHashCode();
                hash = (hash * 23) + (BuyerLoginName != null ? BuyerLoginName.GetHashCode() : 0);
                hash = (hash * 23) + Score.GetHashCode();
                hash = (hash * 23) + (Comment != null ? Comment.GetHashCode() : 0);
                hash = (hash * 23) + RatingDate.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return String.Format(
                "[RatingId = {0} | SellerId = {1} | SellerLoginName = {2} | ProductId = {3} | ProductName = {4} | BuyerUserId = {5} | BuyerLoginName = {6} | Score = {7} | Comment = {8} | RatingDate = {9}]",
                RatingId,
                SellerId,
                SellerLoginName,
                ProductId,
                ProductName,
                BuyerUserId,
                BuyerLoginName,
                Score,
                Comment,
                RatingDate);
        }
    }
}

