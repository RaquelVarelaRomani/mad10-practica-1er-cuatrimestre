using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Es.Udc.DotNet.PracticaMAD.Model;
using Es.Udc.DotNet.PracticaMAD.Model.Cache;
using Es.Udc.DotNet.PracticaMAD.Model.CategoryDao;
using Es.Udc.DotNet.PracticaMAD.Model.ProductDao;
using Es.Udc.DotNet.PracticaMAD.Model.SERVICES;
using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;

namespace Es.Udc.DotNet.PracticaMAD.Test
{
    /// <summary>
    /// Pruebas de integracion para la funcionalidad adicional:
    /// "4. Cacheado de busquedas" (mantenimiento en cache de las 5 ultimas consultas).
    /// </summary>
    [TestClass]
    public class IProductSearchCacheTest
    {
        private static IKernel kernel;
        private static IProductService productService;
        private static ISearchCache searchCache;
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

            productService = kernel.Get<IProductService>();
            searchCache = kernel.Get<ISearchCache>();
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
            searchCache.Clear();
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            searchCache.Clear();
            transaction.Dispose();
        }

        #endregion

        #region Helper methods

        private static string UniqueToken(string prefix)
        {
            return prefix + "_" + Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        private static UserProfile CreateSeller(string loginPrefix = "cacheSeller")
        {
            string uniqueLogin = UniqueToken(loginPrefix);

            UserProfile seller = new UserProfile
            {
                loginName = uniqueLogin,
                enPassword = "encryptedPassword",
                firstName = "Vendedor",
                lastName = "Cache",
                email = uniqueLogin + "@udc.es",
                language = "es",
                country = "ES",
                isExternal = false
            };

            userProfileDao.Create(seller);
            return seller;
        }

        private static Category CreateCategory(string namePrefix = "CatCache")
        {
            Category category = new Category
            {
                categoryName = UniqueToken(namePrefix)
            };

            categoryDao.Create(category);
            return category;
        }

        private static Product CreateProduct(string productName, Category category, UserProfile seller)
        {
            Product product = new Product
            {
                productName = productName,
                description = "Producto de prueba para cache de busquedas",
                creationDate = DateTime.Now,
                categoryId = category.categoryId,
                sellerId = seller.usrId
            };

            productDao.Create(product);
            return product;
        }

        #endregion

        /// <summary>
        /// Comprueba que la cache esta configurada por defecto para mantener
        /// exactamente las 5 ultimas consultas en memoria.
        /// </summary>
        [TestMethod]
        public void SearchCacheDefaultMaxEntriesIsFiveTest()
        {
            Assert.AreEqual(SearchCache.DefaultMaxEntries, searchCache.MaxEntries);
            Assert.AreEqual(5, searchCache.MaxEntries);
            Assert.AreEqual(0, searchCache.Count);

            UserProfile seller = CreateSeller();
            Category category = CreateCategory();
            string term = UniqueToken("CamaraReflex");
            CreateProduct(term + " Pro", category, seller);

            string expectedKey = SearchCacheKeyGenerator.BuildProductSearchKey(term, null, 0, 10);
            Assert.IsFalse(searchCache.Contains(expectedKey));

            productService.FindProductByName(term, 0, 10);

            Assert.AreEqual(1, searchCache.Count);
            Assert.IsTrue(searchCache.Contains(expectedKey));
        }

        /// <summary>
        /// Comprueba que cuando se repite una busqueda incluida en la cache,
        /// es la propia cache quien proporciona los resultados sin acudir a base de datos
        /// (incluso si se han insertado nuevos productos o actualizado valores en BD).
        /// </summary>
        [TestMethod]
        public void CachedSearchReturnsResultFromCacheWithoutQueryingDatabaseTest()
        {
            UserProfile seller = CreateSeller();
            Category category = CreateCategory();
            string term = UniqueToken("PortatilGaming");
            string originalName = term + " ModeloOriginal";

            Product originalProduct = CreateProduct(originalName, category, seller);

            // Primera consulta (MISS): acude a BD y almacena el resultado en cache
            Block<ProductVO> firstResult = productService.FindProductByName(term, 0, 10);
            Assert.AreEqual(1, firstResult.Items.Count);
            Assert.AreEqual(originalName, firstResult.Items[0].Name);
            Assert.AreEqual(1, searchCache.Count);

            // Modificamos el producto existente e insertamos uno nuevo directamente en BD
            originalProduct.productName = term + " ModeloActualizadoEnBD";
            productDao.Update(originalProduct);
            CreateProduct(term + " ModeloNuevoEnBD", category, seller);

            // Verificamos que en BD realmente existen ahora 2 productos con los nuevos nombres
            List<Product> dbProducts = productDao.FindByKeywords(term, null, 0, 10);
            Assert.AreEqual(2, dbProducts.Count);

            // Segunda consulta identica (HIT): debe devolver el resultado cacheado original
            Block<ProductVO> cachedResult = productService.FindProductByName(term, 0, 10);
            Assert.AreEqual(1, cachedResult.Items.Count);
            Assert.AreEqual(originalName, cachedResult.Items[0].Name);
            Assert.AreEqual(1, searchCache.Count);
        }

        /// <summary>
        /// Comprueba que la clave de cache normaliza mayusculas/minusculas y espacios,
        /// y unifica FindProductByName con FindProducts sin filtro de categoria.
        /// </summary>
        [TestMethod]
        public void CachedSearchNormalizesCaseAndWhitespaceAcrossOverloadsTest()
        {
            UserProfile seller = CreateSeller();
            Category category = CreateCategory();
            string term = UniqueToken("TecladoMecanico");

            CreateProduct(term + " RGB", category, seller);

            // Primera busqueda con espacios laterales y mezcla de mayusculas/minusculas
            Block<ProductVO> initial = productService.FindProductByName("   " + term + "   ", 0, 10);
            Assert.AreEqual(1, initial.Items.Count);
            Assert.AreEqual(1, searchCache.Count);

            // Insertamos un segundo producto en BD tras poblar la cache
            CreateProduct(term + " TKL", category, seller);

            // Todas estas variantes equivalentes deben acertar en la misma entrada de cache
            Block<ProductVO> upperResult = productService.FindProductByName(term.ToUpperInvariant(), 0, 10);
            Block<ProductVO> lowerViaFindProductsNull = productService.FindProducts(term.ToLowerInvariant(), null, 0, 10);
            Block<ProductVO> viaFindProductsZeroCat = productService.FindProducts(term, 0, 0, 10);

            Assert.AreEqual(1, searchCache.Count);
            Assert.AreEqual(1, upperResult.Items.Count);
            Assert.AreEqual(1, lowerViaFindProductsNull.Items.Count);
            Assert.AreEqual(1, viaFindProductsZeroCat.Items.Count);
        }

        /// <summary>
        /// Comprueba que la cache mantiene exactamente las 5 ultimas consultas
        /// y que al realizar una 6a consulta distinta se expulsa la mas antigua.
        /// </summary>
        [TestMethod]
        public void CacheRetainsAtMostFiveLastQueriesAndEvictsOldestTest()
        {
            UserProfile seller = CreateSeller();
            Category category = CreateCategory();

            string[] terms = new string[6];
            string[] keys = new string[6];

            for (int i = 0; i < 6; i++)
            {
                terms[i] = UniqueToken("Consulta" + (i + 1));
                keys[i] = SearchCacheKeyGenerator.BuildProductSearchKey(terms[i], null, 0, 10);
                CreateProduct(terms[i] + "_Inicial", category, seller);
            }

            // Ejecutamos las 5 primeras consultas distintas (Q1..Q5)
            for (int i = 0; i < 5; i++)
            {
                Block<ProductVO> res = productService.FindProductByName(terms[i], 0, 10);
                Assert.AreEqual(1, res.Items.Count);
            }

            Assert.AreEqual(5, searchCache.Count);
            for (int i = 0; i < 5; i++)
            {
                Assert.IsTrue(searchCache.Contains(keys[i]));
            }

            // Insertamos un segundo producto en BD para Q1..Q5
            for (int i = 0; i < 5; i++)
            {
                CreateProduct(terms[i] + "_ExtraBD", category, seller);
            }

            // Ejecutamos la 6a consulta distinta (Q6) -> debe expulsar Q1 (la mas antigua)
            Block<ProductVO> q6First = productService.FindProductByName(terms[5], 0, 10);
            Assert.AreEqual(1, q6First.Items.Count);
            Assert.AreEqual(5, searchCache.Count);

            Assert.IsFalse(searchCache.Contains(keys[0]), "La consulta mas antigua (Q1) debio ser expulsada.");
            for (int i = 1; i < 6; i++)
            {
                Assert.IsTrue(searchCache.Contains(keys[i]), "La consulta Q" + (i + 1) + " debe seguir en cache.");
            }

            // Insertamos tambien un segundo producto en BD para Q6
            CreateProduct(terms[5] + "_ExtraBD", category, seller);

            // Comprobamos primero Q2..Q6: todas siguen en cache y devuelven 1 elemento (sin ir a BD)
            for (int i = 1; i < 6; i++)
            {
                Block<ProductVO> cachedRes = productService.FindProductByName(terms[i], 0, 10);
                Assert.AreEqual(1, cachedRes.Items.Count, "Q" + (i + 1) + " debio servirse desde la cache.");
            }

            // Comprobamos por ultimo Q1: al haber sido expulsada, acude a BD y devuelve los 2 productos
            Block<ProductVO> q1Reloaded = productService.FindProductByName(terms[0], 0, 10);
            Assert.AreEqual(2, q1Reloaded.Items.Count);
            Assert.AreEqual(5, searchCache.Count);
        }

        /// <summary>
        /// Comprueba que al repetir una consulta ya presente en la cache, esta pasa a ser
        /// la mas reciente de las 5 ultimas consultas, expulsandose la mas antigua restante.
        /// </summary>
        [TestMethod]
        public void RepeatedQueryRefreshesRecencySoOldestRemainingQueryIsEvictedTest()
        {
            UserProfile seller = CreateSeller();
            Category category = CreateCategory();

            string[] terms = new string[6];
            string[] keys = new string[6];

            for (int i = 0; i < 6; i++)
            {
                terms[i] = UniqueToken("Recencia" + (i + 1));
                keys[i] = SearchCacheKeyGenerator.BuildProductSearchKey(terms[i], null, 0, 10);
                CreateProduct(terms[i] + "_Inicial", category, seller);
            }

            // Poblar cache con Q1, Q2, Q3, Q4, Q5 (orden de antiguedad: Q1 es la mas antigua)
            for (int i = 0; i < 5; i++)
            {
                productService.FindProductByName(terms[i], 0, 10);
            }

            // Repetimos Q1 -> HIT en cache y Q1 pasa a ser la consulta mas reciente (Q2 queda como la mas antigua)
            Block<ProductVO> q1Hit = productService.FindProductByName(terms[0], 0, 10);
            Assert.AreEqual(1, q1Hit.Items.Count);
            Assert.AreEqual(5, searchCache.Count);

            // Anadimos productos adicionales en BD para Q1 y Q2
            CreateProduct(terms[0] + "_ExtraBD", category, seller);
            CreateProduct(terms[1] + "_ExtraBD", category, seller);

            // Ejecutamos Q6 -> debe expulsar Q2 y conservar Q1
            productService.FindProductByName(terms[5], 0, 10);

            Assert.AreEqual(5, searchCache.Count);
            Assert.IsTrue(searchCache.Contains(keys[0]), "Q1 fue reutilizada recientemente y debe permanecer en cache.");
            Assert.IsFalse(searchCache.Contains(keys[1]), "Q2 era la mas antigua y debio ser expulsada.");

            // Q1 sigue sirviendose desde cache (1 producto)
            Block<ProductVO> q1FromCache = productService.FindProductByName(terms[0], 0, 10);
            Assert.AreEqual(1, q1FromCache.Items.Count);

            // Q2 fue expulsada y vuelve a consultar la BD (2 productos)
            Block<ProductVO> q2FromDb = productService.FindProductByName(terms[1], 0, 10);
            Assert.AreEqual(2, q2FromDb.Items.Count);
        }

        /// <summary>
        /// Comprueba que distintas paginas (startIndex, count) y distintas categorias
        /// se almacenan bajo claves independientes sin colisionar entre si.
        /// </summary>
        [TestMethod]
        public void DifferentPaginationAndCategoriesAreCachedIndependentlyTest()
        {
            UserProfile seller = CreateSeller();
            Category categoryA = CreateCategory("Electronica");
            Category categoryB = CreateCategory("Hogar");
            string term = UniqueToken("Monitor4K");

            CreateProduct(term + " CatA 1", categoryA, seller);
            CreateProduct(term + " CatA 2", categoryA, seller);
            CreateProduct(term + " CatB 1", categoryB, seller);

            Block<ProductVO> page0 = productService.FindProductByName(term, 0, 1);
            Block<ProductVO> page1 = productService.FindProductByName(term, 1, 1);
            Block<ProductVO> onlyCatA = productService.FindProducts(term, categoryA.categoryId, 0, 10);
            Block<ProductVO> onlyCatB = productService.FindProducts(term, categoryB.categoryId, 0, 10);

            Assert.AreEqual(4, searchCache.Count);

            Assert.AreEqual(1, page0.Items.Count);
            Assert.IsTrue(page0.ExistsMoreItems);
            Assert.AreEqual(1, page1.Items.Count);
            Assert.IsTrue(page1.ExistsMoreItems);
            Assert.AreNotEqual(page0.Items[0].Name, page1.Items[0].Name);

            Assert.AreEqual(2, onlyCatA.Items.Count);
            Assert.IsTrue(onlyCatA.Items.All(p => p.CategoryName == categoryA.categoryName));

            Assert.AreEqual(1, onlyCatB.Items.Count);
            Assert.AreEqual(categoryB.categoryName, onlyCatB.Items[0].CategoryName);
        }

        /// <summary>
        /// Comprueba que una busqueda sin coincidencias tambien queda almacenada
        /// entre las 5 ultimas consultas y se sirve desde la cache al repetirse.
        /// </summary>
        [TestMethod]
        public void EmptySearchResultIsCachedAndServedFromCacheTest()
        {
            UserProfile seller = CreateSeller();
            Category category = CreateCategory();
            string term = UniqueToken("SinResultadosIniciales");

            Block<ProductVO> firstEmpty = productService.FindProductByName(term, 0, 10);
            Assert.AreEqual(0, firstEmpty.Items.Count);
            Assert.IsFalse(firstEmpty.ExistsMoreItems);
            Assert.AreEqual(1, searchCache.Count);

            // Insertamos un producto coincidente en BD despues de haber cacheado el resultado vacio
            CreateProduct(term + " Nuevo", category, seller);

            Block<ProductVO> secondEmptyFromCache = productService.FindProductByName(term, 0, 10);
            Assert.AreEqual(0, secondEmptyFromCache.Items.Count);
            Assert.IsFalse(secondEmptyFromCache.ExistsMoreItems);
        }

        /// <summary>
        /// Comprueba que modificar externamente la lista Items devuelta por una busqueda
        /// no corrompe la entrada almacenada internamente en la cache.
        /// </summary>
        [TestMethod]
        public void MutatingReturnedItemsListDoesNotCorruptCachedEntryTest()
        {
            UserProfile seller = CreateSeller();
            Category category = CreateCategory();
            string term = UniqueToken("AuricularesBT");

            CreateProduct(term + " Studio", category, seller);

            Block<ProductVO> firstResult = productService.FindProductByName(term, 0, 10);
            Assert.AreEqual(1, firstResult.Items.Count);

            // El consumidor vacia su lista devuelta
            firstResult.Items.Clear();
            Assert.AreEqual(0, firstResult.Items.Count);

            // La copia interna en cache debe permanecer intacta
            Block<ProductVO> secondResult = productService.FindProductByName(term, 0, 10);
            Assert.AreEqual(1, secondResult.Items.Count);
            Assert.AreEqual(term + " Studio", secondResult.Items[0].Name);
        }

        /// <summary>
        /// Comprueba que parametros de paginacion invalidos lanzan ArgumentOutOfRangeException
        /// y no almacenan entradas erroneas en la cache.
        /// </summary>
        [TestMethod]
        public void InvalidPaginationThrowsAndDoesNotPolluteCacheTest()
        {
            bool negativeStartThrown = false;
            try
            {
                productService.FindProductByName("iPhone", -1, 10);
            }
            catch (ArgumentOutOfRangeException)
            {
                negativeStartThrown = true;
            }

            Assert.IsTrue(negativeStartThrown);
            Assert.AreEqual(0, searchCache.Count);

            bool zeroCountThrown = false;
            try
            {
                productService.FindProducts("iPhone", null, 0, 0);
            }
            catch (ArgumentOutOfRangeException)
            {
                zeroCountThrown = true;
            }

            Assert.IsTrue(zeroCountThrown);
            Assert.AreEqual(0, searchCache.Count);
        }
    }
}

