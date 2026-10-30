using System;
using System.Transactions;
using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model;
using Es.Udc.DotNet.PracticaMAD.Model.CategoryDao;
using Es.Udc.DotNet.PracticaMAD.Model.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model.ProductDao;
using Es.Udc.DotNet.PracticaMAD.Model.SellerRatingDao;
using Es.Udc.DotNet.PracticaMAD.Model.SERVICES;
using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace Es.Udc.DotNet.PracticaMAD.Test
{
    [TestClass]
    public class ISellerServiceTest
    {
        private const long NON_EXISTENT_USER_ID = -1;
        private const long NON_EXISTENT_PRODUCT_ID = -1;
        private const long NON_EXISTENT_RATING_ID = -1;
        private const string NON_EXISTENT_LOGIN = "seudonimo_inexistente_xyz";

        private static IKernel kernel;
        private static ISellerService sellerService;
        private static ISellerRatingDao sellerRatingDao;
        private static IProductDao productDao;
        private static ICategoryDao categoryDao;
        private static IUserProfileDao userProfileDao;

        private TransactionScope transaction;

        public TestContext TestContext { get; set; }

        #region Additional test attributes

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            kernel = TestManager.ConfigureNInjectKernel();

            sellerService = kernel.Get<ISellerService>();
            sellerRatingDao = kernel.Get<ISellerRatingDao>();
            productDao = kernel.Get<IProductDao>();
            categoryDao = kernel.Get<ICategoryDao>();
            userProfileDao = kernel.Get<IUserProfileDao>();
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            TestManager.ClearNInjectKernel(kernel);
        }

        [TestInitialize]
        public void MyTestInitialize()
        {
            transaction = new TransactionScope();
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            transaction.Dispose();
        }

        #endregion

        #region Helper methods

        private static UserProfile CreateUser(string loginPrefix)
        {
            string uniqueLogin = loginPrefix + "_" + Guid.NewGuid().ToString("N").Substring(0, 8);

            UserProfile user = new UserProfile
            {
                loginName = uniqueLogin,
                enPassword = "encryptedPassword",
                firstName = "Nombre",
                lastName = "Apellido",
                email = uniqueLogin + "@udc.es",
                language = "es",
                country = "ES",
                isExternal = false
            };

            userProfileDao.Create(user);
            return user;
        }

        private static Category CreateCategory()
        {
            Category category = new Category
            {
                categoryName = "Cat_" + Guid.NewGuid().ToString("N").Substring(0, 8)
            };

            categoryDao.Create(category);
            return category;
        }

        private static Product CreateProduct(UserProfile seller, Category category, string productName)
        {
            Product product = new Product
            {
                productName = productName,
                creationDate = new DateTime(2026, 10, 15, 9, 0, 0),
                description = "Descripcion de " + productName,
                image = null,
                categoryId = category.categoryId,
                sellerId = seller.usrId
            };

            productDao.Create(product);
            return product;
        }

        #endregion

        #region Funcionalidad 7 - Valorar a un vendedor

        [TestMethod]
        public void RateSellerWithCommentTest()
        {
            UserProfile seller = CreateUser("vendedor");
            UserProfile buyer = CreateUser("comprador");
            Category category = CreateCategory();
            Product product = CreateProduct(seller, category, "Portatil Ultrabook");

            DateTime beforeRating = DateTime.Now.AddSeconds(-2);

            long ratingId = sellerService.RateSeller(
                buyer.usrId,
                product.productId,
                5,
                "  Envio rapido y producto en perfecto estado  ");

            DateTime afterRating = DateTime.Now.AddSeconds(2);

            Assert.IsTrue(ratingId > 0);

            SellerRatingVO actual = sellerService.FindSellerRatingById(ratingId);

            Assert.IsTrue(actual.RatingDate >= beforeRating && actual.RatingDate <= afterRating);

            SellerRatingVO expected = new SellerRatingVO(
                ratingId,
                seller.usrId,
                seller.loginName,
                product.productId,
                product.productName,
                buyer.usrId,
                buyer.loginName,
                5,
                "Envio rapido y producto en perfecto estado",
                actual.RatingDate);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RateSellerWithoutCommentTest()
        {
            UserProfile seller = CreateUser("vendedor");
            UserProfile buyer1 = CreateUser("comprador1");
            UserProfile buyer2 = CreateUser("comprador2");
            Category category = CreateCategory();
            Product product = CreateProduct(seller, category, "Monitor 4K");

            long ratingIdNull = sellerService.RateSeller(buyer1.usrId, product.productId, 1, null);
            long ratingIdWhitespace = sellerService.RateSeller(buyer2.usrId, product.productId, 5, "   ");

            SellerRatingVO rating1 = sellerService.FindSellerRatingById(ratingIdNull);
            SellerRatingVO rating2 = sellerService.FindSellerRatingById(ratingIdWhitespace);

            Assert.AreEqual(1, rating1.Score);
            Assert.IsNull(rating1.Comment);
            Assert.AreEqual(buyer1.loginName, rating1.BuyerLoginName);

            Assert.AreEqual(5, rating2.Score);
            Assert.IsNull(rating2.Comment);
            Assert.AreEqual(buyer2.loginName, rating2.BuyerLoginName);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRatingScoreException))]
        public void RateSellerInvalidScoreTooLowTest()
        {
            UserProfile seller = CreateUser("vendedor");
            UserProfile buyer = CreateUser("comprador");
            Category category = CreateCategory();
            Product product = CreateProduct(seller, category, "Teclado Mecanico");

            sellerService.RateSeller(buyer.usrId, product.productId, 0, "Puntuacion fuera de rango");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidRatingScoreException))]
        public void RateSellerInvalidScoreTooHighTest()
        {
            UserProfile seller = CreateUser("vendedor");
            UserProfile buyer = CreateUser("comprador");
            Category category = CreateCategory();
            Product product = CreateProduct(seller, category, "Teclado Mecanico");

            sellerService.RateSeller(buyer.usrId, product.productId, 6, "Puntuacion fuera de rango");
        }

        [TestMethod]
        [ExpectedException(typeof(SelfRatingException))]
        public void RateSellerSelfRatingTest()
        {
            UserProfile seller = CreateUser("vendedor");
            Category category = CreateCategory();
            Product product = CreateProduct(seller, category, "Auriculares Bluetooth");

            sellerService.RateSeller(seller.usrId, product.productId, 5, "Autovaloracion no permitida");
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateInstanceException))]
        public void RateSellerDuplicateRatingOnSameProductTest()
        {
            UserProfile seller = CreateUser("vendedor");
            UserProfile buyer = CreateUser("comprador");
            Category category = CreateCategory();
            Product product = CreateProduct(seller, category, "Camara Reflex");

            sellerService.RateSeller(buyer.usrId, product.productId, 4, "Primera valoracion");
            sellerService.RateSeller(buyer.usrId, product.productId, 5, "Valoracion duplicada");
        }

        [TestMethod]
        public void RateSellerDifferentProductsOfSameSellerTest()
        {
            UserProfile seller = CreateUser("vendedor");
            UserProfile buyer = CreateUser("comprador");
            Category category = CreateCategory();
            Product product1 = CreateProduct(seller, category, "Disco SSD 1TB");
            Product product2 = CreateProduct(seller, category, "Memoria RAM 32GB");

            long ratingId1 = sellerService.RateSeller(buyer.usrId, product1.productId, 4, "Buen disco");
            long ratingId2 = sellerService.RateSeller(buyer.usrId, product2.productId, 5, "Excelente memoria");

            Assert.AreNotEqual(ratingId1, ratingId2);

            SellerRatingSummaryVO summary = sellerService.GetSellerRatingSummary(seller.loginName);
            Assert.AreEqual(2, summary.TotalRatings);
            Assert.AreEqual(4.5, summary.AverageScore, 1e-6);
        }

        [TestMethod]
        [ExpectedException(typeof(InstanceNotFoundException))]
        public void RateSellerNonExistentBuyerTest()
        {
            UserProfile seller = CreateUser("vendedor");
            Category category = CreateCategory();
            Product product = CreateProduct(seller, category, "Raton Gaming");

            sellerService.RateSeller(NON_EXISTENT_USER_ID, product.productId, 5, "Comprador inexistente");
        }

        [TestMethod]
        [ExpectedException(typeof(InstanceNotFoundException))]
        public void RateSellerNonExistentProductTest()
        {
            UserProfile buyer = CreateUser("comprador");

            sellerService.RateSeller(buyer.usrId, NON_EXISTENT_PRODUCT_ID, 5, "Producto inexistente");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RateSellerCommentTooLongTest()
        {
            UserProfile seller = CreateUser("vendedor");
            UserProfile buyer = CreateUser("comprador");
            Category category = CreateCategory();
            Product product = CreateProduct(seller, category, "Impresora Laser");

            string tooLongComment = new string('x', 1001);
            sellerService.RateSeller(buyer.usrId, product.productId, 3, tooLongComment);
        }

        #endregion

        #region Funcionalidad 8 - Mostrar las valoraciones de un vendedor

        [TestMethod]
        public void GetSellerRatingSummaryByLoginNameTest()
        {
            UserProfile seller = CreateUser("vendedor");
            UserProfile buyer1 = CreateUser("comprador1");
            UserProfile buyer2 = CreateUser("comprador2");
            UserProfile buyer3 = CreateUser("comprador3");
            Category category = CreateCategory();
            Product product = CreateProduct(seller, category, "Tablet Pro");

            sellerService.RateSeller(buyer1.usrId, product.productId, 5, "Excelente trato");
            sellerService.RateSeller(buyer2.usrId, product.productId, 4, "Muy bien");
            sellerService.RateSeller(buyer3.usrId, product.productId, 2, "Tardo en llegar");

            SellerRatingSummaryVO expected = new SellerRatingSummaryVO(
                seller.usrId,
                seller.loginName,
                3,
                11.0 / 3.0);

            SellerRatingSummaryVO actualByLogin = sellerService.GetSellerRatingSummary(seller.loginName);
            SellerRatingSummaryVO actualById = sellerService.GetSellerRatingSummary(seller.usrId);

            Assert.AreEqual(expected, actualByLogin);
            Assert.AreEqual(expected, actualById);
        }

        [TestMethod]
        public void GetSellerRatingSummaryWithoutRatingsTest()
        {
            UserProfile seller = CreateUser("vendedorSinVotos");

            SellerRatingSummaryVO expected = new SellerRatingSummaryVO(
                seller.usrId,
                seller.loginName,
                0,
                0.0);

            SellerRatingSummaryVO actual = sellerService.GetSellerRatingSummary(seller.loginName);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(InstanceNotFoundException))]
        public void GetSellerRatingSummaryNonExistentSellerTest()
        {
            sellerService.GetSellerRatingSummary(NON_EXISTENT_LOGIN);
        }

        [TestMethod]
        public void FindSellerRatingsByLoginNameOrderedByDateAndPaginatedTest()
        {
            UserProfile seller = CreateUser("vendedor");
            UserProfile buyer1 = CreateUser("comprador1");
            UserProfile buyer2 = CreateUser("comprador2");
            UserProfile buyer3 = CreateUser("comprador3");
            Category category = CreateCategory();
            Product product = CreateProduct(seller, category, "Consola Portatil");

            DateTime oldestDate = new DateTime(2026, 10, 20, 10, 0, 0);
            DateTime middleDate = new DateTime(2026, 10, 21, 12, 30, 0);
            DateTime newestDate = new DateTime(2026, 10, 22, 18, 45, 0);

            SellerRating oldestRating = new SellerRating
            {
                sellerId = seller.usrId,
                productId = product.productId,
                buyerUserId = buyer1.usrId,
                score = 3,
                comment = "Correcto",
                ratingDate = oldestDate
            };
            sellerRatingDao.Create(oldestRating);

            SellerRating middleRating = new SellerRating
            {
                sellerId = seller.usrId,
                productId = product.productId,
                buyerUserId = buyer2.usrId,
                score = 4,
                comment = null,
                ratingDate = middleDate
            };
            sellerRatingDao.Create(middleRating);

            SellerRating newestRating = new SellerRating
            {
                sellerId = seller.usrId,
                productId = product.productId,
                buyerUserId = buyer3.usrId,
                score = 5,
                comment = "Inmejorable vendedor",
                ratingDate = newestDate
            };
            sellerRatingDao.Create(newestRating);

            SellerRatingVO expectedNewest = new SellerRatingVO(
                newestRating.ratingId,
                seller.usrId,
                seller.loginName,
                product.productId,
                product.productName,
                buyer3.usrId,
                buyer3.loginName,
                5,
                "Inmejorable vendedor",
                newestDate);

            SellerRatingVO expectedMiddle = new SellerRatingVO(
                middleRating.ratingId,
                seller.usrId,
                seller.loginName,
                product.productId,
                product.productName,
                buyer2.usrId,
                buyer2.loginName,
                4,
                null,
                middleDate);

            SellerRatingVO expectedOldest = new SellerRatingVO(
                oldestRating.ratingId,
                seller.usrId,
                seller.loginName,
                product.productId,
                product.productName,
                buyer1.usrId,
                buyer1.loginName,
                3,
                "Correcto",
                oldestDate);

            // Primera pagina (2 elementos, existen mas)
            Block<SellerRatingVO> firstPage = sellerService.FindSellerRatings(seller.loginName, 0, 2);
            Assert.AreEqual(2, firstPage.Items.Count);
            Assert.IsTrue(firstPage.ExistsMoreItems);
            Assert.AreEqual(expectedNewest, firstPage.Items[0]);
            Assert.AreEqual(expectedMiddle, firstPage.Items[1]);

            // Segunda pagina (1 elemento, no existen mas)
            Block<SellerRatingVO> secondPage = sellerService.FindSellerRatings(seller.loginName, 2, 2);
            Assert.AreEqual(1, secondPage.Items.Count);
            Assert.IsFalse(secondPage.ExistsMoreItems);
            Assert.AreEqual(expectedOldest, secondPage.Items[0]);
        }

        [TestMethod]
        public void FindSellerRatingsCreatedViaServiceMostRecentFirstTest()
        {
            UserProfile seller = CreateUser("vendedor");
            UserProfile buyer1 = CreateUser("comprador1");
            UserProfile buyer2 = CreateUser("comprador2");
            UserProfile buyer3 = CreateUser("comprador3");
            Category category = CreateCategory();
            Product product = CreateProduct(seller, category, "Altavoz Inteligente");

            long firstId = sellerService.RateSeller(buyer1.usrId, product.productId, 2, "Primero");
            long secondId = sellerService.RateSeller(buyer2.usrId, product.productId, 4, "Segundo");
            long thirdId = sellerService.RateSeller(buyer3.usrId, product.productId, 5, "Tercero");

            Block<SellerRatingVO> block = sellerService.FindSellerRatings(seller.loginName, 0, 10);

            Assert.AreEqual(3, block.Items.Count);
            Assert.IsFalse(block.ExistsMoreItems);
            Assert.AreEqual(thirdId, block.Items[0].RatingId);
            Assert.AreEqual(secondId, block.Items[1].RatingId);
            Assert.AreEqual(firstId, block.Items[2].RatingId);
        }

        [TestMethod]
        public void FindSellerRatingsEmptyTest()
        {
            UserProfile seller = CreateUser("vendedorVacio");

            Block<SellerRatingVO> blockByLogin = sellerService.FindSellerRatings(seller.loginName, 0, 10);
            Block<SellerRatingVO> blockById = sellerService.FindSellerRatings(seller.usrId, 0, 10);

            Assert.AreEqual(0, blockByLogin.Items.Count);
            Assert.IsFalse(blockByLogin.ExistsMoreItems);
            Assert.AreEqual(0, blockById.Items.Count);
            Assert.IsFalse(blockById.ExistsMoreItems);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FindSellerRatingsNegativeStartIndexTest()
        {
            UserProfile seller = CreateUser("vendedor");

            sellerService.FindSellerRatings(seller.loginName, -1, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void FindSellerRatingsZeroCountTest()
        {
            UserProfile seller = CreateUser("vendedor");

            sellerService.FindSellerRatings(seller.loginName, 0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(InstanceNotFoundException))]
        public void FindSellerRatingsNonExistentSellerTest()
        {
            sellerService.FindSellerRatings(NON_EXISTENT_LOGIN, 0, 10);
        }

        [TestMethod]
        [ExpectedException(typeof(InstanceNotFoundException))]
        public void FindSellerRatingByIdNonExistentTest()
        {
            sellerService.FindSellerRatingById(NON_EXISTENT_RATING_ID);
        }

        #endregion
    }
}
