using System;

namespace Es.Udc.DotNet.PracticaMAD.Model.Exceptions
{
    /// <summary>
    /// Excepcion lanzada cuando la contrasena proporcionada no coincide con la del usuario.
    /// </summary>
    [Serializable]
    public class IncorrectPasswordException : Exception
    {
        public string LoginName { get; private set; }

        public IncorrectPasswordException(string loginName)
            : base($"Contrasena incorrecta para el usuario: '{loginName}'.")
        {
            LoginName = loginName;
        }
    }
}

