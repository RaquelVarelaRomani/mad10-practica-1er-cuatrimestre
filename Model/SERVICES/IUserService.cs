using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.ModelUtil.Transactions;
using Es.Udc.DotNet.PracticaMaD.Model.SERVICES;
using Es.Udc.DotNet.PracticaMAD.Model.Exceptions;
using Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao;
using Ninject;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    public interface IUserService
    {
        [Inject]
        IUserProfileDao UserProfileDao { set; }

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="login">Name of the login.</param>
        /// <param name="clearPassword">The clear password.</param>
        /// <param name="details">The details.</param>
        /// <exception cref="DuplicateInstanceException"/>
        [Transactional]
        long RegisterUser(string login, string clearPassword, UserProfileDetails details);

        /// <summary>
        /// Logins the specified login name.
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <param name="password">The password.</param>
        /// <param name="passwordIsEncrypted">if set to <c>true</c> [password is encrypted].</param>
        /// <exception cref="InstanceNotFoundException"/>
        /// <exception cref="IncorrectPasswordException"/>
        [Transactional]
        LoginResult Login(string loginName, string password, bool passwordIsEncrypted);
    }
}
