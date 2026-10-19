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

            return comments
                .Where(c => c.productId == productId)
                .OrderByDescending(c => c.commentDate)
                .Skip(startIndex)
                .Take(count)
                .ToList();
        }

        public int GetNumberOfCommentsByProduct(long productId)
        {
            DbSet<Comment> comments = Context.Set<Comment>();

            return comments.Count(c => c.productId == productId);
        }

        public List<Comment> FindByTagId(long tagId, int startIndex, int count)
        {
            DbSet<Comment> comments = Context.Set<Comment>();

            return comments
                .Where(c => c.Tags.Any(t => t.tagId == tagId))
                .OrderByDescending(c => c.commentDate)
                .Skip(startIndex)
                .Take(count)
                .ToList();
        }

        public int GetNumberOfCommentsByTag(long tagId)
        {
            DbSet<Comment> comments = Context.Set<Comment>();

            return comments.Count(c => c.Tags.Any(t => t.tagId == tagId));
        }
    }
}

