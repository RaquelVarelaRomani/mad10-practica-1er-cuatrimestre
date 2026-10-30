using System;
using System.Collections.Generic;
using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.ModelUtil.Transactions;
using Es.Udc.DotNet.PracticaMAD.Model.Exceptions;
using PagedList;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    public interface ICommentService
    {
        /// <summary>
        /// Crea un comentario en un producto con las etiquetas indicadas.
        /// Crea las etiquetas que no existan.
        /// </summary>
        /// <param name="userId">Identificador del usuario.</param>
        /// <param name="productId">Identificador del producto.</param>
        /// <param name="text">Texto del comentario.</param>
        /// <param name="tagNames">Nombres de las etiquetas.</param>
        /// <returns>Identificador del comentario creado.</returns>
        /// <exception cref="InstanceNotFoundException"/>
        /// <exception cref="ArgumentException"/>
        [Transactional]
        long AddComment(long userId, long productId,
            string text, List<string> tagNames);

        /// <summary>
        /// Modifica el texto y las etiquetas de un comentario propio.
        /// </summary>
        /// <param name="commentId">Identificador del comentario.</param>
        /// <param name="userId">Identificador del usuario.</param>
        /// <param name="newText">Nuevo texto del comentario.</param>
        /// <param name="tagNames">Nombres de las etiquetas.</param>
        /// <exception cref="InstanceNotFoundException"/>
        /// <exception cref="NotCommentAuthorException"/>
        /// <exception cref="CommentInReviewException"/>
        /// <exception cref="ArgumentException"/>
        [Transactional]
        void UpdateComment(long commentId, long userId,
            string newText, List<string> tagNames);

        /// <summary>
        /// Elimina un comentario si pertenece al usuario.
        /// </summary>
        /// <param name="commentId">Identificador del comentario.</param>
        /// <param name="userId">Identificador del usuario.</param>
        /// <exception cref="InstanceNotFoundException"/>
        /// <exception cref="NotCommentAuthorException"/>
        [Transactional]
        void DeleteComment(long commentId, long userId);

        /// <summary>
        /// Permite reportar un comentario indicando un motivo.
        /// Cada usuario solo puede reportarlo una vez.
        /// </summary>
        /// <param name="commentId">Identificador del comentario.</param>
        /// <param name="userId">Identificador del usuario.</param>
        /// <param name="reason">Motivo del reporte.</param>
        /// <exception cref="InstanceNotFoundException"/>
        /// <exception cref="CommentAlreadyReportedException"/>
        /// <exception cref="ArgumentException"/>
        [Transactional]
        void ReportComment(long commentId, long userId, string reason);

        /// <summary>
        /// Obtiene los datos de un comentario.
        /// </summary>
        /// <param name="commentId">Identificador del comentario.</param>
        /// <returns>Datos del comentario.</returns>
        /// <exception cref="InstanceNotFoundException"/>
        [Transactional]
        CommentVO FindCommentById(long commentId);

        /// <summary>
        /// Obtiene los comentarios de un usuario, paginados.
        /// </summary>
        /// <param name="userId">Identificador del usuario.</param>
        /// <param name="page">Numero de pagina (empezando en 1).</param>
        /// <param name="pageSize">Comentarios por pagina.</param>
        /// <returns>Lista paginada de comentarios.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [Transactional]
        IPagedList<CommentVO> FindCommentsByUser(
            long userId, int page, int pageSize);

        /// <summary>
        /// Obtiene los comentarios de un producto, ordenados
        /// por fecha descendente y paginados.
        /// </summary>
        /// <param name="productId">Identificador del producto.</param>
        /// <param name="page">Numero de pagina (empezando en 1).</param>
        /// <param name="pageSize">Comentarios por pagina.</param>
        /// <returns>Lista paginada de comentarios.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [Transactional]
        IPagedList<CommentVO> FindCommentsByProduct(
            long productId, int page, int pageSize);

        /// <summary>
        /// Obtiene los comentarios asociados a una etiqueta.
        /// </summary>
        /// <param name="tagId">Identificador de la etiqueta.</param>
        /// <param name="page">Numero de pagina (empezando en 1).</param>
        /// <param name="pageSize">Comentarios por pagina.</param>
        /// <returns>Lista paginada de comentarios.</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [Transactional]
        IPagedList<CommentVO> FindCommentsByTagId(
            long tagId, int page, int pageSize);

        /// <summary>
        /// Obtiene el numero total de comentarios de un producto.
        /// </summary>
        /// <param name="productId">Identificador del producto.</param>
        /// <returns>Numero de comentarios.</returns>
        [Transactional]
        int GetNumberOfCommentsByProduct(long productId);
    }
}
