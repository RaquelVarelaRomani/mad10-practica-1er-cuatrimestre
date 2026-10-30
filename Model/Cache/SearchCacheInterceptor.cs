using System;
using Es.Udc.DotNet.ModelUtil.Log;
using Ninject.Extensions.Interception;

namespace Es.Udc.DotNet.PracticaMAD.Model.Cache
{
    /// <summary>
    /// Interceptor AOP de Ninject que consulta <see cref="ISearchCache"/> antes de invocar
    /// el servicio de busqueda y almacena el resultado tras una consulta a base de datos.
    /// </summary>
    public class SearchCacheInterceptor : IInterceptor
    {
        private readonly ISearchCache searchCache;

        public SearchCacheInterceptor(ISearchCache searchCache)
        {
            if (searchCache == null)
            {
                throw new ArgumentNullException("searchCache");
            }

            this.searchCache = searchCache;
        }

        public void Intercept(IInvocation invocation)
        {
            string cacheKey = SearchCacheKeyGenerator.BuildKey(
                invocation.Request.Method,
                invocation.Request.Arguments);

            object cachedResult;
            if (searchCache.TryGet(cacheKey, out cachedResult))
            {
                RecordLog(string.Format("SearchCache HIT for key '{0}'", cacheKey));
                invocation.ReturnValue = cachedResult;
                return;
            }

            RecordLog(string.Format("SearchCache MISS for key '{0}'", cacheKey));
            invocation.Proceed();

            if (invocation.ReturnValue != null)
            {
                searchCache.Put(cacheKey, invocation.ReturnValue);
            }
        }

        private static void RecordLog(string message)
        {
            try
            {
                LogManager.RecordMessage(message, MessageType.Info);
            }
            catch
            {
                // Ignorar fallos de configuracion de log en entornos aislados de prueba
            }
        }
    }
}

