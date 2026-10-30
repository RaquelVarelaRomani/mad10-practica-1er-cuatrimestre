using System;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    /// <summary>
    /// Value Object con el resumen de valoraciones recibidas por un vendedor
    /// (numero total de valoraciones y puntuacion media).
    /// </summary>
    [Serializable]
    public class SellerRatingSummaryVO
    {
        #region Properties

        public long SellerId { get; private set; }
        public string SellerLoginName { get; private set; }
        public int TotalRatings { get; private set; }
        public double AverageScore { get; private set; }

        #endregion

        public SellerRatingSummaryVO(
            long sellerId,
            string sellerLoginName,
            int totalRatings,
            double averageScore)
        {
            SellerId = sellerId;
            SellerLoginName = sellerLoginName;
            TotalRatings = totalRatings;
            AverageScore = averageScore;
        }

        public override bool Equals(object obj)
        {
            SellerRatingSummaryVO target = obj as SellerRatingSummaryVO;

            if (target == null)
            {
                return false;
            }

            return SellerId == target.SellerId
                && SellerLoginName == target.SellerLoginName
                && TotalRatings == target.TotalRatings
                && Math.Abs(AverageScore - target.AverageScore) < 1e-9;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + SellerId.GetHashCode();
                hash = (hash * 23) + (SellerLoginName != null ? SellerLoginName.GetHashCode() : 0);
                hash = (hash * 23) + TotalRatings.GetHashCode();
                hash = (hash * 23) + Math.Round(AverageScore, 6).GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return String.Format(
                "[SellerId = {0} | SellerLoginName = {1} | TotalRatings = {2} | AverageScore = {3:F2}]",
                SellerId,
                SellerLoginName,
                TotalRatings,
                AverageScore);
        }
    }
}

