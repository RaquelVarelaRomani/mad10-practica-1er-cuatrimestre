using System.Data.Entity;
using System.Linq;
using Es.Udc.DotNet.ModelUtil.Dao;
using Es.Udc.DotNet.ModelUtil.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao
{
    public class UserProfileDaoEntityFramework : GenericDaoEntityFramework<UserProfile, long>, IUserProfileDao
    {
        public UserProfile FindByGoogleSubject(string subject)
        {
            UserProfile user = Context.Set<UserProfile>().FirstOrDefault(u => u.googleSubject == subject);
            if (user == null)
                throw new InstanceNotFoundException(subject, typeof(UserProfile).FullName);
            return user;
        }

        /// <summary>
        /// Busca un perfil de usuario a partir de su loginName.
        /// </summary>
        /// <param name="loginName">El login del usuario.</param>
        /// <returns>El perfil de usuario correspondiente.</returns>
        /// <exception cref="InstanceNotFoundException"/>
        public UserProfile FindByLoginName(string loginName)
        {
            DbSet<UserProfile> userProfiles = Context.Set<UserProfile>();

            var result = (from u in userProfiles
                          where u.loginName == loginName
                          select u);

            UserProfile userProfile = result.FirstOrDefault();

            if (userProfile == null)
            {
                throw new InstanceNotFoundException(loginName, typeof(UserProfile).FullName);
            }

            return userProfile;
        }
    }
}

