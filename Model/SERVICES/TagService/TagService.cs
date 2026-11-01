using System;
using System.Collections.Generic;
using Es.Udc.DotNet.ModelUtil.Transactions;
using Es.Udc.DotNet.PracticaMAD.Model.TagDao;
using Es.Udc.DotNet.PracticaMAD.Model.CommentDao;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    public class TagService : ITagService
    {
        private readonly ITagDao tagDao;
        private readonly ICommentDao commentDao;

        public TagService(ITagDao tagDao, ICommentDao commentDao)
        {
            this.tagDao = tagDao;
            this.commentDao = commentDao;
        }

        /// <summary>
        /// Obtiene las etiquetas mas utilizadas, paginadas.
        /// </summary>
        [Transactional]
        public Block<Tag> GetMostUsedTags(
            int maxTags, int startIndex, int count)
        {
            if (maxTags <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxTags));

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            // No se devuelven etiquetas fuera del limite maxTags.
            if (startIndex >= maxTags)
                return new Block<Tag>(new List<Tag>(), false);

            int remaining = maxTags - startIndex;
            int fetchCount = count < remaining ? count + 1 : remaining;
            List<Tag> tags = tagDao.GetMostUsedTags(startIndex, fetchCount);

            bool existsMoreItems = tags.Count > count;
            if (existsMoreItems)
                tags.RemoveAt(count);

            return new Block<Tag>(tags, existsMoreItems);
        }

        /// <summary>
        /// Obtiene todas las etiquetas con su frecuencia de uso.
        /// </summary>
        [Transactional]
        public List<TagVO> GetAllTags()
        {
            List<TagVO> result = new List<TagVO>();

            List<Tag> tags = tagDao.FindAll();

            foreach (Tag tag in tags)
            {
                int frequency =
                    commentDao.GetNumberOfCommentsByTag(tag.tagId);

                TagVO tagVO = new TagVO(
                    tag.tagId,
                    tag.tagName,
                    frequency
                );

                result.Add(tagVO);
            }

            return result;
        }
    }
}
