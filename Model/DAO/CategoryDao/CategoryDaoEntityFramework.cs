using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Es.Udc.DotNet.ModelUtil.Dao;
using Es.Udc.DotNet.ModelUtil.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.CategoryDao
{
    public class CategoryDaoEntityFramework : GenericDaoEntityFramework<Category, long>, ICategoryDao
    {
        public List<Category> FindAll()
        {
            DbSet<Category> categories = Context.Set<Category>();

            return (from c in categories
                    orderby c.categoryName ascending
                    select c).ToList();
        }

        public Category FindByName(string categoryName)
        {
            DbSet<Category> categories = Context.Set<Category>();

            var category = (from c in categories
                            where c.categoryName == categoryName
                            select c).FirstOrDefault();

            if (category == null)
            {
                throw new InstanceNotFoundException(categoryName, typeof(Category).FullName);
            }

            return category;
        }
    }
}

