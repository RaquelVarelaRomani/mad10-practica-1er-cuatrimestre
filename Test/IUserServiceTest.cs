
using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model;
using Es.Udc.DotNet.PracticaMAD.Model.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model.SERVICES;
using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;
using System.Transactions;
using Es.Udc.DotNet.PracticaMaD.Model.SERVICES;
using Es.Udc.DotNet.PracticaMaD.Model.SERVICES.Util;

namespace Es.Udc.DotNet.PracticaMAD.Test
{
    [TestClass]
    public class IUserServiceTest
    {
        public TestContext TestContext { get; set; }

        private const string login = "darras";
        private const string firstName = "Darras";
        private const string lastName = "LastName";
        private const string email = "user@udc.es";
        private const string password = "password";
        private const string language = "es";
        private const string country = "ES";

        private static IKernel kernel;
        private static IUserService userService;
        private static IUserProfileDao userProfileDao;

        #region Additional test attributes

        private long registerUser()
        {
            return userService.RegisterUser(
                login,
                password,
                new UserProfileDetails(
                    firstName,
                    lastName,
                    email,
                    language,
                    country
                )
            );
        }

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            kernel = TestManager.ConfigureNInjectKernel();

            userProfileDao = kernel.Get<IUserProfileDao>();
            userService = kernel.Get<IUserService>();
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            TestManager.ClearNInjectKernel(kernel);
        }

        [TestInitialize]
        public void MyTestInitialize()
        {
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
        }

        #endregion

        // REGISTER USER

        [TestMethod]
        public void RegisterUserTest()
        {
            using (var scope = new TransactionScope())
            {
                long userId = registerUser();

                UserProfile user = userProfileDao.Find(userId);

                Assert.AreEqual(userId, user.usrId);
                Assert.AreEqual(login, user.loginName);
                Assert.AreEqual(
                    PasswordEncrypter.Crypt(password),
                    user.enPassword
                );
                Assert.AreEqual(firstName, user.firstName);
                Assert.AreEqual(lastName, user.lastName);
                Assert.AreEqual(email, user.email);
                Assert.AreEqual(language, user.language);
                Assert.AreEqual(country, user.country);
                Assert.IsFalse(user.isExternal);

                // Rollback automático
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateInstanceException))]
        public void RegisterDuplicatedUserTest()
        {
            using (var scope = new TransactionScope())
            {
                registerUser();

                registerUser();
            }
        }

        // LOGIN

        [TestMethod]
        public void LoginClearPasswordTest()
        {
            using (var scope = new TransactionScope())
            {
                long userId = registerUser();

                var expected = new LoginResult(
                    userId,
                    firstName,
                    PasswordEncrypter.Crypt(password)
                );

                var actual = userService.Login(
                    login,
                    password,
                    false
                );

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void LoginEncryptedPasswordTest()
        {
            using (var scope = new TransactionScope())
            {
                long userId = registerUser();

                var expected = new LoginResult(
                    userId,
                    firstName,
                    PasswordEncrypter.Crypt(password)
                );

                var actual = userService.Login(
                    login,
                    PasswordEncrypter.Crypt(password),
                    true
                );

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectPasswordException))]
        public void LoginIncorrectPasswordTest()
        {
            using (var scope = new TransactionScope())
            {
                registerUser();

                userService.Login(
                    login,
                    password + "X",
                    false
                );
            }
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectPasswordException))]
        public void LoginIncorrectEncryptedPasswordTest()
        {
            using (var scope = new TransactionScope())
            {
                registerUser();

                userService.Login(
                    login,
                    PasswordEncrypter.Crypt(password + "X"),
                    true
                );
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InstanceNotFoundException))]
        public void LoginNonExistingUserTest()
        {
            userService.Login(
                "usuarioQueNoExiste123",
                password,
                false
            );
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectPasswordException))]
        public void LoginExternalUserTest()
        {
            using (var scope = new TransactionScope())
            {
                long userId = registerUser();

                UserProfile user = userProfileDao.Find(userId);

                user.isExternal = true;

                userService.Login(
                    login,
                    password,
                    false
                );
            }
        }
    }
}
