using System.Collections.Generic;
using Es.Udc.DotNet.ModelUtil.Transactions;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    public interface ITagService
    {
        /// <summary>
        /// Obtiene las etiquetas mas utilizadas, ordenadas
        /// por frecuencia descendente y paginadas.
        /// </summary>
        /// <param name="maxTags">Numero maximo de etiquetas a considerar.</param>
        /// <param name="startIndex">Posicion inicial, empezando en 0.</param>
        /// <param name="count">Numero maximo de etiquetas por bloque.</param>
        /// <returns>Bloque de etiquetas e indicador de si hay mas resultados.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        [Transactional]
        Block<Tag> GetMostUsedTags(
            int maxTags, int startIndex, int count);

        /// <summary>
        /// Obtiene todas las etiquetas con su frecuencia de uso.
        /// </summary>
        /// <returns>Lista de etiquetas con sus frecuencias.</returns>
        [Transactional]
        List<TagVO> GetAllTags();
    }
}
