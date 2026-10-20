using System;
using Es.Udc.DotNet.ModelUtil.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.Exceptions
{
    /// <summary>
    /// Excepcion lanzada cuando la contrasena proporcionada no coincide con la del usuario.
    /// </summary>
    [Serializable]
    public class IncorrectPasswordException : ModelException
    {
        public string LoginName { get; private set; }

        public IncorrectPasswordException(string loginName)
            : base(String.Format("Contrasena incorrecta para el usuario: '{0}'.", loginName))
        {
            this.LoginName = loginName;
        }
    }
}

