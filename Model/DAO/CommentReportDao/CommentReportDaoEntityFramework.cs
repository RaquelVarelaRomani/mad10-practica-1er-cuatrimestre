using Es.Udc.DotNet.ModelUtil.Dao;
using System.Data.Entity;
using System.Linq;

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

        public int GetNumberOfReportsByComment(long commentId)
        {
            DbSet<CommentReport> reports = Context.Set<CommentReport>();

            int result = (from r in reports
                          where r.commentId == commentId
                          select r).Count();

            return result;
        }
    }
}

