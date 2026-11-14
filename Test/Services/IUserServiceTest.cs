
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

        private long registerExternalUser()
        {
            return userService.RegisterExternalUser(
                email,
                new UserProfileDetails(firstName, lastName, email, language, country)
            );
        }

        [TestMethod]
        public void GoogleLoginCreatesAnExternalAccountWithStableSubject()
        {
            using (var scope = new TransactionScope())
            {
                const string subject = "123456789012345678901";
                var details = new UserProfileDetails(firstName, lastName,
                    "a.long.email.address.for.google@example.com", language, country);
                LoginResult result = userService.LoginWithGoogle(subject, details);
                UserProfile user = userProfileDao.FindByGoogleSubject(subject);
                Assert.AreEqual(result.UsrId, user.usrId);
                Assert.AreEqual(subject, user.googleSubject);
                Assert.IsTrue(user.isExternal);
                Assert.AreEqual("", user.enPassword);
                Assert.AreEqual(30, user.loginName.Length);
                Assert.AreEqual(user.loginName, result.LoginName);
                Assert.AreEqual(details.Email, user.email);
            }
        }

        [TestMethod]
        public void RepeatedGoogleLoginUsesSubjectAndKeepsEditedProfile()
        {
            using (var scope = new TransactionScope())
            {
                var details = new UserProfileDetails(firstName, lastName, email, language, country);
                LoginResult first = userService.LoginWithGoogle("google-subject", details);
                var edited = new UserProfileDetails("Nombre editado", lastName, "edited@example.com", "gl", country);
                userService.UpdateUserProfileDetails(first.UsrId, edited);
                LoginResult second = userService.LoginWithGoogle("google-subject", details);
                Assert.AreEqual(first.UsrId, second.UsrId);
                Assert.AreEqual(edited, userService.FindUserProfileDetails(second.UsrId));
            }
        }

        [TestMethod]
        public void GoogleLoginDoesNotLinkLocalOrOtherGoogleAccountsByEmail()
        {
            using (var scope = new TransactionScope())
            {
                long localId = registerUser();
                var details = new UserProfileDetails(firstName, lastName, email, language, country);
                var first = userService.LoginWithGoogle("first-google-subject", details);
                var second = userService.LoginWithGoogle("second-google-subject", details);
                Assert.AreNotEqual(localId, first.UsrId);
                Assert.AreNotEqual(first.UsrId, second.UsrId);
                Assert.AreEqual(localId, userService.Login(login, password, false).UsrId);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectPasswordException))]
        public void GoogleAccountCannotChangeALocalPassword()
        {
            using (var scope = new TransactionScope())
            {
                var details = new UserProfileDetails(firstName, lastName, email, language, country);
                var result = userService.LoginWithGoogle("google-subject", details);
                userService.ChangePassword(result.UsrId, "", "new-password");
            }
        }

        [TestMethod]
        public void GoogleSubjectsAreCaseSensitive()
        {
            using (var scope = new TransactionScope())
            {
                var details = new UserProfileDetails(firstName, lastName, email, language, country);
                var first = userService.LoginWithGoogle("subject-A", details);
                var second = userService.LoginWithGoogle("subject-a", details);
                Assert.AreNotEqual(first.UsrId, second.UsrId);
            }
        }

        [TestMethod]
        public void RegisterExternalUserTest()
        {
            using (var scope = new TransactionScope())
            {
                long userId = registerExternalUser();

                UserProfile user = userProfileDao.Find(userId);

                Assert.AreEqual(userId, user.usrId);
                Assert.AreEqual(email, user.loginName);
                Assert.AreEqual("", user.enPassword);
                Assert.AreEqual(firstName, user.firstName);
                Assert.AreEqual(lastName, user.lastName);
                Assert.AreEqual(email, user.email);
                Assert.AreEqual(language, user.language);
                Assert.AreEqual(country, user.country);
                Assert.IsTrue(user.isExternal);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateInstanceException))]
        public void RegisterDuplicatedExternalUserTest()
        {
            using (var scope = new TransactionScope())
            {
                registerExternalUser();
                registerExternalUser();
            }
        }

        [TestMethod]
        public void LoginExternalTest()
        {
            using (var scope = new TransactionScope())
            {
                long userId = registerExternalUser();

                var expected =
                    new LoginResult(userId, firstName, null);

                var actual = userService.LoginExternal(email);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InstanceNotFoundException))]
        public void LoginExternalNonExternalUserTest()
        {
            using (var scope = new TransactionScope())
            {
                registerUser(); // Usuario normal

                userService.LoginExternal(login);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InstanceNotFoundException))]
        public void LoginExternalNonExistingUserTest()
        {
            using (var scope = new TransactionScope())
            {
                userService.LoginExternal("noexiste@google.com");
            }
        }

    }
}
