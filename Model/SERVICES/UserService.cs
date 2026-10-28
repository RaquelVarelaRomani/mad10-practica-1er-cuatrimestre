using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.ModelUtil.Transactions;
using Es.Udc.DotNet.PracticaMaD.Model.SERVICES;
using Es.Udc.DotNet.PracticaMaD.Model.SERVICES.Util;
using Es.Udc.DotNet.PracticaMAD.Model.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
