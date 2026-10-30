using System.Collections.Generic;
using Es.Udc.DotNet.ModelUtil.Dao;
using Es.Udc.DotNet.ModelUtil.Exceptions;
using PagedList;

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
        /// <param name="count">Numero maximo de etiquetas a considerar.</param>
        /// <param name="page">Numero de pagina (empezando en 1).</param>
        /// <param name="pageSize">Cantidad de etiquetas por pagina.</param>
        /// <returns>Lista paginada de las etiquetas mas utilizadas.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        IPagedList<Tag> GetMostUsedTags(int count, int page, int pageSize);
    }
}

