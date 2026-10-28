using Es.Udc.DotNet.ModelUtil.Transactions;
using Es.Udc.DotNet.PracticaMAD.Model.ProductDao;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    public class ProductService : IProductService
    {
        [Inject]
        public IProductDao ProductDao { private get; set; }

        [Transactional]
        public Block<ProductDetails> FindProductByName(string name, int startIndex, int count)
        {
            List<Product> products =
                ProductDao.FindByKeywords(name, null, startIndex, count + 1);

            bool existMoreItems = (products.Count == count + 1);

            if (existMoreItems)
                products.RemoveAt(count);

            List<ProductDetails> productDetails = new List<ProductDetails>();

            foreach (Product product in products)
            {
                productDetails.Add(new ProductDetails(
                    product.productName,
                    product.Category.categoryName,
                    product.creationDate,
                    product.sellerId,
                    product.UserProfile.loginName
                ));
            }

            return new Block<ProductDetails>(
                productDetails, existMoreItems);
        }
    }
}
