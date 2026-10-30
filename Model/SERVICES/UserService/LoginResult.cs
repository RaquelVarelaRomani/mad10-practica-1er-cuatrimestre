using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Udc.DotNet.PracticaMaD.Model.SERVICES
{
    [Serializable()]
    public class LoginResult
    {
        public string EncryptedPassword { get; private set; }
        public string FirstName { get; private set; }
        public long UsrId { get; private set; }
        public bool IsAdmin { get; private set; }

        public LoginResult(long usrId, string firstName, String encryptedPassword)
        {
            this.UsrId = usrId;
            this.FirstName = firstName;
            this.EncryptedPassword = encryptedPassword;
        }

        public override bool Equals(object obj)
        {
            return obj is LoginResult result &&
                   EncryptedPassword == result.EncryptedPassword &&
                   FirstName == result.FirstName &&
                   UsrId == result.UsrId;
        }

        public override int GetHashCode()
        {
            return this.UsrId.GetHashCode();
        }
    }
}