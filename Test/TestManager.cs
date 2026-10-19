using Ninject;
using System.Configuration;
using System.Data.Entity;
using Es.Udc.DotNet.PracticaMAD.Model;

namespace Es.Udc.DotNet.PracticaMAD.Test
{
    public class TestManager
    {
        /// <summary>
        /// Configures and populates the Ninject kernel
        /// </summary>
        /// <returns>The NInject kernel</returns>
        public static IKernel ConfigureNInjectKernel()
        {
            NinjectSettings settings = new NinjectSettings() { LoadExtensions = true };

            IKernel kernel = new StandardKernel(settings);

            #region PracticaMAD Bindings

            /* 
             * kernel.Bind<IUserProfileDao>().To<UserProfileDaoEntityFramework>();
             * 
             * kernel.Bind<IUserService>().To<UserService>();
             */

            #endregion

            string connectionString =
                ConfigurationManager.ConnectionStrings["PracticaMADEntities"].ConnectionString;

            kernel.Bind<DbContext>().
                To<PracticaMADEntities>().
                InSingletonScope().
                WithConstructorArgument("nameOrConnectionString", connectionString);

            return kernel;
        }


        public static void ClearNInjectKernel(IKernel kernel)
        {
            kernel.Dispose();
        }
    }
}