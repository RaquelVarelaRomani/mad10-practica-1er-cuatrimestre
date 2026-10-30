using System;

namespace Es.Udc.DotNet.PracticaMAD.Model.Cache
{
    /// <summary>
    /// Contrato de la cache en memoria para almacenar los resultados
    /// de las ultimas consultas de busqueda realizadas por los usuarios.
    /// </summary>
    public interface ISearchCache : IDisposable
    {
        /// <summary>
        /// Numero maximo de consultas que se mantienen simultaneamente en la cache.
        /// </summary>
        int MaxEntries { get; }

        /// <summary>
        /// Numero actual de consultas almacenadas en la cache.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Intenta recuperar de la cache el resultado asociado a una clave de consulta.
        /// Si existe, actualiza la consulta como la mas reciente.
        /// </summary>
        /// <typeparam name="T">Tipo del resultado almacenado.</typeparam>
        /// <param name="key">Clave normalizada de la consulta.</param>
        /// <param name="value">Resultado recuperado si existe en la cache.</param>
        /// <returns><c>true</c> si la consulta se encuentra en la cache; <c>false</c> en caso contrario.</returns>
        bool TryGet<T>(string key, out T value) where T : class;

        /// <summary>
        /// Almacena en la cache el resultado de una consulta. Si se supera el limite
        /// maximo (<see cref="MaxEntries"/>), expulsa la consulta mas antigua.
        /// </summary>
        /// <typeparam name="T">Tipo del resultado a almacenar.</typeparam>
        /// <param name="key">Clave normalizada de la consulta.</param>
        /// <param name="value">Resultado de la consulta.</param>
        void Put<T>(string key, T value) where T : class;

        /// <summary>
        /// Comprueba si una clave de consulta esta almacenada actualmente en la cache
        /// sin alterar su orden de recencia.
        /// </summary>
        /// <param name="key">Clave normalizada de la consulta.</param>
        /// <returns><c>true</c> si existe en la cache; <c>false</c> en caso contrario.</returns>
        bool Contains(string key);

        /// <summary>
        /// Elimina todas las entradas almacenadas en la cache.
        /// </summary>
        void Clear();
    }
}

