using Es.Udc.DotNet.ModelUtil.Dao;
using Es.Udc.DotNet.ModelUtil.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao
{
    public interface IUserProfileDao : IGenericDao<UserProfile, long>
    {
        /// <summary>
        /// Busca un perfil de usuario a partir de su loginName.
        /// </summary>
        /// <param name="loginName">El login del usuario.</param>
        /// <returns>El perfil de usuario correspondiente.</returns>
        /// <exception cref="InstanceNotFoundException"/>
        UserProfile FindByLoginName(string loginName);
    }
}

