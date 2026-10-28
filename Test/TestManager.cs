using Ninject;
using System.Configuration;
using System.Data.Entity;
using Es.Udc.DotNet.PracticaMAD.Model;
using Es.Udc.DotNet.PracticaMAD.Model.CategoryDao;
using Es.Udc.DotNet.PracticaMAD.Model.CommentDao;
using Es.Udc.DotNet.PracticaMAD.Model.CommentReportDao;
using Es.Udc.DotNet.PracticaMAD.Model.ProductDao;
using Es.Udc.DotNet.PracticaMAD.Model.SellerRatingDao;
using Es.Udc.DotNet.PracticaMAD.Model.TagDao;
using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;
using Es.Udc.DotNet.PracticaMAD.Model.SERVICES;

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

            kernel.Bind<IUserProfileDao>().To<UserProfileDaoEntityFramework>();
            kernel.Bind<ICategoryDao>().To<CategoryDaoEntityFramework>();
            kernel.Bind<IProductDao>().To<ProductDaoEntityFramework>();
            kernel.Bind<ICommentDao>().To<CommentDaoEntityFramework>();
            kernel.Bind<ICommentReportDao>().To<CommentReportDaoEntityFramework>();
            kernel.Bind<ISellerRatingDao>().To<SellerRatingDaoEntityFramework>();
            kernel.Bind<ITagDao>().To<TagDaoEntityFramework>();
            kernel.Bind<IUserService>()
                .To<UserService>();
            #endregion

            string connectionString =
                ConfigurationManager.ConnectionStrings["PracticaMADModelEntities"].ConnectionString;

            kernel.Bind<DbContext>().
                To<PracticaMADModelEntities>().
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