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

            var result = (from r in ratings
                          where r.sellerId == sellerId
                          orderby r.ratingDate descending
                          select r).Skip(startIndex).Take(count).ToList();

            return result;
        }

        public int GetNumberOfRatingsBySeller(long sellerId)
        {
            DbSet<SellerRating> ratings = Context.Set<SellerRating>();

            int result = (from r in ratings
                          where r.sellerId == sellerId
                          select r).Count();

            return result;
        }

        public double GetAverageRatingBySeller(long sellerId)
        {
            DbSet<SellerRating> ratings = Context.Set<SellerRating>();

            var sellerRatings = (from r in ratings
                                 where r.sellerId == sellerId
                                 select r);

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

