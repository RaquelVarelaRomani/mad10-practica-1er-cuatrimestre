using System.Collections.Generic;
using Es.Udc.DotNet.ModelUtil.Dao;

namespace Es.Udc.DotNet.PracticaMAD.Model.CommentReportDao
{
    public interface ICommentReportDao : IGenericDao<CommentReport, long>
    {
        /// <summary>
        /// Busca si existe ya un reporte emitido por un usuario sobre un comentario concreto.
        /// </summary>
        /// <param name="commentId">Identificador del comentario.</param>
        /// <param name="userId">Identificador del usuario.</param>
        /// <returns>El reporte si existe, o null en caso contrario.</returns>
        CommentReport FindByCommentAndUser(long commentId, long userId);

        /// <summary>
        /// Obtiene el numero total de reportes que ha recibido un comentario.
        /// </summary>
        /// <param name="commentId">Identificador del comentario.</param>
        /// <returns>Cantidad de reportes acumulados.</returns>
        int GetNumberOfReportsByComment(long commentId);

        /// <summary>
        /// Obtiene la lista de reportes asociados a un comentario.
        /// </summary>
        /// <param name="commentId">Identificador del comentario.</param>
        /// <returns>Lista de reportes del comentario.</returns>
        List<CommentReport> FindByCommentId(long commentId);
    }
}

