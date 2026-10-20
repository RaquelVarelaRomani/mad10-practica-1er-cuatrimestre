using System;
using Es.Udc.DotNet.ModelUtil.Exceptions;

namespace Es.Udc.DotNet.PracticaMAD.Model.Exceptions
{
    /// <summary>
    /// Excepcion lanzada cuando un vendedor intenta autovalorarse en uno de sus productos.
    /// </summary>
    [Serializable]
    public class SelfRatingException : ModelException
    {
        public long UserId { get; private set; }

        public SelfRatingException(long userId)
            : base(String.Format("El usuario con ID {0} no puede valorarse a si mismo.", userId))
        {
            this.UserId = userId;
        }
    }
}

