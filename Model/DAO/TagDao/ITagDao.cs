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
        /// Obtiene las etiquetas mas utilizadas, ordenadas por frecuencia descendente y paginadas.
        /// </summary>
        /// <param name="startIndex">Posicion inicial, empezando en 0.</param>
        /// <param name="count">Numero maximo de etiquetas que se devuelven.</param>
        /// <returns>Lista de las etiquetas mas utilizadas.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        List<Tag> GetMostUsedTags(int startIndex, int count);
    }
}

