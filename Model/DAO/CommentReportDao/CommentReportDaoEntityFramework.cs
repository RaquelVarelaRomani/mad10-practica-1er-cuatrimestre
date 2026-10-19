using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Es.Udc.DotNet.ModelUtil.Dao;

namespace Es.Udc.DotNet.PracticaMAD.Model.CommentReportDao
{
    public class CommentReportDaoEntityFramework : GenericDaoEntityFramework<CommentReport, long>, ICommentReportDao
    {
        public CommentReport FindByCommentAndUser(long commentId, long userId)
        {
            DbSet<CommentReport> reports = Context.Set<CommentReport>();

            return (from r in reports
                    where r.commentId == commentId && r.userId == userId
                    select r).FirstOrDefault();
        }

        public int GetNumberOfReportsByComment(long commentId)
        {
            DbSet<CommentReport> reports = Context.Set<CommentReport>();

            return reports.Count(r => r.commentId == commentId);
        }

        public List<CommentReport> FindByCommentId(long commentId)
        {
            DbSet<CommentReport> reports = Context.Set<CommentReport>();

            return reports
                .Where(r => r.commentId == commentId)
                .OrderByDescending(r => r.reportDate)
                .ToList();
        }
    }
}

