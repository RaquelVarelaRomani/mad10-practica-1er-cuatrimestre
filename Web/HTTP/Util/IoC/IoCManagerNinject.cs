using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;
using Es.Udc.DotNet.PracticaMAD.Model.SERVICES;
using Es.Udc.DotNet.PracticaMAD.Model;
using Es.Udc.DotNet.ModelUtil.IoC;
using Ninject;
using System.Configuration;
using System.Data.Entity;
using Es.Udc.DotNet.PracticaMAD.Model.Cache;
using Es.Udc.DotNet.PracticaMAD.Model.ProductDao;

namespace Es.Udc.DotNet.PracticaMAD.HTTP.Util.IoC
{
    internal class IoCManagerNinject : IIoCManager
    {
        private static IKernel kernel;
        private static NinjectSettings settings;

        public void Configure()
        {
            settings = new NinjectSettings() { LoadExtensions = true };
            kernel = new StandardKernel(settings);

            /* UserProfileDao */
            kernel.Bind<IUserProfileDao>().
                To<UserProfileDaoEntityFramework>();

            /* UserService */
            kernel.Bind<IUserService>().
                To<UserService>();

            kernel.Bind<IProductDao>().To<ProductDaoEntityFramework>();
            kernel.Bind<IProductService>().To<ProductService>();
            kernel.Bind<ISearchCache>().To<SearchCache>().InSingletonScope();

            /* DbContext */
            string connectionString =
                ConfigurationManager.ConnectionStrings["PracticaMADModelEntities"].ConnectionString;

            kernel.Bind<DbContext>().
                To<PracticaMADModelEntities>().
                InSingletonScope().
                WithConstructorArgument("nameOrConnectionString", connectionString);
        }

        public T Resolve<T>()
        {
            return kernel.Get<T>();
        }
    }
}
