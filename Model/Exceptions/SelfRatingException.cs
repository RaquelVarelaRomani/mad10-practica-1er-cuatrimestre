using System;

namespace Es.Udc.DotNet.PracticaMAD.Model.Exceptions
{
    /// <summary>
    /// Excepcion lanzada cuando un vendedor intenta autovalorarse en uno de sus productos.
    /// </summary>
    [Serializable]
    public class SelfRatingException : Exception
    {
        public long UserId { get; private set; }

        public SelfRatingException(long userId)
            : base($"El usuario con ID {userId} no puede valorarse a si mismo.")
        {
            UserId = userId;
        }
    }
}

