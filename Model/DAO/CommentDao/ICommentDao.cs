using System.Collections.Generic;
using Es.Udc.DotNet.ModelUtil.Dao;

namespace Es.Udc.DotNet.PracticaMAD.Model.CommentDao
{
    public interface ICommentDao : IGenericDao<Comment, long>
    {
        /// <summary>
        /// Obtiene los comentarios de un producto ordenados por fecha descendente (mas recientes primero), paginados.
        /// </summary>
        /// <param name="productId">Identificador del producto.</param>
        /// <param name="startIndex">Indice inicial.</param>
        /// <param name="count">Cantidad maxima a recuperar.</param>
        /// <returns>Lista de comentarios del producto.</returns>
        List<Comment> FindByProductId(long productId, int startIndex, int count);

        /// <summary>
        /// Obtiene el numero total de comentarios de un producto.
        /// </summary>
        /// <param name="productId">Identificador del producto.</param>
        /// <returns>Numero de comentarios.</returns>
        int GetNumberOfCommentsByProduct(long productId);

        /// <summary>
        /// Obtiene los comentarios asociados a una etiqueta ordenados por fecha descendente, paginados.
        /// </summary>
        /// <param name="tagId">Identificador de la etiqueta.</param>
        /// <param name="startIndex">Indice inicial.</param>
        /// <param name="count">Cantidad maxima a recuperar.</param>
        /// <returns>Lista de comentarios con dicha etiqueta.</returns>
        List<Comment> FindByTagId(long tagId, int startIndex, int count);

        /// <summary>
        /// Obtiene el numero total de comentarios asociados a una etiqueta.
        /// </summary>
        /// <param name="tagId">Identificador de la etiqueta.</param>
        /// <returns>Numero de comentarios.</returns>
        int GetNumberOfCommentsByTag(long tagId);
    }
}

