using Es.Udc.DotNet.ModelUtil.Dao;

namespace Es.Udc.DotNet.PracticaMAD.Model.CommentReportDao
{
    public interface ICommentReportDao : IGenericDao<CommentReport, long>
    {
        /// <summary>
        /// Comprueba si existe ya un reporte emitido por un usuario sobre un comentario concreto.
        /// </summary>
        /// <param name="commentId">Identificador del comentario.</param>
        /// <param name="userId">Identificador del usuario.</param>
        /// <returns>true si ya existe el reporte; false en caso contrario.</returns>
        bool ExistsByCommentAndUser(long commentId, long userId);

        /// <summary>
        /// Obtiene el numero total de reportes que ha recibido un comentario.
        /// </summary>
        /// <param name="commentId">Identificador del comentario.</param>
        /// <returns>Cantidad de reportes acumulados.</returns>
        int GetNumberOfReportsByComment(long commentId);

    }
}

