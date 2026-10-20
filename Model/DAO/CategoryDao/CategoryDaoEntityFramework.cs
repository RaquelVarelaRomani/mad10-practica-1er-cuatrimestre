using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Es.Udc.DotNet.ModelUtil.Dao;
using Es.Udc.DotNet.ModelUtil.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.CategoryDao
{
    public class CategoryDaoEntityFramework : GenericDaoEntityFramework<Category, long>, ICategoryDao
    {
        /// <summary>
        /// Obtiene todas las categorias disponibles en el catalogo ordenadas alfabeticamente.
        /// </summary>
        /// <returns>Lista de todas las categorias.</returns>
        public List<Category> FindAll()
        {
            DbSet<Category> categories = Context.Set<Category>();

            var result = (from c in categories
                          orderby c.categoryName ascending
                          select c).ToList();

            return result;
        }

        /// <summary>
        /// Busca una categoria por su nombre exacto.
        /// </summary>
        /// <param name="categoryName">Nombre de la categoria.</param>
        /// <returns>La categoria correspondiente.</returns>
        /// <exception cref="InstanceNotFoundException"/>
        public Category FindByName(string categoryName)
        {
            DbSet<Category> categories = Context.Set<Category>();

            var result = (from c in categories
                          where c.categoryName == categoryName
                          select c);

            Category category = result.FirstOrDefault();

            if (category == null)
            {
                throw new InstanceNotFoundException(categoryName, typeof(Category).FullName);
            }

            return category;
        }
    }
}

