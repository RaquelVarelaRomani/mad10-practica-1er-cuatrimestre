using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Es.Udc.DotNet.ModelUtil.Dao;

namespace Es.Udc.DotNet.PracticaMAD.Model.CommentDao
{
    public class CommentDaoEntityFramework : GenericDaoEntityFramework<Comment, long>, ICommentDao
    {
        public List<Comment> FindByProductId(long productId, int startIndex, int count)
        {
            DbSet<Comment> comments = Context.Set<Comment>();

            var result = (from c in comments
                          where c.productId == productId
                          orderby c.commentDate descending
                          select c).Skip(startIndex).Take(count).ToList();

            return result;
        }

        public int GetNumberOfCommentsByProduct(long productId)
        {
            DbSet<Comment> comments = Context.Set<Comment>();

            int result = (from c in comments
                          where c.productId == productId
                          select c).Count();

            return result;
        }

        public List<Comment> FindByTagId(long tagId, int startIndex, int count)
        {
            DbSet<Comment> comments = Context.Set<Comment>();

            var result = (from c in comments
                          where c.Tags.Any(t => t.tagId == tagId)
                          orderby c.commentDate descending
                          select c).Skip(startIndex).Take(count).ToList();

            return result;
        }

        public int GetNumberOfCommentsByTag(long tagId)
        {
            DbSet<Comment> comments = Context.Set<Comment>();

            int result = (from c in comments
                          where c.Tags.Any(t => t.tagId == tagId)
                          select c).Count();

            return result;
        }
    }
}

