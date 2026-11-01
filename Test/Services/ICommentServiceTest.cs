using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model;
using Es.Udc.DotNet.PracticaMAD.Model.CategoryDao;
using Es.Udc.DotNet.PracticaMAD.Model.CommentDao;
using Es.Udc.DotNet.PracticaMAD.Model.CommentReportDao;
using Es.Udc.DotNet.PracticaMAD.Model.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model.ProductDao;
using Es.Udc.DotNet.PracticaMAD.Model.SERVICES;
using Es.Udc.DotNet.PracticaMAD.Model.TagDao;
using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;

namespace Es.Udc.DotNet.PracticaMAD.Test
{
    [TestClass]
    public class ICommentServiceTest
    {
        private IKernel kernel;
        private TransactionScope transactionScope;
        private ICommentService commentService;
        private ICommentDao commentDao;
        private ICommentReportDao commentReportDao;
        private IUserProfileDao userProfileDao;
        private ICategoryDao categoryDao;
        private IProductDao productDao;
        private ITagDao tagDao;

        private UserProfile author;
        private Product product;

        [TestInitialize]
        public void TestInitialize()
        {
            // Cada test usa un DbContext propio y revierte los cambios al terminar.
            transactionScope = new TransactionScope();
            kernel = TestManager.ConfigureNInjectKernel();

            commentService = kernel.Get<ICommentService>();
            commentDao = kernel.Get<ICommentDao>();
            commentReportDao = kernel.Get<ICommentReportDao>();
            userProfileDao = kernel.Get<IUserProfileDao>();
            categoryDao = kernel.Get<ICategoryDao>();
            productDao = kernel.Get<IProductDao>();
            tagDao = kernel.Get<ITagDao>();

            author = CreateUser();
            product = CreateProduct(author);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            try
            {
                // Liberar primero el contexto de EF, mientras la transaccion existe.
                if (kernel != null)
                    TestManager.ClearNInjectKernel(kernel);
            }
            finally
            {
                // Sin Complete(): se deshacen los datos creados por esta prueba.
                if (transactionScope != null)
                    transactionScope.Dispose();
            }
        }

        [TestMethod]
        public void AddCommentWithoutTagsCreatesComment()
        {
            long id = commentService.AddComment(
                author.usrId, product.productId, "  Muy buen producto  ", null);

            Assert.IsTrue(id > 0);
            CommentVO result = commentService.FindCommentById(id);
            Assert.AreEqual(id, result.CommentId);
            Assert.AreEqual(author.usrId, result.UserId);
            Assert.AreEqual(author.loginName, result.UserName);
            Assert.AreEqual(product.productId, result.ProductId);
            Assert.AreEqual("Muy buen producto", result.Text);
            Assert.IsFalse(result.IsInReview);
            Assert.AreEqual(0, result.Tags.Count);
            Assert.AreEqual(1, commentService.GetNumberOfCommentsByProduct(product.productId));
        }

        [TestMethod]
        public void AddCommentCreatesAndReusesTags()
        {
            string existingName = UniqueName("ex");
            string newName = UniqueName("new");
            Tag existing = new Tag { tagName = existingName };
            tagDao.Create(existing);

            long id = commentService.AddComment(
                author.usrId, product.productId, "Con etiquetas",
                new List<string> { existingName, newName, newName.ToUpper(), " " });

            CommentVO result = commentService.FindCommentById(id);
            Assert.AreEqual(2, result.Tags.Count);
            Assert.IsTrue(result.Tags.Any(t => t.TagId == existing.tagId));
            Assert.IsTrue(result.Tags.Any(t => t.TagName == newName));
            Assert.AreEqual(1, commentDao.GetNumberOfCommentsByTag(existing.tagId));
            Assert.AreEqual(1, commentDao.GetNumberOfCommentsByTag(tagDao.FindByName(newName).tagId));
        }

        [TestMethod]
        public void AddCommentWithEmptyTextThrowsArgumentException()
        {
            Assert.AreEqual(0, commentService.GetNumberOfCommentsByProduct(product.productId));

            ExpectException<ArgumentException>(() =>
                commentService.AddComment(author.usrId, product.productId,
                    "  ", new List<string>()));
        }

        [TestMethod]
        public void AddCommentWithUnknownProductThrowsInstanceNotFoundException()
        {
            ExpectException<InstanceNotFoundException>(() =>
                commentService.AddComment(author.usrId, long.MaxValue,
                    "Comentario", null));
        }

        [TestMethod]
        public void AddCommentWithUnknownUserThrowsInstanceNotFoundException()
        {
            ExpectException<InstanceNotFoundException>(() =>
                commentService.AddComment(long.MaxValue, product.productId,
                    "Comentario", null));
        }

        [TestMethod]
        public void UpdateCommentChangesTextAndTags()
        {
            string removedName = UniqueName("removed");
            string retainedName = UniqueName("kept");
            string addedName = UniqueName("added");
            long id = commentService.AddComment(author.usrId, product.productId,
                "Texto original", new List<string> { removedName, retainedName });

            commentService.UpdateComment(id, author.usrId,
                "  Texto editado  ", new List<string> { retainedName, addedName });

            CommentVO result = commentService.FindCommentById(id);
            Assert.AreEqual("Texto editado", result.Text);
            Assert.AreEqual(2, result.Tags.Count);
            Assert.IsFalse(result.Tags.Any(t => t.TagName == removedName));
            Assert.IsTrue(result.Tags.Any(t => t.TagName == retainedName));
            Assert.IsTrue(result.Tags.Any(t => t.TagName == addedName));
            // Se retira la asociacion, pero la etiqueta sigue existiendo.
            Assert.AreEqual(0,
                commentDao.GetNumberOfCommentsByTag(tagDao.FindByName(removedName).tagId));
        }

        [TestMethod]
        public void UpdateCommentByOtherUserThrowsNotCommentAuthorException()
        {
            long id = commentService.AddComment(
                author.usrId, product.productId, "Original", null);
            UserProfile other = CreateUser();

            Assert.AreEqual("Original", commentService.FindCommentById(id).Text);

            ExpectException<NotCommentAuthorException>(() =>
                commentService.UpdateComment(id, other.usrId,
                    "No autorizado", null));
        }

        [TestMethod]
        public void UpdateCommentInReviewThrowsCommentInReviewException()
        {
            long id = commentService.AddComment(
                author.usrId, product.productId, "Original", null);
            Comment comment = commentDao.Find(id);
            comment.isInReview = true;
            commentDao.Update(comment);

            Assert.AreEqual("Original", commentService.FindCommentById(id).Text);

            ExpectException<CommentInReviewException>(() =>
                commentService.UpdateComment(id, author.usrId, "Nuevo", null));
        }

        [TestMethod]
        public void DeleteCommentByAuthorRemovesComment()
        {
            string name = UniqueName("tag");

            long id = commentService.AddComment(
                author.usrId, product.productId,
                "Eliminar", new List<string> { name });

            UserProfile reporter = CreateUser();

            commentService.ReportComment(
                id, reporter.usrId, "Contenido inapropiado");

            // Cerramos el kernel anterior y su DbContext
            TestManager.ClearNInjectKernel(kernel);

            // Creamos un kernel con un DbContext nuevo
            kernel = TestManager.ConfigureNInjectKernel();

            // Obtenemos los servicios y DAOs del nuevo kernel
            var newCommentService = kernel.Get<ICommentService>();
            var newCommentDao = kernel.Get<ICommentDao>();
            var newReportDao = kernel.Get<ICommentReportDao>();
            var newTagDao = kernel.Get<ITagDao>();

            // Eliminamos utilizando el Remove() genérico
            newCommentService.DeleteComment(id, author.usrId);

            // Comprobaciones
            Assert.AreEqual(0,
                newCommentService.GetNumberOfCommentsByProduct(product.productId));

            Assert.AreEqual(0,
                newReportDao.GetNumberOfReportsByComment(id));

            Assert.IsTrue(newTagDao.ExistsByName(name));

            ExpectException<InstanceNotFoundException>(() => newCommentDao.Find(id));
        }

        [TestMethod]
        public void DeleteCommentByOtherUserThrowsNotCommentAuthorException()
        {
            long id = commentService.AddComment(
                author.usrId, product.productId, "No borrar", null);

            UserProfile other = CreateUser();

            Assert.AreEqual(1,
                commentService.GetNumberOfCommentsByProduct(product.productId));

            ExpectException<NotCommentAuthorException>(() =>
                commentService.DeleteComment(id, other.usrId));
        }

        [TestMethod]
        public void ReportCommentOnceAndRejectDuplicate()
        {
            long id = commentService.AddComment(
                author.usrId, product.productId, "Para reportar", null);

            UserProfile reporter = CreateUser();

            commentService.ReportComment(id, reporter.usrId, "  Spam  ");

            // Comprobamos que el comentario tiene un reporte
            Assert.AreEqual(1,
                commentReportDao.GetNumberOfReportsByComment(id));

            // Comprobamos que el usuario ha reportado el comentario
            Assert.IsTrue(
                commentReportDao.ExistsByCommentAndUser(id, reporter.usrId));

            // Con un solo reporte, el comentario no está en revisión
            Assert.IsFalse(commentService.FindCommentById(id).IsInReview);

            // No se puede reportar dos veces el mismo comentario
            // La excepción esperada debe ser la última operación
            ExpectException<CommentAlreadyReportedException>(() =>
                commentService.ReportComment(id, reporter.usrId, "Otra vez"));
        }

        [TestMethod]
        public void ThirdReportMarksCommentInReviewAndPreventsEditing()
        {
            // Este test presupone ReportThreshold = 3 en CommentService.
            long id = commentService.AddComment(author.usrId, product.productId,
                "Comentario reportado", null);
            UserProfile first = CreateUser();
            UserProfile second = CreateUser();
            UserProfile third = CreateUser();

            commentService.ReportComment(id, first.usrId, "Motivo 1");
            Assert.IsFalse(commentService.FindCommentById(id).IsInReview);
            commentService.ReportComment(id, second.usrId, "Motivo 2");
            Assert.IsFalse(commentService.FindCommentById(id).IsInReview);
            commentService.ReportComment(id, third.usrId, "Motivo 3");

            Assert.AreEqual(3, commentReportDao.GetNumberOfReportsByComment(id));
            Assert.IsTrue(commentService.FindCommentById(id).IsInReview);
            ExpectException<CommentInReviewException>(() =>
                commentService.UpdateComment(id, author.usrId, "Cambio", null));
        }

        [TestMethod]
        public void ReportCommentWithEmptyReasonThrowsArgumentException()
        {
            long id = commentService.AddComment(
                author.usrId, product.productId, "Comentario", null);

            UserProfile reporter = CreateUser();

            Assert.AreEqual(0,
                commentReportDao.GetNumberOfReportsByComment(id));

            ExpectException<ArgumentException>(() =>
                commentService.ReportComment(id, reporter.usrId, "  "));
        }

        [TestMethod]
        public void FindCommentsByProductAreOrderedAndPaged()
        {
            long first = commentService.AddComment(author.usrId, product.productId, "Primero", null);
            long second = commentService.AddComment(author.usrId, product.productId, "Segundo", null);
            long third = commentService.AddComment(author.usrId, product.productId, "Tercero", null);

            Block<CommentVO> page1 = commentService.FindCommentsByProduct(product.productId, 0, 2);
            Block<CommentVO> page2 = commentService.FindCommentsByProduct(product.productId, 2, 2);

            Assert.AreEqual(2, page1.Items.Count);
            Assert.IsTrue(page1.ExistsMoreItems);
            Assert.AreEqual(third, page1.Items[0].CommentId);
            Assert.AreEqual(second, page1.Items[1].CommentId);
            Assert.AreEqual(1, page2.Items.Count);
            Assert.IsFalse(page2.ExistsMoreItems);
            Assert.AreEqual(first, page2.Items[0].CommentId);
            Assert.AreEqual(author.loginName, page1.Items[0].UserName);
        }

        [TestMethod]
        public void FindCommentsByUserReturnsOnlyOwnComments()
        {
            Product anotherProduct = CreateProduct(author);
            UserProfile anotherAuthor = CreateUser();
            long first = commentService.AddComment(author.usrId, product.productId, "Uno", null);
            long second = commentService.AddComment(author.usrId, anotherProduct.productId, "Dos", null);
            commentService.AddComment(anotherAuthor.usrId, product.productId, "Ajeno", null);

            Block<CommentVO> comments = commentService.FindCommentsByUser(author.usrId, 0, 10);
            Assert.AreEqual(2, comments.Items.Count);
            Assert.IsFalse(comments.ExistsMoreItems);
            Assert.IsTrue(comments.Items.Any(c => c.CommentId == first));
            Assert.IsTrue(comments.Items.Any(c => c.CommentId == second));
            Assert.IsTrue(comments.Items.All(c => c.UserId == author.usrId));
        }

        [TestMethod]
        public void FindCommentsByTagIdWorks()
        {
            string tagA = UniqueName("a");
            string tagB = UniqueName("b");

            long both = commentService.AddComment(author.usrId, product.productId,
                "Ambas", new List<string> { tagA, tagB });

            long onlyA = commentService.AddComment(author.usrId, product.productId,
                "Solo A", new List<string> { tagA });

            long onlyB = commentService.AddComment(author.usrId, product.productId,
                "Solo B", new List<string> { tagB });

            long tagAId = tagDao.FindByName(tagA).tagId;
            long tagBId = tagDao.FindByName(tagB).tagId;

            Block<CommentVO> byA =
                commentService.FindCommentsByTagId(tagAId, 0, 10);

            Block<CommentVO> byB =
                commentService.FindCommentsByTagId(tagBId, 0, 10);

            // La etiqueta A tiene dos comentarios.
            Assert.AreEqual(2, byA.Items.Count);
            Assert.IsFalse(byA.ExistsMoreItems);
            Assert.IsTrue(byA.Items.Any(c => c.CommentId == both));
            Assert.IsTrue(byA.Items.Any(c => c.CommentId == onlyA));
            Assert.IsFalse(byA.Items.Any(c => c.CommentId == onlyB));

            // La etiqueta B también tiene dos comentarios.
            Assert.AreEqual(2, byB.Items.Count);
            Assert.IsFalse(byB.ExistsMoreItems);
            Assert.IsTrue(byB.Items.Any(c => c.CommentId == both));
            Assert.IsTrue(byB.Items.Any(c => c.CommentId == onlyB));
            Assert.IsFalse(byB.Items.Any(c => c.CommentId == onlyA));
        }

        [TestMethod]
        public void FindCommentsByProductWithInvalidStartIndexThrowsArgumentOutOfRangeException()
        {
            ExpectException<ArgumentOutOfRangeException>(() =>
                commentService.FindCommentsByProduct(product.productId, -1, 10));
        }

        [TestMethod]
        public void FindCommentsByProductWithInvalidCountThrowsArgumentOutOfRangeException()
        {
            ExpectException<ArgumentOutOfRangeException>(() =>
                commentService.FindCommentsByProduct(product.productId, 0, 0));
        }

        [TestMethod]
        public void FindUnknownCommentThrowsInstanceNotFoundException()
        {
            ExpectException<InstanceNotFoundException>(() =>
                commentService.FindCommentById(long.MaxValue));
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
            Product newProduct = new Product
            {
                productName = "Producto de prueba",
                creationDate = DateTime.Now,
                description = "Producto para pruebas de integracion",
                image = "test.png",
                categoryId = category.categoryId,
                sellerId = seller.usrId
            };
            productDao.Create(newProduct);
            return newProduct;
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
