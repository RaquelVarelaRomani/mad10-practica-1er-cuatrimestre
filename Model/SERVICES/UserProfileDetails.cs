using System;

namespace Es.Udc.DotNet.PracticaMAD.Model.SERVICES
{
    [Serializable]
    public class UserProfileDetails
    {
        #region Properties
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Language { get; private set; }
        public string Country { get; private set; }
        #endregion

        public UserProfileDetails(string firstName, string lastName,string email, string language, string country) 
        { 
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Language = language;
            Country = country;
        }

        public override bool Equals(object obj)
        {
            return obj is UserProfileDetails details &&
                FirstName == details.FirstName &&
                LastName == details.LastName &&
                Language == details.Language && Country == details.Country && Email == details.Email;
        }

        public override int GetHashCode()
        {
            return FirstName.GetHashCode();
        }
    }
}
