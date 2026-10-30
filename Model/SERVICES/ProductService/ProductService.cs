using Es.Udc.DotNet.ModelUtil.Transactions;
using Es.Udc.DotNet.PracticaMAD.Model.Cache;
using Es.Udc.DotNet.PracticaMAD.Model.ProductDao;
using Ninject;
using System;
using System.Collections.Generic;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    public class ProductService : IProductService
    {
        [Inject]
        public IProductDao ProductDao { private get; set; }

        /// <exception cref="ArgumentOutOfRangeException"/>
        [SearchCache]
        [Transactional]
        public Block<ProductVO> FindProductByName(string name, int startIndex, int count)
        {
            return FindProductsInternal(name, null, startIndex, count);
        }

        /// <exception cref="ArgumentOutOfRangeException"/>
        [SearchCache]
        [Transactional]
        public Block<ProductVO> FindProducts(string keyword, long? categoryId, int startIndex, int count)
        {
            return FindProductsInternal(keyword, categoryId, startIndex, count);
        }

        private Block<ProductVO> FindProductsInternal(string keyword, long? categoryId, int startIndex, int count)
        {
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            List<Product> products =
                ProductDao.FindByKeywords(keyword, categoryId, startIndex, count + 1);

            bool existMoreItems = (products.Count == count + 1);

            if (existMoreItems)
            {
                products.RemoveAt(count);
            }

            List<ProductVO> productDetails = new List<ProductVO>(products.Count);

            foreach (Product product in products)
            {
                productDetails.Add(new ProductVO(
                    product.productName,
                    product.Category.categoryName,
                    product.creationDate,
                    product.sellerId,
                    product.UserProfile.loginName
                ));
            }

            return new Block<ProductVO>(
                productDetails, existMoreItems);
        }
    }
}
