using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    public interface IProductService
    {
        /// <summary>
        /// Finds the product by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns>A pageable list of products</returns>
        Block<ProductVO> FindProductByName(string name, int startIndex, int count);
    }
}
