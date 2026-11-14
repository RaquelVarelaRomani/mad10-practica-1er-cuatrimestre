using System;

namespace Es.Udc.DotNet.PracticaMAD.Web.HTTP.Session
{
    public class UserSession
    {
        public bool IsExternal { get; set; }

        private long userProfileId;
        private String firstName;

        public long UserProfileId
        {
            get { return userProfileId; }
            set { userProfileId = value; }
        }
        
        public String FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }
    }
}


