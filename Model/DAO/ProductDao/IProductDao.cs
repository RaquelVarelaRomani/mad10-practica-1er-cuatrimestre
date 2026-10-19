using System.Collections.Generic;
using Es.Udc.DotNet.ModelUtil.Dao;

namespace Es.Udc.DotNet.PracticaMAD.Model.ProductDao
{
    public interface IProductDao : IGenericDao<Product, long>
    {
        /// <summary>
        /// Busca productos que contengan la palabra clave en su nombre, opcionalmente filtrados por categoria, de forma paginada.
        /// </summary>
        /// <param name="keyword">Palabra clave contenida en el nombre del producto.</param>
        /// <param name="categoryId">Identificador de la categoria (opcional, null para todas).</param>
        /// <param name="startIndex">Indice de inicio para paginacion.</param>
        /// <param name="count">Cantidad maxima de productos a recuperar.</param>
        /// <returns>Lista de productos encontrados.</returns>
        List<Product> FindByKeywords(string keyword, long? categoryId, int startIndex, int count);

        /// <summary>
        /// Obtiene el numero total de productos que coinciden con la palabra clave y categoria.
        /// </summary>
        /// <param name="keyword">Palabra clave contenida en el nombre.</param>
        /// <param name="categoryId">Identificador de la categoria (opcional, null para todas).</param>
        /// <returns>Numero total de productos coincidentes.</returns>
        int GetNumberOfProductsByKeywords(string keyword, long? categoryId);

        /// <summary>
        /// Busca los productos puestos a la venta por un vendedor concreto.
        /// </summary>
        /// <param name="sellerId">Identificador del vendedor.</param>
        /// <param name="startIndex">Indice de inicio para paginacion.</param>
        /// <param name="count">Cantidad maxima de productos a recuperar.</param>
        /// <returns>Lista de productos del vendedor.</returns>
        List<Product> FindBySellerId(long sellerId, int startIndex, int count);
    }
}

