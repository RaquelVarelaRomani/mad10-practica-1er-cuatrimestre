using System;
using System.Linq;
using Ninject;
using Ninject.Extensions.Interception;
using Ninject.Extensions.Interception.Attributes;
using Ninject.Extensions.Interception.Request;

namespace Es.Udc.DotNet.PracticaMAD.Model.Cache
{
    /// <summary>
    /// Define el atributo <c>[SearchCache]</c> para interceptar mediante Ninject
    /// los metodos de busqueda cuyos resultados deben almacenarse en la cache.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SearchCacheAttribute : InterceptAttribute
    {
        public SearchCacheAttribute()
        {
            // Se ejecuta antes que [Transactional] (Order = 0) para evitar abrir
            // una transaccion o acceder a base de datos cuando hay acierto en cache.
            this.Order = -1;
        }

        public override IInterceptor CreateInterceptor(IProxyRequest request)
        {
            if (!request.Kernel.GetBindings(typeof(ISearchCache)).Any())
            {
                request.Kernel.Bind<ISearchCache>().To<SearchCache>().InSingletonScope();
            }

            return request.Kernel.Get<SearchCacheInterceptor>();
        }
    }
}

