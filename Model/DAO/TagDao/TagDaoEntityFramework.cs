using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Es.Udc.DotNet.ModelUtil.Dao;
using Es.Udc.DotNet.ModelUtil.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.TagDao
{
    public class TagDaoEntityFramework : GenericDaoEntityFramework<Tag, long>, ITagDao
    {
        public bool ExistsByName(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return false;
            }

            DbSet<Tag> tags = Context.Set<Tag>();
            string lowerName = tagName.Trim().ToLower();

            return (from t in tags
                    where t.tagName.ToLower() == lowerName
                    select t).Any();
        }

        /// <summary>
        /// Busca una etiqueta por su nombre exacto (case-insensitive).
        /// </summary>
        /// <param name="tagName">Nombre de la etiqueta.</param>
        /// <returns>La etiqueta encontrada.</returns>
        /// <exception cref="InstanceNotFoundException"/>
        public Tag FindByName(string tagName)
        {
            DbSet<Tag> tags = Context.Set<Tag>();
            string lowerName = string.IsNullOrWhiteSpace(tagName) ? string.Empty : tagName.Trim().ToLower();

            Tag tag = (from t in tags
                       where t.tagName.ToLower() == lowerName
                       select t).FirstOrDefault();

            if (tag == null)
            {
                throw new InstanceNotFoundException(tagName, typeof(Tag).FullName);
            }

            return tag;
        }

        public List<Tag> FindAll()
        {
            DbSet<Tag> tags = Context.Set<Tag>();

            return (from t in tags
                    orderby t.tagName ascending
                    select t).ToList();
        }

        public Dictionary<Tag, int> GetTagCloud()
        {
            DbSet<Tag> tags = Context.Set<Tag>();

            var list = (from t in tags.Include(x => x.Comments)
                        orderby t.tagName ascending
                        select t).ToList();

            var result = new Dictionary<Tag, int>();

            foreach (var tag in list)
            {
                result.Add(tag, tag.Comments != null ? tag.Comments.Count : 0);
            }

            return result;
        }
    }
}

