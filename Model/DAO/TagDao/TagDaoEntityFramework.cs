using System;
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

        public Tag FindByName(string tagName)
        {
            DbSet<Tag> tags = Context.Set<Tag>();

            string lowerName = string.IsNullOrWhiteSpace(tagName)
                ? string.Empty : tagName.Trim().ToLower();

            Tag tag = (from t in tags
                       where t.tagName.ToLower() == lowerName
                       select t).FirstOrDefault();

            if (tag == null)
            {
                throw new InstanceNotFoundException(
                    tagName, typeof(Tag).FullName);
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

        public List<Tag> GetMostUsedTags(int startIndex, int count)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            DbSet<Tag> tags = Context.Set<Tag>();

            return tags
                .OrderByDescending(t => t.Comments.Count)
                .ThenBy(t => t.tagName)
                .ThenBy(t => t.tagId)
                .Skip(startIndex)
                .Take(count)
                .ToList();
        }
    }
}
