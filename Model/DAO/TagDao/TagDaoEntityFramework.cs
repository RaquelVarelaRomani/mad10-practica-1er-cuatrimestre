using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Es.Udc.DotNet.ModelUtil.Dao;

namespace Es.Udc.DotNet.PracticaMAD.Model.TagDao
{
    public class TagDaoEntityFramework : GenericDaoEntityFramework<Tag, long>, ITagDao
    {
        public Tag FindByName(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return null;
            }

            DbSet<Tag> tags = Context.Set<Tag>();
            string lowerName = tagName.Trim().ToLower();

            return (from t in tags
                    where t.tagName.ToLower() == lowerName
                    select t).FirstOrDefault();
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

            var list = tags
                .Include(t => t.Comments)
                .OrderBy(t => t.tagName)
                .ToList();

            var result = new Dictionary<Tag, int>();

            foreach (var tag in list)
            {
                result.Add(tag, tag.Comments != null ? tag.Comments.Count : 0);
            }

            return result;
        }
    }
}

