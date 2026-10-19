using System.Collections.Generic;
using Es.Udc.DotNet.ModelUtil.Dao;

namespace Es.Udc.DotNet.PracticaMAD.Model.SellerRatingDao
{
    public interface ISellerRatingDao : IGenericDao<SellerRating, long>
    {
        /// <summary>
        /// Obtiene las valoraciones recibidas por un vendedor ordenadas cronologicamente por fecha descendente, paginadas.
        /// </summary>
        /// <param name="sellerId">Identificador del vendedor.</param>
        /// <param name="startIndex">Indice inicial.</param>
        /// <param name="count">Cantidad maxima a recuperar.</param>
        /// <returns>Lista de valoraciones recibidas.</returns>
        List<SellerRating> FindBySellerId(long sellerId, int startIndex, int count);

        /// <summary>
        /// Obtiene el numero total de valoraciones recibidas por un vendedor.
        /// </summary>
        /// <param name="sellerId">Identificador del vendedor.</param>
        /// <returns>Numero total de valoraciones.</returns>
        int GetNumberOfRatingsBySeller(long sellerId);

        /// <summary>
        /// Calcula la media aritmetica de las puntuaciones obtenidas por un vendedor (0.0 si no tiene valoraciones).
        /// </summary>
        /// <param name="sellerId">Identificador del vendedor.</param>
        /// <returns>Media de puntuacion.</returns>
        double GetAverageRatingBySeller(long sellerId);

        /// <summary>
        /// Busca si un comprador ya ha valorado al vendedor con relacion a un producto concreto.
        /// </summary>
        /// <param name="sellerId">Identificador del vendedor.</param>
        /// <param name="productId">Identificador del producto.</param>
        /// <param name="buyerUserId">Identificador del comprador.</param>
        /// <returns>La valoracion previa si existe, o null en caso contrario.</returns>
        SellerRating FindBySellerAndProductAndBuyer(long sellerId, long productId, long buyerUserId);
    }
}

