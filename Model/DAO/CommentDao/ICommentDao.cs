using System.Collections.Generic;
using Es.Udc.DotNet.ModelUtil.Dao;
using PagedList;

namespace Es.Udc.DotNet.PracticaMAD.Model.CommentDao
{
    public interface ICommentDao : IGenericDao<Comment, long>
    {
        /// <summary>
        /// Obtiene los comentarios de un producto ordenados por fecha descendente y paginados.
        /// </summary>
        /// <param name="productId">Identificador del producto.</param>
        /// <param name="page">Numero de pagina (empezando en 1).</param>
        /// <param name="pageSize">Cantidad de comentarios por pagina.</param>
        /// <returns>Lista paginada de comentarios del producto.</returns>
        IPagedList<Comment> FindByProductId(long productId, int page, int pageSize);

        /// <summary>
        /// Obtiene los comentarios realizados por un usuario, ordenados por fecha descendente y paginados.
        /// </summary>
        /// <param name="userId">Identificador del usuario.</param>
        /// <param name="page">Numero de pagina (empezando en 1).</param>
        /// <param name="pageSize">Cantidad de comentarios por pagina.</param>
        /// <returns>Lista paginada de comentarios del usuario.</returns>
        IPagedList<Comment> FindByUser(long userId, int page, int pageSize);

        /// <summary>
        /// Obtiene el numero total de comentarios de un producto.
        /// </summary>
        /// <param name="productId">Identificador del producto.</param>
        /// <returns>Numero de comentarios.</returns>
        int GetNumberOfCommentsByProduct(long productId);

        /// <summary>
        /// Obtiene los comentarios de una etiqueta ordenados por fecha descendente y paginados.
        /// </summary>
        /// <param name="tagId">Identificador de la etiqueta.</param>
        /// <param name="page">Numero de pagina (empezando en 1).</param>
        /// <param name="pageSize">Cantidad de comentarios por pagina.</param>
        /// <returns>Lista paginada de comentarios con dicha etiqueta.</returns>
        IPagedList<Comment> FindByTagId(long tagId, int page, int pageSize);

        /// <summary>
        /// Obtiene el numero total de comentarios asociados a una etiqueta.
        /// </summary>
        /// <param name="tagId">Identificador de la etiqueta.</param>
        /// <returns>Numero de comentarios.</returns>
        int GetNumberOfCommentsByTag(long tagId);

        /// <summary>Reemplaza las asociaciones de etiquetas de un comentario.</summary>
        void ReplaceCommentTags(long commentId, ICollection<long> tagIds);

        /// <summary>Obtiene las etiquetas de un comentario sin usar relaciones en memoria.</summary>
        List<Tag> FindTagsByCommentId(long commentId);
    }
}