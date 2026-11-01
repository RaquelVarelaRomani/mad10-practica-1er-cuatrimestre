
using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model;
using Es.Udc.DotNet.PracticaMAD.Model.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model.SERVICES;
using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;
using System.Transactions;
using Es.Udc.DotNet.PracticaMAD.Model.SERVICES.Util;

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

        private IKernel kernel;
        private IUserService userService;
        private IUserProfileDao userProfileDao;

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

        [TestInitialize]
        public void MyTestInitialize()
        {
            kernel = TestManager.ConfigureNInjectKernel();

            userProfileDao = kernel.Get<IUserProfileDao>();
            userService = kernel.Get<IUserService>();
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            TestManager.ClearNInjectKernel(kernel);
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


        [TestMethod]
        public void FindUserProfileDetailsTest()
        {
            using (var scope = new TransactionScope())
            {
                long userId = registerUser();

                UserProfileDetails details =
                    userService.FindUserProfileDetails(userId);

                Assert.AreEqual(firstName, details.FirstName);
                Assert.AreEqual(lastName, details.LastName);
                Assert.AreEqual(email, details.Email);
                Assert.AreEqual(language, details.Language);
                Assert.AreEqual(country, details.Country);
            }
        }


        [TestMethod]
        public void UpdateUserProfileDetailsTest()
        {
            using (var scope = new TransactionScope())
            {
                long userId = registerUser();

                UserProfileDetails newDetails = new UserProfileDetails(
                    "NuevoNombre",
                    "NuevoApellido",
                    "nuevo@udc.es",
                    "gl",
                    "ES"
                );

                userService.UpdateUserProfileDetails(userId, newDetails);

                UserProfileDetails updatedDetails =
                    userService.FindUserProfileDetails(userId);

                Assert.AreEqual(newDetails, updatedDetails);

                // Comprobar que el seudónimo no ha cambiado
                UserProfile user = userProfileDao.Find(userId);
                Assert.AreEqual(login, user.loginName);
            }
        }


        [TestMethod]
        public void ChangePasswordTest()
        {
            using (var scope = new TransactionScope())
            {
                long userId = registerUser();

                string newPassword = "nuevaPassword123";

                userService.ChangePassword(
                    userId,
                    password,
                    newPassword
                );

                UserProfile user = userProfileDao.Find(userId);

                // Comprobar que la nueva contraseña está cifrada
                Assert.AreEqual(
                    PasswordEncrypter.Crypt(newPassword),
                    user.enPassword
                );

                // Comprobar que la contraseña ha cambiado
                Assert.AreNotEqual(
                    PasswordEncrypter.Crypt(password),
                    user.enPassword
                );

                // Comprobar que permite iniciar sesión con la nueva
                LoginResult result = userService.Login(
                    login,
                    newPassword,
                    false
                );

                Assert.AreEqual(userId, result.UsrId);
            }
        }

    }
}
