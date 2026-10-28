
using Es.Udc.DotNet.PracticaMAD.Model;
using Es.Udc.DotNet.PracticaMAD.Model.ProductDao;
using Es.Udc.DotNet.PracticaMAD.Model.CategoryDao;
using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;
using Es.Udc.DotNet.PracticaMAD.Model.SERVICES;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;
using System.Linq;
using System.Transactions;

namespace Es.Udc.DotNet.PracticaMAD.Test
{
    [TestClass]
    public class IProductServiceTest
    {
        public TestContext TestContext { get; set; }

        private TransactionScope transaction;
        private static IKernel kernel;
        private static IProductService productService;
        private static IProductDao productDao;
        private static ICategoryDao categoryDao;
        private static IUserProfileDao userProfileDao;

        private static UserProfile seller;
        private static Category category;
        private static Product product1;
        private static Product product2;

        private const string login = "sellerTest";
        private const string password = "password";
        private const string firstName = "Vendedor";
        private const string lastName = "Prueba";
        private const string email = "seller@udc.es";
        private const string language = "es";
        private const string country = "ES";

        private static void CreateSeller()
        {
            seller = new UserProfile();
            seller.loginName = login;
            seller.enPassword = password;
            seller.firstName = firstName;
            seller.lastName = lastName;
            seller.email = email;
            seller.language = language;
            seller.country = country;
            seller.isExternal = false;

            userProfileDao.Create(seller);
        }

        private static void CreateCategory()
        {
            category = new Category();
            category.categoryName = "Electronica";
            categoryDao.Create(category);
        }

        private static void CreateProducts()
        {
            product1 = new Product();
            product1.productName = "iPhone 13";
            product1.creationDate = DateTime.Now;
            product1.description = "Telefono movil";
            product1.categoryId = category.categoryId;
            product1.sellerId = seller.usrId;
            productDao.Create(product1);

            product2 = new Product();
            product2.productName = "iPhone 15";
            product2.creationDate = DateTime.Now;
            product2.description = "Telefono movil nuevo";
            product2.categoryId = category.categoryId;
            product2.sellerId = seller.usrId;
            productDao.Create(product2);
        }

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            kernel = TestManager.ConfigureNInjectKernel();
            productService = kernel.Get<IProductService>();
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
            CreateSeller();
            CreateCategory();
            CreateProducts();
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            transaction.Dispose();
        }

        [TestMethod]
        public void FindProductByNameTest()
        {
            Block<ProductDetails> result = productService.FindProductByName("iPhone", 0, 10);

            Assert.AreEqual(2, result.Items.Count);
            Assert.IsTrue(result.Items.Any(p => p.Name == "iPhone 13"));
            Assert.IsTrue(result.Items.Any(p => p.Name == "iPhone 15"));
        }

        [TestMethod]
        public void FindProductByNameIgnoreCaseTest()
        {
            Block<ProductDetails> result = productService.FindProductByName("IPHONE", 0, 10);

            Assert.AreEqual(2, result.Items.Count);
        }

        [TestMethod]
        public void FindProductByNamePartialMatchTest()
        {
            Block<ProductDetails> result = productService.FindProductByName("hone 13", 0, 10);

            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual("iPhone 13", result.Items.First().Name);
        }

        [TestMethod]
        public void FindProductByNameNotFoundTest()
        {
            Block<ProductDetails> result = productService.FindProductByName("ProductoInexistenteXYZ", 0, 10);

            Assert.AreEqual(0, result.Items.Count);
            Assert.IsFalse(result.ExistsMoreItems);
        }

        [TestMethod]
        public void FindProductByNamePaginationTest()
        {
            Block<ProductDetails> result1 = productService.FindProductByName("iPhone", 0, 1);
            Block<ProductDetails> result2 = productService.FindProductByName("iPhone", 1, 1);

            Assert.AreEqual(1, result1.Items.Count);
            Assert.IsTrue(result1.ExistsMoreItems);
            Assert.AreEqual(1, result2.Items.Count);
            Assert.IsFalse(result2.ExistsMoreItems);
            Assert.AreNotEqual(result1.Items.First().Name, result2.Items.First().Name);
        }

        [TestMethod]
        public void FindProductByNameDetailsTest()
        {
            Block<ProductDetails> result = productService.FindProductByName("iPhone 13", 0, 10);

            Assert.AreEqual(1, result.Items.Count);

            ProductDetails details = result.Items.First();

            Assert.AreEqual(product1.productName, details.Name);
            Assert.AreEqual(category.categoryName, details.CategoryName);
            Assert.AreEqual(product1.creationDate, details.CreationDate);
            Assert.AreEqual(seller.usrId, details.SellerId);
            Assert.AreEqual(seller.loginName, details.SellerName);
        }
    }
}
