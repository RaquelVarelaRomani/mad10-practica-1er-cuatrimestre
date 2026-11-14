using Es.Udc.DotNet.ModelUtil.Exceptions;
using Es.Udc.DotNet.ModelUtil.Transactions;
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
        /// Registra un nuevo usuario en la aplicación.
        /// </summary>
        /// <param name="login">Seudónimo del usuario.</param>
        /// <param name="clearPassword">Contraseña del usuario en texto plano.</param>
        /// <param name="details">Datos del perfil del usuario.</param>
        /// <returns>Identificador del usuario registrado.</returns>
        /// <exception cref="DuplicateInstanceException">
        /// Se lanza si ya existe un usuario con el mismo seudónimo.
        /// </exception>
        [Transactional]
        long RegisterUser(string login, string clearPassword,
            UserProfileDetails details);

        /// <summary>
        /// Autentica a un usuario mediante su seudónimo y contraseña.
        /// </summary>
        /// <param name="loginName">Seudónimo del usuario.</param>
        /// <param name="password">Contraseña del usuario.</param>
        /// <param name="passwordIsEncrypted">
        /// Indica si la contraseña proporcionada está cifrada.
        /// </param>
        /// <returns>Resultado de la autenticación con los datos del usuario.</returns>
        /// <exception cref="InstanceNotFoundException">
        /// Se lanza si el usuario no existe.
        /// </exception>
        /// <exception cref="IncorrectPasswordException">
        /// Se lanza si la contraseña proporcionada es incorrecta.
        /// </exception>
        [Transactional]
        LoginResult Login(string loginName, string password,
            bool passwordIsEncrypted);

        /// <summary>
        /// Obtiene los datos del perfil de un usuario.
        /// </summary>
        /// <param name="userId">Identificador del usuario.</param>
        /// <returns>Datos del perfil del usuario.</returns>
        /// <exception cref="InstanceNotFoundException">
        /// Se lanza si el usuario no existe.
        /// </exception>
        [Transactional]
        UserProfileDetails FindUserProfileDetails(long userId);

        /// <summary>
        /// Actualiza los datos del perfil de un usuario.
        /// </summary>
        /// <param name="userId">Identificador del usuario.</param>
        /// <param name="details">Nuevos datos del perfil del usuario.</param>
        /// <exception cref="InstanceNotFoundException">
        /// Se lanza si el usuario no existe.
        /// </exception>
        [Transactional]
        void UpdateUserProfileDetails(long userId,
            UserProfileDetails details);

        /// <summary>
        /// Cambia la contraseña de un usuario.
        /// </summary>
        /// <param name="userId">Identificador del usuario.</param>
        /// <param name="oldPassword">Contraseña actual del usuario.</param>
        /// <param name="newPassword">Nueva contraseña del usuario.</param>
        /// <exception cref="InstanceNotFoundException">
        /// Se lanza si el usuario no existe.
        /// </exception>
        /// <exception cref="IncorrectPasswordException">
        /// Se lanza si la contraseña actual es incorrecta.
        /// </exception>
        [Transactional]
        void ChangePassword(long userId,
            string oldPassword, string newPassword);

        /// <summary>
        /// Registers an external user.
        /// </summary>
        /// <param name="login">Name of the login.</param>
        /// <param name="details">The details.</param>
        /// <exception cref="DuplicateInstanceException"/>
        long RegisterExternalUser(string login, UserProfileDetails details);

        /// <summary>
        /// Logins the specified external login name.
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <exception cref="InstanceNotFoundException"/>
        LoginResult LoginExternal(string loginName);

        /// <summary>
        /// Signs in or creates a Google account using a subject verified by the web layer.
        /// Accounts are never linked by email or by a user-supplied login name.
        /// </summary>
        [Transactional]
        LoginResult LoginWithGoogle(string subject, UserProfileDetails details);
    }
}
