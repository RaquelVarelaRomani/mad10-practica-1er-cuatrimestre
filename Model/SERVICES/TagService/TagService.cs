using System.Collections.Generic;
using Es.Udc.DotNet.ModelUtil.Transactions;
using Es.Udc.DotNet.PracticaMAD.Model.TagDao;
using Es.Udc.DotNet.PracticaMAD.Model.CommentDao;
using PagedList;

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
        public IPagedList<Tag> GetMostUsedTags(
            int count, int page, int pageSize)
        {
            return tagDao.GetMostUsedTags(count, page, pageSize);
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
