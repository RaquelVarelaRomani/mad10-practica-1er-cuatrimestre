using System.Data.Entity;
using System.Linq;
using Es.Udc.DotNet.ModelUtil.Dao;
using Es.Udc.DotNet.ModelUtil.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.UserProfileDao
{
    public class UserProfileDaoEntityFramework : GenericDaoEntityFramework<UserProfile, long>, IUserProfileDao
    {
        public UserProfile FindByLoginName(string loginName)
        {
            DbSet<UserProfile> userProfiles = Context.Set<UserProfile>();

            var userProfile = (from u in userProfiles
                               where u.loginName == loginName
                               select u).FirstOrDefault();

            if (userProfile == null)
            {
                throw new InstanceNotFoundException(loginName, typeof(UserProfile).FullName);
            }

            return userProfile;
        }
    }
}

