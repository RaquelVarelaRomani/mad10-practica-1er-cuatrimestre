using System.Collections.Generic;
using Es.Udc.DotNet.ModelUtil.Transactions;
using PagedList;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    public interface ITagService
    {
        /// <summary>
        /// Obtiene las etiquetas mas utilizadas, ordenadas
        /// por frecuencia descendente y paginadas.
        /// </summary>
        /// <param name="count">Numero maximo de etiquetas.</param>
        /// <param name="page">Numero de pagina (empezando en 1).</param>
        /// <param name="pageSize">Etiquetas por pagina.</param>
        /// <returns>Lista paginada de etiquetas.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        [Transactional]
        IPagedList<Tag> GetMostUsedTags(
            int count, int page, int pageSize);

        /// <summary>
        /// Obtiene todas las etiquetas con su frecuencia de uso.
        /// </summary>
        /// <returns>Lista de etiquetas con sus frecuencias.</returns>
        [Transactional]
        List<TagVO> GetAllTags();
    }
}
