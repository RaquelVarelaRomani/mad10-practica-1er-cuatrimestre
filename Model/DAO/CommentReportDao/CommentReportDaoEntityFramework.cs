using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Es.Udc.DotNet.ModelUtil.Dao;
using Es.Udc.DotNet.ModelUtil.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.CommentReportDao
{
    public class CommentReportDaoEntityFramework : GenericDaoEntityFramework<CommentReport, long>, ICommentReportDao
    {
        public bool ExistsByCommentAndUser(long commentId, long userId)
        {
            DbSet<CommentReport> reports = Context.Set<CommentReport>();

            return (from r in reports
                    where r.commentId == commentId && r.userId == userId
                    select r).Any();
        }

        /// <summary>
        /// Busca un reporte emitido por un usuario sobre un comentario concreto.
        /// </summary>
        /// <param name="commentId">Identificador del comentario.</param>
        /// <param name="userId">Identificador del usuario.</param>
        /// <returns>El reporte correspondiente.</returns>
        /// <exception cref="InstanceNotFoundException"/>
        public CommentReport FindByCommentAndUser(long commentId, long userId)
        {
            DbSet<CommentReport> reports = Context.Set<CommentReport>();

            CommentReport report = (from r in reports
                                    where r.commentId == commentId && r.userId == userId
                                    select r).FirstOrDefault();

            if (report == null)
            {
                throw new InstanceNotFoundException(
                    string.Format("commentId={0}, userId={1}", commentId, userId),
                    typeof(CommentReport).FullName);
            }

            return report;
        }

        public int GetNumberOfReportsByComment(long commentId)
        {
            DbSet<CommentReport> reports = Context.Set<CommentReport>();

            int result = (from r in reports
                          where r.commentId == commentId
                          select r).Count();

            return result;
        }

        public List<CommentReport> FindByCommentId(long commentId)
        {
            DbSet<CommentReport> reports = Context.Set<CommentReport>();

            var result = (from r in reports
                          where r.commentId == commentId
                          orderby r.reportDate descending
                          select r).ToList();

            return result;
        }
    }
}

