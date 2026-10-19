using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Es.Udc.DotNet.ModelUtil.Dao;

namespace Es.Udc.DotNet.PracticaMAD.Model.SellerRatingDao
{
    public class SellerRatingDaoEntityFramework : GenericDaoEntityFramework<SellerRating, long>, ISellerRatingDao
    {
        public List<SellerRating> FindBySellerId(long sellerId, int startIndex, int count)
        {
            DbSet<SellerRating> ratings = Context.Set<SellerRating>();

            return ratings
                .Where(r => r.sellerId == sellerId)
                .OrderByDescending(r => r.ratingDate)
                .Skip(startIndex)
                .Take(count)
                .ToList();
        }

        public int GetNumberOfRatingsBySeller(long sellerId)
        {
            DbSet<SellerRating> ratings = Context.Set<SellerRating>();

            return ratings.Count(r => r.sellerId == sellerId);
        }

        public double GetAverageRatingBySeller(long sellerId)
        {
            DbSet<SellerRating> ratings = Context.Set<SellerRating>();

            var sellerRatings = ratings.Where(r => r.sellerId == sellerId);

            if (!sellerRatings.Any())
            {
                return 0.0;
            }

            return sellerRatings.Average(r => (double)r.score);
        }

        public SellerRating FindBySellerAndProductAndBuyer(long sellerId, long productId, long buyerUserId)
        {
            DbSet<SellerRating> ratings = Context.Set<SellerRating>();

            return (from r in ratings
                    where r.sellerId == sellerId && r.productId == productId && r.buyerUserId == buyerUserId
                    select r).FirstOrDefault();
        }
    }
}

