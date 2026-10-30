using System;
using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.ModelUtil.Transactions;
using Es.Udc.DotNet.PracticaMAD.Model.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model.ProductDao;
using Es.Udc.DotNet.PracticaMAD.Model.SellerRatingDao;
using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;
using Ninject;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    /// <summary>
    /// Servicio de negocio para la valoracion de vendedores y consulta de sus opiniones.
    /// </summary>
    public interface ISellerService
    {
        [Inject]
        ISellerRatingDao SellerRatingDao { set; }

        [Inject]
        IProductDao ProductDao { set; }

        [Inject]
        IUserProfileDao UserProfileDao { set; }

        /// <summary>
        /// Valora de 1 a 5 a un vendedor con relacion a uno de sus productos ofrecidos,
        /// pudiendo anadir opcionalmente un comentario justificativo.
        /// </summary>
        /// <param name="buyerUserId">Identificador del usuario que realiza la valoracion.</param>
        /// <param name="productId">Identificador del producto ofrecido por el vendedor.</param>
        /// <param name="score">Voto otorgado al vendedor (entre 1 y 5, ambos inclusive).</param>
        /// <param name="comment">Comentario justificativo opcional.</param>
        /// <returns>Identificador de la valoracion creada.</returns>
        /// <exception cref="InstanceNotFoundException"/>
        /// <exception cref="SelfRatingException"/>
        /// <exception cref="InvalidRatingScoreException"/>
        /// <exception cref="DuplicateInstanceException"/>
        /// <exception cref="ArgumentException"/>
        [Transactional]
        long RateSeller(long buyerUserId, long productId, int score, string comment = null);

        /// <summary>
        /// Obtiene el resumen de opiniones (numero total de valoraciones recibidas y media)
        /// de un vendedor a partir de su seudonimo.
        /// </summary>
        /// <param name="sellerLoginName">Seudonimo (loginName) del vendedor.</param>
        /// <returns>Resumen con el total y la media de valoraciones recibidas.</returns>
        /// <exception cref="InstanceNotFoundException"/>
        [Transactional]
        SellerRatingSummaryVO GetSellerRatingSummary(string sellerLoginName);

        /// <summary>
        /// Obtiene el resumen de opiniones (numero total de valoraciones recibidas y media)
        /// de un vendedor a partir de su identificador.
        /// </summary>
        /// <param name="sellerId">Identificador del vendedor.</param>
        /// <returns>Resumen con el total y la media de valoraciones recibidas.</returns>
        /// <exception cref="InstanceNotFoundException"/>
        [Transactional]
        SellerRatingSummaryVO GetSellerRatingSummary(long sellerId);

        /// <summary>
        /// Obtiene de forma paginada las valoraciones recibidas por un vendedor a partir
        /// de su seudonimo, ordenadas por fecha (primero las mas recientes).
        /// </summary>
        /// <param name="sellerLoginName">Seudonimo (loginName) del vendedor.</param>
        /// <param name="startIndex">Indice inicial desde el que recuperar elementos (base 0).</param>
        /// <param name="count">Numero maximo de valoraciones a recuperar en la pagina.</param>
        /// <returns>Bloque paginado de valoraciones del vendedor.</returns>
        /// <exception cref="InstanceNotFoundException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [Transactional]
        Block<SellerRatingVO> FindSellerRatings(string sellerLoginName, int startIndex, int count);

        /// <summary>
        /// Obtiene de forma paginada las valoraciones recibidas por un vendedor a partir
        /// de su identificador, ordenadas por fecha (primero las mas recientes).
        /// </summary>
        /// <param name="sellerId">Identificador del vendedor.</param>
        /// <param name="startIndex">Indice inicial desde el que recuperar elementos (base 0).</param>
        /// <param name="count">Numero maximo de valoraciones a recuperar en la pagina.</param>
        /// <returns>Bloque paginado de valoraciones del vendedor.</returns>
        /// <exception cref="InstanceNotFoundException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [Transactional]
        Block<SellerRatingVO> FindSellerRatings(long sellerId, int startIndex, int count);

        /// <summary>
        /// Recupera los detalles de una valoracion concreta a partir de su identificador.
        /// </summary>
        /// <param name="ratingId">Identificador de la valoracion.</param>
        /// <returns>Datos detallados de la valoracion.</returns>
        /// <exception cref="InstanceNotFoundException"/>
        [Transactional]
        SellerRatingVO FindSellerRatingById(long ratingId);
    }
}

