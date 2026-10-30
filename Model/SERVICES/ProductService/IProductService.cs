using System;
using Es.Udc.DotNet.ModelUtil.Transactions;
using Es.Udc.DotNet.PracticaMAD.Model.Cache;

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
        /// <exception cref="ArgumentOutOfRangeException"/>
        [SearchCache]
        [Transactional]
        Block<ProductVO> FindProductByName(string name, int startIndex, int count);

        /// <summary>
        /// Busca productos de forma paginada por palabras clave en el nombre y,
        /// opcionalmente, filtrando por categoria. Los resultados de las 5 ultimas
        /// consultas realizadas se mantienen y sirven desde la cache en memoria.
        /// </summary>
        /// <param name="keyword">Palabras clave contenidas en el nombre del producto.</param>
        /// <param name="categoryId">Identificador opcional de la categoria (<c>null</c> o &lt;= 0 para todas).</param>
        /// <param name="startIndex">Indice inicial desde el que recuperar productos (base 0).</param>
        /// <param name="count">Numero maximo de productos por pagina.</param>
        /// <returns>Bloque paginado de productos coincidentes.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [SearchCache]
        [Transactional]
        Block<ProductVO> FindProducts(string keyword, long? categoryId, int startIndex, int count);
    }
}
