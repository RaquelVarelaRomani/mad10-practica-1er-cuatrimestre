using System;
using System.Collections.Generic;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model;
using Es.Udc.DotNet.PracticaMAD.Model.CategoryDao;
using Es.Udc.DotNet.PracticaMAD.Model.CommentDao;
using Es.Udc.DotNet.PracticaMAD.Model.CommentReportDao;
using Es.Udc.DotNet.PracticaMAD.Model.ProductDao;
using Es.Udc.DotNet.PracticaMAD.Model.SellerRatingDao;
using Es.Udc.DotNet.PracticaMAD.Model.TagDao;
using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;

namespace Es.Udc.DotNet.PracticaMAD.Test
{
    [TestClass]
    public class DaoIntegrationTest
    {
        private IKernel kernel;
        private ICategoryDao categoryDao;
        private IUserProfileDao userProfileDao;
        private IProductDao productDao;
        private ICommentDao commentDao;
        private ICommentReportDao commentReportDao;
        private ISellerRatingDao sellerRatingDao;
        private ITagDao tagDao;

        private TransactionScope transactionScope;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void MyTestInitialize()
        {
            // Un contexto independiente por prueba evita entidades cacheadas
            // entre tests y mantiene el aislamiento de los cambios.
            transactionScope = new TransactionScope();
            kernel = TestManager.ConfigureNInjectKernel();
            categoryDao = kernel.Get<ICategoryDao>();
            userProfileDao = kernel.Get<IUserProfileDao>();
            productDao = kernel.Get<IProductDao>();
            commentDao = kernel.Get<ICommentDao>();
            commentReportDao = kernel.Get<ICommentReportDao>();
            sellerRatingDao = kernel.Get<ISellerRatingDao>();
            tagDao = kernel.Get<ITagDao>();
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            try
            {
                if (transactionScope != null)
                    transactionScope.Dispose(); // Rollback: nunca llamamos a Complete()
            }
            finally
            {
                if (kernel != null)
                    TestManager.ClearNInjectKernel(kernel);
            }
        }

        [TestMethod]
        public void TestKernelAndDaosResolution()
        {
            Assert.IsNotNull(categoryDao);
            Assert.IsNotNull(userProfileDao);
            Assert.IsNotNull(productDao);
            Assert.IsNotNull(commentDao);
            Assert.IsNotNull(commentReportDao);
            Assert.IsNotNull(sellerRatingDao);
            Assert.IsNotNull(tagDao);
        }

        [TestMethod]
        public void TestUserProfileDaoOperations()
        {
            string uniqueLogin = "user_" + Guid.NewGuid().ToString("N").Substring(0, 10);
            UserProfile user = new UserProfile
            {
                loginName = uniqueLogin,
                enPassword = "passwordHash123",
                firstName = "Juan",
                lastName = "García",
                email = uniqueLogin + "@udc.es",
                language = "es",
                country = "ES"
            };

            userProfileDao.Create(user);
            Assert.IsTrue(user.usrId > 0);

            UserProfile found = userProfileDao.FindByLoginName(uniqueLogin);
            Assert.AreEqual(user, found);

            UserProfile foundById = userProfileDao.Find(user.usrId);
            Assert.AreEqual(user, foundById);
        }

        [TestMethod]
        [ExpectedException(typeof(InstanceNotFoundException))]
        public void TestUserProfileDaoNotFound()
        {
            userProfileDao.FindByLoginName("non_existent_login_" + Guid.NewGuid().ToString("N"));
        }

        [TestMethod]
        public void TestCategoryDaoOperations()
        {
            Category category = new Category
            {
                categoryName = "Electrónica Test " + Guid.NewGuid().ToString("N").Substring(0, 5)
            };

            categoryDao.Create(category);
            Assert.IsTrue(category.categoryId > 0);

            Category found = categoryDao.Find(category.categoryId);
            Assert.AreEqual(category, found);

            List<Category> all = categoryDao.FindAll();
            Assert.IsTrue(all.Count > 0);
        }

        [TestMethod]
        public void TestProductDaoOperations()
        {
            UserProfile seller = new UserProfile
            {
                loginName = "seller_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                enPassword = "pwd",
                firstName = "Vendedor",
                lastName = "Prueba",
                email = "seller@test.es",
                language = "es",
                country = "ES"
            };
            userProfileDao.Create(seller);

            Category cat = new Category { categoryName = "Cat_" + Guid.NewGuid().ToString("N").Substring(0, 5) };
            categoryDao.Create(cat);

            Product product = new Product
            {
                productName = "Portátil Gaming Pro",
                creationDate = DateTime.Now,
                description = "Portátil de alta gama",
                image = "http://example.com/laptop.jpg",
                categoryId = cat.categoryId,
                sellerId = seller.usrId
            };
            productDao.Create(product);
            Assert.IsTrue(product.productId > 0);

            Product found = productDao.Find(product.productId);
            Assert.AreEqual(product, found);

            List<Product> searchResults = productDao.FindByKeywords("gaming", cat.categoryId, 0, 10);
            Assert.IsTrue(searchResults.Count >= 1);

            int count = productDao.GetNumberOfProductsByKeywords("gaming", cat.categoryId);
            Assert.IsTrue(count >= 1);

            List<Product> sellerProducts = productDao.FindBySellerId(seller.usrId, 0, 10);
            Assert.AreEqual(1, sellerProducts.Count);
        }

        [TestMethod]
        public void TestTagDaoOperations()
        {
            string tagName = "etiqueta_" + Guid.NewGuid().ToString("N").Substring(0, 6);
            Tag tag = new Tag
            {
                tagName = tagName
            };

            tagDao.Create(tag);
            Assert.IsTrue(tag.tagId > 0);

            Assert.IsTrue(tagDao.ExistsByName(tagName.ToUpper()));
            Tag found = tagDao.FindByName(tagName);
            Assert.AreEqual(tag, found);

            List<Tag> tags = tagDao.FindAll();
            Assert.IsTrue(tags.Count > 0);
        }

        [TestMethod]
        public void TestCommentAndReportOperations()
        {
            UserProfile author = new UserProfile
            {
                loginName = "author_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                enPassword = "pwd",
                firstName = "Autor",
                lastName = "Comentario",
                email = "author@test.es",
                language = "es",
                country = "ES"
            };
            userProfileDao.Create(author);

            UserProfile reporter = new UserProfile
            {
                loginName = "reporter_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                enPassword = "pwd",
                firstName = "Reportador",
                lastName = "Comentario",
                email = "reporter@test.es",
                language = "es",
                country = "ES"
            };
            userProfileDao.Create(reporter);

            Category cat = new Category { categoryName = "CatComment_" + Guid.NewGuid().ToString("N").Substring(0, 5) };
            categoryDao.Create(cat);

            Product product = new Product
            {
                productName = "Teclado Mecánico",
                creationDate = DateTime.Now,
                description = "Teclado RGB",
                image = "http://example.com/keyboard.jpg",
                categoryId = cat.categoryId,
                sellerId = author.usrId
            };
            productDao.Create(product);

            Comment comment = new Comment
            {
                productId = product.productId,
                userId = author.usrId,
                commentText = "Excelente producto, muy recomendado.",
                commentDate = DateTime.Now,
                isInReview = false
            };
            commentDao.Create(comment);
            Assert.IsTrue(comment.commentId > 0);

            // Paginacion por posicion inicial (desde 0) y cantidad maxima.
            List<Comment> productComments = commentDao.FindByProductId(product.productId, 0, 10);
            Assert.AreEqual(1, productComments.Count);
            Assert.AreEqual(1, commentDao.GetNumberOfCommentsByProduct(product.productId));

            // Report operations
            CommentReport report = new CommentReport
            {
                commentId = comment.commentId,
                userId = reporter.usrId,
                reason = "Spam publicitario",
                reportDate = DateTime.Now
            };
            commentReportDao.Create(report);
            Assert.IsTrue(report.reportId > 0);

            Assert.IsTrue(commentReportDao.ExistsByCommentAndUser(comment.commentId, reporter.usrId));
            Assert.AreEqual(1, commentReportDao.GetNumberOfReportsByComment(comment.commentId));
        }

        [TestMethod]
        public void TestSellerRatingOperations()
        {
            UserProfile seller = new UserProfile
            {
                loginName = "sellerR_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                enPassword = "pwd",
                firstName = "Vendedor",
                lastName = "Valorado",
                email = "sellerR@test.es",
                language = "es",
                country = "ES"
            };
            userProfileDao.Create(seller);

            UserProfile buyer = new UserProfile
            {
                loginName = "buyerR_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                enPassword = "pwd",
                firstName = "Comprador",
                lastName = "Calificador",
                email = "buyerR@test.es",
                language = "es",
                country = "ES"
            };
            userProfileDao.Create(buyer);

            Category cat = new Category { categoryName = "CatRating_" + Guid.NewGuid().ToString("N").Substring(0, 5) };
            categoryDao.Create(cat);

            Product product = new Product
            {
                productName = "Ratón Inalámbrico",
                creationDate = DateTime.Now,
                description = "Ratón ergonómico",
                image = "http://example.com/mouse.jpg",
                categoryId = cat.categoryId,
                sellerId = seller.usrId
            };
            productDao.Create(product);

            SellerRating rating = new SellerRating
            {
                sellerId = seller.usrId,
                productId = product.productId,
                buyerUserId = buyer.usrId,
                score = 5,
                comment = "Envío rápido y vendedor atento",
                ratingDate = DateTime.Now
            };
            sellerRatingDao.Create(rating);
            Assert.IsTrue(rating.ratingId > 0);

            List<SellerRating> ratings = sellerRatingDao.FindBySellerId(seller.usrId, 0, 10);
            Assert.AreEqual(1, ratings.Count);
            Assert.AreEqual(1, sellerRatingDao.GetNumberOfRatingsBySeller(seller.usrId));
            Assert.AreEqual(5.0, sellerRatingDao.GetAverageRatingBySeller(seller.usrId));

            SellerRating foundRating = sellerRatingDao.FindBySellerAndProductAndBuyer(seller.usrId, product.productId, buyer.usrId);
            Assert.IsNotNull(foundRating);
            Assert.AreEqual(rating, foundRating);
        }
    }
}