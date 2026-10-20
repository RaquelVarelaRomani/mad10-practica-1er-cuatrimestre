using System.Collections.Generic;
using Es.Udc.DotNet.ModelUtil.Dao;
using Es.Udc.DotNet.ModelUtil.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.TagDao
{
    public interface ITagDao : IGenericDao<Tag, long>
    {
        /// <summary>
        /// Comprueba si existe una etiqueta con el nombre indicado (case-insensitive).
        /// </summary>
        /// <param name="tagName">Nombre de la etiqueta.</param>
        /// <returns>true si existe; false en caso contrario.</returns>
        bool ExistsByName(string tagName);

        /// <summary>
        /// Busca una etiqueta por su nombre exacto (case-insensitive).
        /// </summary>
        /// <param name="tagName">Nombre de la etiqueta.</param>
        /// <returns>La etiqueta encontrada.</returns>
        /// <exception cref="InstanceNotFoundException"/>
        Tag FindByName(string tagName);

        /// <summary>
        /// Obtiene todas las etiquetas registradas en el sistema.
        /// </summary>
        /// <returns>Lista de todas las etiquetas.</returns>
        List<Tag> FindAll();

        /// <summary>
        /// Obtiene todas las etiquetas junto con el numero de comentarios en los que se utiliza cada una (para la nube de etiquetas).
        /// </summary>
        /// <returns>Diccionario con cada etiqueta y su frecuencia de uso.</returns>
        Dictionary<Tag, int> GetTagCloud();
    }
}

