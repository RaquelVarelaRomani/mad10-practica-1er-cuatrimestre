using System.Collections.Generic;
using Es.Udc.DotNet.ModelUtil.Dao;
using Es.Udc.DotNet.ModelUtil.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.CategoryDao
{
    public interface ICategoryDao : IGenericDao<Category, long>
    {
        /// <summary>
        /// Obtiene todas las categorias disponibles en el catalogo ordenadas alfabeticamente.
        /// </summary>
        /// <returns>Lista de todas las categorias.</returns>
        List<Category> FindAll();

        /// <summary>
        /// Busca una categoria por su nombre exacto.
        /// </summary>
        /// <param name="categoryName">Nombre de la categoria.</param>
        /// <returns>La categoria correspondiente.</returns>
        /// <exception cref="InstanceNotFoundException"/>
        Category FindByName(string categoryName);
    }
}

