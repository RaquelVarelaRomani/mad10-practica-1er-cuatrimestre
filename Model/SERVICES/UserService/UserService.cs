using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.ModelUtil.Transactions;
using Es.Udc.DotNet.PracticaMAD.Model.SERVICES.Util;
using Es.Udc.DotNet.PracticaMAD.Model.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;
using Ninject;
using System;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    public class UserService : IUserService
    {
        [Inject]
        public IUserProfileDao UserProfileDao { private get; set; }

        
        /// <exception cref="IncorrectPasswordException"/>
        [Transactional]
        public LoginResult Login(string loginName, string password, bool passwordIsEncrypted)
        {
            UserProfile user = UserProfileDao.FindByLoginName(loginName);

            if (user.isExternal)
                throw new IncorrectPasswordException(user.loginName);

            if (passwordIsEncrypted)
            {
                if (!user.enPassword.Equals(password))
                {
                    throw new IncorrectPasswordException(loginName);
                }
            }
            else
            {
                if (!PasswordEncrypter.IsClearPasswordCorrect(password, user.enPassword))
                {
                    throw new IncorrectPasswordException(loginName);
                }
            }

            return new LoginResult(user.usrId, user.firstName, user.enPassword);
        }

        /// <exception cref="DuplicateInstanceException"/>
        /// /// <exception cref="InstanceNotFoundException"/>
        [Transactional]
        public long RegisterUser(string login, string clearPassword, UserProfileDetails details)
        {
            try
            {
                UserProfileDao.FindByLoginName(login);

                throw new DuplicateInstanceException(login, typeof(UserProfile).FullName);
            }
            catch (InstanceNotFoundException)
            {
                String encryptedPassword = PasswordEncrypter.Crypt(clearPassword);

                UserProfile user = new UserProfile();

                user.loginName = login;
                user.enPassword = encryptedPassword;
                user.firstName = details.FirstName;
                user.lastName = details.LastName;
                user.email = details.Email;
                user.language = details.Language;
                user.country = details.Country;

                UserProfileDao.Create(user);

                return user.usrId;
            }
        }

        /// <exception cref="InstanceNotFoundException"/>
        [Transactional]
        public UserProfileDetails FindUserProfileDetails(long userId)
        {
            UserProfile user = UserProfileDao.Find(userId);

            return new UserProfileDetails(
                user.firstName,
                user.lastName,
                user.email,
                user.language,
                user.country
            );
        }

        /// <exception cref="InstanceNotFoundException"/>
        [Transactional]
        public void UpdateUserProfileDetails(long userId,
            UserProfileDetails details)
        {
            UserProfile user = UserProfileDao.Find(userId);

            user.firstName = details.FirstName;
            user.lastName = details.LastName;
            user.email = details.Email;
            user.language = details.Language;
            user.country = details.Country;

            UserProfileDao.Update(user);
        }

        /// <exception cref="InstanceNotFoundException"/>
        /// <exception cref="IncorrectPasswordException"/>
        [Transactional]
        public void ChangePassword(long userId,
            string oldPassword, string newPassword)
        {
            UserProfile user = UserProfileDao.Find(userId);

            if (user.isExternal ||
                !PasswordEncrypter.IsClearPasswordCorrect(
                    oldPassword, user.enPassword))
            {
                throw new IncorrectPasswordException(user.loginName);
            }

            user.enPassword = PasswordEncrypter.Crypt(newPassword);

            UserProfileDao.Update(user);
        }

        [Transactional]
        public long RegisterExternalUser(string login, UserProfileDetails details)
        {
            try
            {
                UserProfileDao.FindByLoginName(login);

                throw new DuplicateInstanceException(login, typeof(UserProfile).FullName);
            }
            catch (InstanceNotFoundException)
            {
                UserProfile user = new UserProfile();

                user.loginName = login;
                user.enPassword = "";
                user.firstName = details.FirstName;
                user.lastName = details.LastName;
                user.email = details.Email;
                user.language = details.Language;
                user.country = details.Country;
                user.isExternal = true;

                UserProfileDao.Create(user);

                return user.usrId;
            }
        }

        [Transactional]
        public LoginResult LoginExternal(string loginName)
        {
            UserProfile user = UserProfileDao.FindByLoginName(loginName);

            if (!user.isExternal)
            {
                throw new InstanceNotFoundException(loginName, typeof(UserProfile).FullName);
            }

            return new LoginResult(user.usrId, user.firstName, null);
        }
    }
}
