using System;
using System.Collections.Generic;
using System.Linq;
using Es.Udc.DotNet.ModelUtil.Transactions;
using Es.Udc.DotNet.PracticaMAD.Model.CommentDao;
using Es.Udc.DotNet.PracticaMAD.Model.CommentReportDao;
using Es.Udc.DotNet.PracticaMAD.Model.TagDao;
using Es.Udc.DotNet.PracticaMAD.Model.ProductDao;
using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;
using Es.Udc.DotNet.PracticaMAD.Model.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    public class CommentService : ICommentService
    {
        // Ajustar si habeis establecido otro umbral para la practica.
        private const int ReportThreshold = 3;

        private readonly ICommentDao commentDao;
        private readonly ICommentReportDao commentReportDao;
        private readonly ITagDao tagDao;
        private readonly IProductDao productDao;
        private readonly IUserProfileDao userProfileDao;

        public CommentService(
            ICommentDao commentDao,
            ICommentReportDao commentReportDao,
            ITagDao tagDao,
            IProductDao productDao,
            IUserProfileDao userProfileDao)
        {
            this.commentDao = commentDao;
            this.commentReportDao = commentReportDao;
            this.tagDao = tagDao;
            this.productDao = productDao;
            this.userProfileDao = userProfileDao;
        }

        /// <summary>
        /// Crea un comentario en un producto con sus etiquetas.
        /// </summary>
        [Transactional]
        public long AddComment(long userId, long productId,
            string text, List<string> tagNames)
        {
            string validText = ValidateCommentText(text);
            List<string> names = NormalizeTagNames(tagNames);

            userProfileDao.Find(userId);
            productDao.Find(productId);

            Comment comment = new Comment
            {
                userId = userId,
                productId = productId,
                commentText = validText,
                commentDate = DateTime.Now,
                isInReview = false
            };

            if (comment.Tags == null)
                comment.Tags = new HashSet<Tag>();

            foreach (string name in names)
                comment.Tags.Add(GetOrCreateTag(name));

            commentDao.Create(comment);
            return comment.commentId;
        }

        /// <summary>
        /// Modifica el texto y las etiquetas de un comentario propio.
        /// Las asociaciones se actualizan mediante el DAO para evitar conflictos de seguimiento.
        /// </summary>
        [Transactional]
        public void UpdateComment(long commentId, long userId,
            string newText, List<string> tagNames)
        {
            Comment comment = commentDao.Find(commentId);

            if (comment.userId != userId)
                throw new NotCommentAuthorException(commentId, userId);

            if (comment.isInReview)
                throw new CommentInReviewException(commentId);

            string validText = ValidateCommentText(newText);
            List<string> names = NormalizeTagNames(tagNames);

            List<Tag> desiredTags = names.Select(GetOrCreateTag).ToList();

            comment.commentText = validText;
            commentDao.Update(comment);

            // Se reemplazan las filas CommentTag desde el DAO. No se modifica
            // comment.Tags porque puede conservar relaciones antiguas en EF6.
            commentDao.ReplaceCommentTags(commentId,
                desiredTags.Select(t => t.tagId).ToList());
        }

        /// <summary>
        /// Elimina un comentario propio, sus relaciones y sus reportes.
        /// Las etiquetas del catalogo permanecen en la base de datos.
        /// </summary>
        [Transactional]
        public void DeleteComment(long commentId, long userId)
        {
            Comment comment = commentDao.Find(commentId);

            if (comment.userId != userId)
                throw new NotCommentAuthorException(commentId, userId);

            commentDao.Remove(commentId);
        }

        /// <summary>
        /// Registra el reporte y marca el comentario en revision al alcanzar el umbral.
        /// </summary>
        [Transactional]
        public void ReportComment(long commentId, long userId, string reason)
        {
            Comment comment = commentDao.Find(commentId);
            userProfileDao.Find(userId);
            string validReason = ValidateReportReason(reason);

            if (commentReportDao.ExistsByCommentAndUser(commentId, userId))
                throw new CommentAlreadyReportedException(commentId, userId);

            int existingReports =
                commentReportDao.GetNumberOfReportsByComment(commentId);

            CommentReport report = new CommentReport
            {
                commentId = commentId,
                userId = userId,
                reason = validReason,
                reportDate = DateTime.Now
            };
            commentReportDao.Create(report);

            if (!comment.isInReview && existingReports + 1 >= ReportThreshold)
            {
                comment.isInReview = true;
                commentDao.Update(comment);
            }
        }

        /// <summary>Obtiene un comentario como VO.</summary>
        [Transactional]
        public CommentVO FindCommentById(long commentId)
        {
            return ToCommentVO(commentDao.Find(commentId));
        }

        /// <summary>Obtiene los comentarios paginados de un usuario.</summary>
        [Transactional]
        public Block<CommentVO> FindCommentsByUser(long userId, int startIndex, int count)
        {
            ValidatePaging(startIndex, count);
            return ConvertToBlock(commentDao.FindByUser(userId, startIndex, count + 1), count);
        }

        /// <summary>Obtiene los comentarios paginados de un producto.</summary>
        [Transactional]
        public Block<CommentVO> FindCommentsByProduct(long productId, int startIndex, int count)
        {
            ValidatePaging(startIndex, count);
            return ConvertToBlock(commentDao.FindByProductId(productId, startIndex, count + 1), count);
        }

        /// <summary>Obtiene los comentarios paginados de una etiqueta.</summary>
        [Transactional]
        public Block<CommentVO> FindCommentsByTagId(long tagId, int startIndex, int count)
        {
            ValidatePaging(startIndex, count);
            return ConvertToBlock(commentDao.FindByTagId(tagId, startIndex, count + 1), count);
        }

        /// <summary>Cuenta los comentarios de un producto.</summary>
        [Transactional]
        public int GetNumberOfCommentsByProduct(long productId)
        {
            return commentDao.GetNumberOfCommentsByProduct(productId);
        }

        /// <summary>Busca una etiqueta o la crea si no existe.</summary>
        private Tag GetOrCreateTag(string name)
        {
            if (tagDao.ExistsByName(name))
                return tagDao.FindByName(name);

            Tag tag = new Tag { tagName = name };
            tagDao.Create(tag);
            return tag;
        }

        /// <summary>Normaliza y valida los nombres de las etiquetas.</summary>
        private static List<string> NormalizeTagNames(IEnumerable<string> tagNames)
        {
            List<string> names = (tagNames ?? Enumerable.Empty<string>())
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (names.Any(n => n.Length > 50))
                throw new ArgumentException(
                    "Los nombres de etiquetas no pueden superar 50 caracteres.",
                    nameof(tagNames));

            return names;
        }

        /// <summary>Valida el texto del comentario.</summary>
        private static string ValidateCommentText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException(
                    "El comentario no puede estar vacio.", nameof(text));

            string validText = text.Trim();
            if (validText.Length > 1000)
                throw new ArgumentException(
                    "El comentario no puede superar los 1000 caracteres.",
                    nameof(text));

            return validText;
        }

        /// <summary>Valida el motivo de un reporte.</summary>
        private static string ValidateReportReason(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException(
                    "El motivo del reporte no puede estar vacio.",
                    nameof(reason));

            string validReason = reason.Trim();
            if (validReason.Length > 255)
                throw new ArgumentException(
                    "El motivo no puede superar los 255 caracteres.",
                    nameof(reason));

            return validReason;
        }

        /// <summary>Convierte la entidad en VO usando las etiquetas persistidas.</summary>
        private CommentVO ToCommentVO(Comment comment)
        {
            UserProfile user = userProfileDao.Find(comment.userId);

            List<TagVO> tags = commentDao.FindTagsByCommentId(comment.commentId)
                .Select(t => new TagVO(t.tagId, t.tagName))
                .ToList();

            return new CommentVO(
                comment.commentId,
                comment.userId,
                user.loginName,
                comment.productId,
                comment.commentText,
                comment.commentDate,
                comment.isInReview,
                tags);
        }

        /// <summary>
        /// Convierte las entidades en un bloque de VOs y comprueba si hay mas resultados.
        /// </summary>
        private Block<CommentVO> ConvertToBlock(List<Comment> comments, int count)
        {
            bool existsMoreItems = comments.Count > count;
            if (existsMoreItems)
                comments.RemoveAt(count);

            List<CommentVO> commentVOs = comments.Select(ToCommentVO).ToList();
            return new Block<CommentVO>(commentVOs, existsMoreItems);
        }

        private static void ValidatePaging(int startIndex, int count)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));
        }
    }
}
