using System.Collections.Generic;
using Es.Udc.DotNet.ModelUtil.Dao;

namespace Es.Udc.DotNet.PracticaMAD.Model.CommentDao
{
    public interface ICommentDao : IGenericDao<Comment, long>
    {
        /// <summary>
        /// Obtiene los comentarios de un producto ordenados por fecha descendente y paginados.
        /// </summary>
        /// <param name="productId">Identificador del producto.</param>
        /// <param name="startIndex">Posicion inicial, empezando en 0.</param>
        /// <param name="count">Numero maximo de comentarios que se devuelven.</param>
        /// <returns>Lista de comentarios paginados del producto.</returns>
        List<Comment> FindByProductId(long productId, int startIndex, int count);

        /// <summary>
        /// Obtiene los comentarios realizados por un usuario, ordenados por fecha descendente y paginados.
        /// </summary>
        /// <param name="userId">Identificador del usuario.</param>
        /// <param name="startIndex">Posicion inicial, empezando en 0.</param>
        /// <param name="count">Numero maximo de comentarios que se devuelven.</param>
        /// <returns>Lista de comentarios paginados del usuario.</returns>
        List<Comment> FindByUser(long userId, int startIndex, int count);

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
        /// <param name="startIndex">Posicion inicial, empezando en 0.</param>
        /// <param name="count">Numero maximo de comentarios que se devuelven.</param>
        /// <returns>Lista de comentarios paginados con dicha etiqueta.</returns>
        List<Comment> FindByTagId(long tagId, int startIndex, int count);

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