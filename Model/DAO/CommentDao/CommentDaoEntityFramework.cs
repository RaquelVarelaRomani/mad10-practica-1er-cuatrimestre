using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Es.Udc.DotNet.ModelUtil.Dao;
using PagedList;

namespace Es.Udc.DotNet.PracticaMAD.Model.CommentDao
{
    public class CommentDaoEntityFramework : GenericDaoEntityFramework<Comment, long>, ICommentDao
    {
        public IPagedList<Comment> FindByProductId(long productId, int page, int pageSize)
        {
            ValidatePaging(page, pageSize);

            return Context.Set<Comment>()
                .Where(c => c.productId == productId)
                .OrderByDescending(c => c.commentDate)
                .ThenByDescending(c => c.commentId)
                .ToPagedList(page, pageSize);
        }

        public IPagedList<Comment> FindByUser(long userId, int page, int pageSize)
        {
            ValidatePaging(page, pageSize);

            return Context.Set<Comment>()
                .Where(c => c.userId == userId)
                .OrderByDescending(c => c.commentDate)
                .ThenByDescending(c => c.commentId)
                .ToPagedList(page, pageSize);
        }

        public int GetNumberOfCommentsByProduct(long productId)
        {
            return Context.Set<Comment>().Count(c => c.productId == productId);
        }

        public IPagedList<Comment> FindByTagId(long tagId, int page, int pageSize)
        {
            ValidatePaging(page, pageSize);

            return Context.Set<Comment>()
                .Where(c => c.Tags.Any(t => t.tagId == tagId))
                .OrderByDescending(c => c.commentDate)
                .ThenByDescending(c => c.commentId)
                .ToPagedList(page, pageSize);
        }

        public int GetNumberOfCommentsByTag(long tagId)
        {
            return Context.Set<Comment>().Count(c => c.Tags.Any(t => t.tagId == tagId));
        }

        /// <summary>
        /// Actualiza CommentTag mediante comandos parametrizados. Evita
        /// inconsistencias del change tracker en asociaciones ya cargadas.
        /// </summary>
        public void ReplaceCommentTags(long commentId, ICollection<long> tagIds)
        {
            if (tagIds == null)
                throw new ArgumentNullException(nameof(tagIds));

            Context.Database.ExecuteSqlCommand(
                "DELETE FROM [dbo].[CommentTag] WHERE [commentId] = @p0", commentId);

            foreach (long tagId in tagIds.Distinct())
            {
                Context.Database.ExecuteSqlCommand(
                    "INSERT INTO [dbo].[CommentTag] ([commentId], [tagId]) VALUES (@p0, @p1)",
                    commentId, tagId);
            }
        }

        /// <summary>Lee el estado real de CommentTag y Tag desde la base de datos.</summary>
        public List<Tag> FindTagsByCommentId(long commentId)
        {
            return Context.Database.SqlQuery<Tag>(
                @"SELECT t.[tagId], t.[tagName]
                  FROM [dbo].[Tag] AS t
                  INNER JOIN [dbo].[CommentTag] AS ct ON ct.[tagId] = t.[tagId]
                  WHERE ct.[commentId] = @p0", commentId).ToList();
        }

        private static void ValidatePaging(int page, int pageSize)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));
        }
    }
}
