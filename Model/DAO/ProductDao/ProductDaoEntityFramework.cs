using System;
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

            var query = products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string lowerKeyword = keyword.Trim().ToLower();
                query = query.Where(p => p.productName.ToLower().Contains(lowerKeyword));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.categoryId == categoryId.Value);
            }

            return query
                .OrderByDescending(p => p.creationDate)
                .ThenBy(p => p.productName)
                .Skip(startIndex)
                .Take(count)
                .ToList();
        }

        public int GetNumberOfProductsByKeywords(string keyword, long? categoryId)
        {
            DbSet<Product> products = Context.Set<Product>();

            var query = products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string lowerKeyword = keyword.Trim().ToLower();
                query = query.Where(p => p.productName.ToLower().Contains(lowerKeyword));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.categoryId == categoryId.Value);
            }

            return query.Count();
        }

        public List<Product> FindBySellerId(long sellerId, int startIndex, int count)
        {
            DbSet<Product> products = Context.Set<Product>();

            return products
                .Where(p => p.sellerId == sellerId)
                .OrderByDescending(p => p.creationDate)
                .Skip(startIndex)
                .Take(count)
                .ToList();
        }
    }
}

