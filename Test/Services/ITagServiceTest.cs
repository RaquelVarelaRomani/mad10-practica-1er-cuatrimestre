using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Es.Udc.DotNet.PracticaMAD.Model;
using Es.Udc.DotNet.PracticaMAD.Model.CategoryDao;
using Es.Udc.DotNet.PracticaMAD.Model.CommentDao;
using Es.Udc.DotNet.PracticaMAD.Model.ProductDao;
using Es.Udc.DotNet.PracticaMAD.Model.SERVICES;
using Es.Udc.DotNet.PracticaMAD.Model.TagDao;
using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;

namespace Es.Udc.DotNet.PracticaMAD.Test
{
    [TestClass]
    public class ITagServiceTest
    {
        private IKernel kernel;
        private TransactionScope transactionScope;
        private ITagService tagService;
        private ICommentService commentService;
        private ITagDao tagDao;
        private IUserProfileDao userProfileDao;
        private ICategoryDao categoryDao;
        private IProductDao productDao;
        private ICommentDao commentDao;

        [TestInitialize]
        public void TestInitialize()
        {
            transactionScope = new TransactionScope();
            kernel = TestManager.ConfigureNInjectKernel();

            tagService = kernel.Get<ITagService>();
            commentService = kernel.Get<ICommentService>();
            tagDao = kernel.Get<ITagDao>();
            userProfileDao = kernel.Get<IUserProfileDao>();
            categoryDao = kernel.Get<ICategoryDao>();
            productDao = kernel.Get<IProductDao>();
            commentDao = kernel.Get<ICommentDao>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            try
            {
                if (transactionScope != null)
                    transactionScope.Dispose(); // Rollback
            }
            finally
            {
                if (kernel != null)
                    TestManager.ClearNInjectKernel(kernel);
            }
        }

        [TestMethod]
        public void GetAllTagsIncludesUnusedTagWithFrequencyZero()
        {
            string tagName = UniqueName("unused");
            Tag tag = new Tag { tagName = tagName };
            tagDao.Create(tag);

            TagVO result = tagService.GetAllTags()
                .Single(t => t.TagId == tag.tagId);

            Assert.AreEqual(tagName, result.TagName);
            Assert.AreEqual(0, result.Frequency);
        }

        [TestMethod]
        public void GetAllTagsReturnsActualCommentFrequencies()
        {
            UserProfile author = CreateUser();
            Product product = CreateProduct(author);
            string frequentName = UniqueName("frequent");
            string rareName = UniqueName("rare");

            commentService.AddComment(author.usrId, product.productId,
                "Comentario 1", new List<string> { frequentName, rareName });
            commentService.AddComment(author.usrId, product.productId,
                "Comentario 2", new List<string> { frequentName });

            long frequentId = tagDao.FindByName(frequentName).tagId;
            long rareId = tagDao.FindByName(rareName).tagId;
            List<TagVO> tags = tagService.GetAllTags();

            Assert.AreEqual(2, tags.Single(t => t.TagId == frequentId).Frequency);
            Assert.AreEqual(1, tags.Single(t => t.TagId == rareId).Frequency);
            Assert.AreEqual(2, commentDao.GetNumberOfCommentsByTag(frequentId));
        }

        [TestMethod]
        public void GetMostUsedTagsOrdersByDescendingFrequency()
        {
            UserProfile author = CreateUser();
            Product product = CreateProduct(author);
            string frequentName = UniqueName("frequent");
            string rareName = UniqueName("rare");
            string unusedName = UniqueName("unused");

            tagDao.Create(new Tag { tagName = unusedName });
            commentService.AddComment(author.usrId, product.productId,
                "Comentario 1", new List<string> { frequentName, rareName });
            commentService.AddComment(author.usrId, product.productId,
                "Comentario 2", new List<string> { frequentName });

            long frequentId = tagDao.FindByName(frequentName).tagId;
            long rareId = tagDao.FindByName(rareName).tagId;
            long unusedId = tagDao.FindByName(unusedName).tagId;

            // La BD puede tener etiquetas precargadas: pedimos todas,
            // y solo comparamos el orden relativo de las creadas en este test.
            int totalTags = tagDao.FindAll().Count;
            Block<Tag> result = tagService.GetMostUsedTags(totalTags, 0, totalTags);
            List<long> ids = result.Items.Select(t => t.tagId).ToList();

            Assert.AreEqual(totalTags, result.Items.Count);
            Assert.IsFalse(result.ExistsMoreItems);
            Assert.IsTrue(ids.IndexOf(frequentId) >= 0);
            Assert.IsTrue(ids.IndexOf(frequentId) < ids.IndexOf(rareId));
            Assert.IsTrue(ids.IndexOf(rareId) < ids.IndexOf(unusedId));
        }

        [TestMethod]
        public void GetMostUsedTagsSupportsPagination()
        {
            tagDao.Create(new Tag { tagName = UniqueName("one") });
            tagDao.Create(new Tag { tagName = UniqueName("two") });
            tagDao.Create(new Tag { tagName = UniqueName("three") });

            int total = tagDao.FindAll().Count;
            Block<Tag> first = tagService.GetMostUsedTags(total, 0, 1);
            Block<Tag> second = tagService.GetMostUsedTags(total, 1, 1);

            Assert.AreEqual(1, first.Items.Count);
            Assert.IsTrue(first.ExistsMoreItems);
            Assert.AreEqual(1, second.Items.Count);
            Assert.IsTrue(second.ExistsMoreItems);
            Assert.AreNotEqual(first.Items[0].tagId, second.Items[0].tagId);
        }

        [TestMethod]
        public void GetMostUsedTagsWithInvalidCountThrowsArgumentOutOfRangeException()
        {
            ExpectException<ArgumentOutOfRangeException>(() =>
                tagService.GetMostUsedTags(0, 0, 10));
        }

        [TestMethod]
        public void GetMostUsedTagsWithInvalidStartIndexThrowsArgumentOutOfRangeException()
        {
            ExpectException<ArgumentOutOfRangeException>(() =>
                tagService.GetMostUsedTags(10, -1, 10));
        }

        [TestMethod]
        public void GetMostUsedTagsWithInvalidBlockSizeThrowsArgumentOutOfRangeException()
        {
            ExpectException<ArgumentOutOfRangeException>(() =>
                tagService.GetMostUsedTags(10, 0, 0));
        }

        [TestMethod]
        public void GetMostUsedTagsRespectsMaximumTags()
        {
            tagDao.Create(new Tag { tagName = UniqueName("limit_a") });
            tagDao.Create(new Tag { tagName = UniqueName("limit_b") });

            Block<Tag> first = tagService.GetMostUsedTags(1, 0, 1);
            Block<Tag> second = tagService.GetMostUsedTags(1, 1, 1);

            Assert.AreEqual(1, first.Items.Count);
            Assert.IsFalse(first.ExistsMoreItems);
            Assert.AreEqual(0, second.Items.Count);
            Assert.IsFalse(second.ExistsMoreItems);
        }

        private UserProfile CreateUser()
        {
            string login = UniqueName("user");
            UserProfile user = new UserProfile
            {
                loginName = login,
                enPassword = "pwd",
                firstName = "Prueba",
                lastName = "Integracion",
                email = login + "@test.es",
                language = "es",
                country = "ES"
            };
            userProfileDao.Create(user);
            return user;
        }

        private Product CreateProduct(UserProfile seller)
        {
            Category category = new Category
            {
                categoryName = UniqueName("cat")
            };
            categoryDao.Create(category);
            Product product = new Product
            {
                productName = "Producto de prueba",
                creationDate = DateTime.Now,
                description = "Producto para pruebas de integracion",
                image = "test.png",
                categoryId = category.categoryId,
                sellerId = seller.usrId
            };
            productDao.Create(product);
            return product;
        }

        private static string UniqueName(string prefix)
        {
            return prefix + "_" + Guid.NewGuid().ToString("N").Substring(0, 12);
        }

        private static void ExpectException<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException)
            {
                return;
            }

            Assert.Fail("Se esperaba la excepcion " + typeof(TException).Name);
        }
    }
}
