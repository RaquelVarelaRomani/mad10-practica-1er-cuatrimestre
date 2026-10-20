using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Es.Udc.DotNet.ModelUtil.Dao;

namespace Es.Udc.DotNet.PracticaMAD.Model.ProductDao
{
    public class ProductDaoEntityFramework : GenericDaoEntityFramework<Product, long>, IProductDao
    {
        public List<Product> FindByKeywords(string keyword, long? categoryId, int startIndex, int count)
        {
            DbSet<Product> products = Context.Set<Product>();

            string lowerKeyword = string.IsNullOrWhiteSpace(keyword) ? string.Empty : keyword.Trim().ToLower();

            var result = (from p in products
                          where (lowerKeyword == string.Empty || p.productName.ToLower().Contains(lowerKeyword))
                             && (!categoryId.HasValue || categoryId.Value <= 0 || p.categoryId == categoryId.Value)
                          orderby p.creationDate descending, p.productName ascending
                          select p).Skip(startIndex).Take(count).ToList();

            return result;
        }

        public int GetNumberOfProductsByKeywords(string keyword, long? categoryId)
        {
            DbSet<Product> products = Context.Set<Product>();

            string lowerKeyword = string.IsNullOrWhiteSpace(keyword) ? string.Empty : keyword.Trim().ToLower();

            int result = (from p in products
                          where (lowerKeyword == string.Empty || p.productName.ToLower().Contains(lowerKeyword))
                             && (!categoryId.HasValue || categoryId.Value <= 0 || p.categoryId == categoryId.Value)
                          select p).Count();

            return result;
        }

        public List<Product> FindBySellerId(long sellerId, int startIndex, int count)
        {
            DbSet<Product> products = Context.Set<Product>();

            var result = (from p in products
                          where p.sellerId == sellerId
                          orderby p.creationDate descending
                          select p).Skip(startIndex).Take(count).ToList();

            return result;
        }
    }
}

